using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Threading;
using AppDevTools.Addons.AppUpdateLoader.Models;
using Notification.Wpf;
using Notification.Wpf.Classes;
using Octokit;
using appDevToolsExts = AppDevTools.Extensions;

namespace AppDevTools.Addons.AppUpdateLoader
{
    public class UpdateLoader
    {
        #region Fields

        #region Private
        private BackgroundWorker? updateSearcher;
        private WebClient? webClient;
        private NotificationManager? notificationManager;
        #endregion Private

        #endregion Fields

        #region Properties

        #region Public
        public Settings Settings { get; set; }
        #endregion Public

        #endregion Properties

        #region Constructors

        #region Public
        public UpdateLoader()
        {
            Settings = new Settings();
        }

        public UpdateLoader(Settings settings)
        {
            settings ??= new Settings();
            Settings = settings;
        }
        #endregion Public

        #endregion Constructors

        #region Methods

        #region Private
        private void StartSearchingUpdate(object? sender, DoWorkEventArgs e)
        {
            if (sender == null)
            {
                e.Cancel = true;

                return;
            }

            BackgroundWorker updateSearcher = (BackgroundWorker)sender;
            if (updateSearcher.CancellationPending)
            {
                e.Cancel = true;

                return;
            }

            if (e.Argument is Settings settings && settings != null)
            {
                GitHubClient? gitHubClient;
                Release? release;
                ReleaseAsset? releaseAsset;

                try
                {
                    gitHubClient = new(new Connection(new ProductHeaderValue(settings.RepositoryName)));
                }
                catch
                {
                    gitHubClient = null;
                }

                if (gitHubClient != null)
                {
                    Task<Release>? releaseTask;

                    try
                    {
                        releaseTask = gitHubClient.Repository.Release.GetLatest(settings.OwnerName, settings.RepositoryName);
                        releaseTask.Wait();
                    }
                    catch 
                    {
                        releaseTask = null;
                    }

                    if (releaseTask != null && releaseTask.IsCompleted)
                    {
                        release = releaseTask.Result;
                    }
                    else
                    {
                        release = null;
                    }

                    if (release != null)
                    {
                        try
                        {
                            releaseAsset = release.Assets.ToList().Find(asset => asset.Name.Contains(settings.SourceName));
                        }
                        catch
                        {
                            releaseAsset = null;
                        }

                        if (releaseAsset != null)
                        {
                            if (settings.AppVersion != release.TagName)
                            {
                                e.Result = new Update(release.TagName, release.Body, releaseAsset.Size, releaseAsset.BrowserDownloadUrl);
                            }
                        }
                    }
                }
            }
        }

        private void SearchingUpdateFinished(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (sender == null || e.Cancelled)
            {
                return;
            }

            if (e.Result is Update update && update != null)
            {
                UpdateAppeared?.Invoke(update);
                updateSearcher?.RunWorkerAsync(Settings);
            }

            UpdateSearchIsOver?.Invoke();
        }
        #endregion Private

        #region Public
        public void SearchUpdate()
        {
            updateSearcher = new BackgroundWorker() { WorkerSupportsCancellation = true };
            updateSearcher.DoWork += StartSearchingUpdate;
            updateSearcher.RunWorkerCompleted += SearchingUpdateFinished;
            updateSearcher.RunWorkerAsync(Settings);
        }

        public void StopSearchingUpdate()
        {
            if (updateSearcher == null)
            {
                return;
            }

            updateSearcher.CancelAsync();
            updateSearcher.DoWork -= StartSearchingUpdate;
            updateSearcher.RunWorkerCompleted -= SearchingUpdateFinished;
            updateSearcher.Dispose();
        }

        public void DownloadUpdate(Update update)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(update.DownloadLink);
                IWebProxy? proxy = request.Proxy;
                if (proxy != null)
                {
                    Uri? proxyUri = proxy.GetProxy(request.RequestUri);
                    if (proxyUri != null)
                    {
                        request.UseDefaultCredentials = true;
                        request.Proxy = new WebProxy(proxyUri.ToString(), false) { Credentials = CredentialCache.DefaultCredentials };

                        if (File.Exists(Settings.PathToLoading))
                        {
                            File.Delete(Settings.PathToLoading);
                        }
                        
                        webClient = new WebClient()
                        {
                            UseDefaultCredentials = request.UseDefaultCredentials,
                            Proxy = request.Proxy,
                            Credentials = request.Credentials
                        };
                        webClient.DownloadFileAsync(new Uri(update.DownloadLink), Settings.PathToLoading);
                    }
                }
            }
            catch
            {
                return;
            }
        }

        public void DownloadUpdate(Update update, NotificationContent contentForWaitingLoadingUpdate, NotificationContent contentForInstallLoadedUpdate)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(update.DownloadLink);
                IWebProxy? proxy = request.Proxy;
                if (proxy != null)
                {
                    Uri? proxyUri = proxy.GetProxy(request.RequestUri);
                    if (proxyUri != null)
                    {
                        request.UseDefaultCredentials = true;
                        request.Proxy = new WebProxy(proxyUri.ToString(), false) { Credentials = CredentialCache.DefaultCredentials };

                        string? pathToLoadingDir = Path.GetDirectoryName(Settings.PathToLoading);
                        if (Directory.Exists(pathToLoadingDir))
                        {
                            Directory.Delete(pathToLoadingDir, true);
                        }
                        Directory.CreateDirectory(pathToLoadingDir!);

                        notificationManager = new NotificationManager();
                        NotifierProgress<(double?, string, string, bool?)> notifierProgress = notificationManager.ShowProgressBar
                        (
                            contentForWaitingLoadingUpdate.Title, 
                            false, 
                            false, 
                            "", 
                            false, 
                            1, 
                            "", 
                            false, 
                            true,
                            contentForWaitingLoadingUpdate.Background,
                            contentForWaitingLoadingUpdate.Foreground, 
                            null,
                            contentForWaitingLoadingUpdate.Icon, 
                            null, 
                            null, 
                            true
                        );

                        DispatcherTimer? reportTimer = new() { Interval = TimeSpan.FromSeconds(1) };
                        reportTimer.Tick += (sender, e) => { notifierProgress.Report((null, contentForWaitingLoadingUpdate.Message, contentForWaitingLoadingUpdate.Title, false)); };
                        reportTimer.Start();

                        webClient = new WebClient()
                        {
                            UseDefaultCredentials = request.UseDefaultCredentials,
                            Proxy = request.Proxy,
                            Credentials = request.Credentials
                        };
                        webClient.DownloadFileCompleted += (sender, e) =>
                        {
                            reportTimer.Stop();
                            reportTimer = null;

                            if (e.Cancelled)
                            {
                                return;
                            }

                            string? destDirNameForUpdate = Path.GetDirectoryName(Settings.PathToLoading);
                            if (destDirNameForUpdate != null)
                            {
                                ZipFile.ExtractToDirectory(Settings.PathToLoading, destDirNameForUpdate);
                                File.Delete(Settings.PathToLoading);
                                List<string> pathsToDirsInDestDir = Directory.EnumerateDirectories(destDirNameForUpdate).ToList();
                                string? pathToSubDirForUpdate = pathsToDirsInDestDir.Find(x => x.Contains(Settings.SourceName));
                                if (pathToSubDirForUpdate != null)
                                {
                                    appDevToolsExts.File.CopyFilesRecursively(pathToSubDirForUpdate, destDirNameForUpdate);
                                    Directory.Delete(pathToSubDirForUpdate, true);
                                }
                            }

                            notifierProgress.Dispose();
                            notificationManager.Show(contentForInstallLoadedUpdate, null, TimeSpan.MaxValue);
                        };

                        webClient.DownloadFileAsync(new Uri(update.DownloadLink), Settings.PathToLoading);
                    }
                }
            }
            catch
            {
                return;
            }
        }

        public void CancelDownloadUpdate()
        {
            if (webClient == null)
            {
                return;
            }

            webClient.CancelAsync();
            webClient.Dispose();
        }
        #endregion Public

        #endregion Methods

        #region Events

        #region Public
        public event Action? UpdateSearchIsOver;
        public event Action<Update>? UpdateAppeared;
        #endregion Public

        #endregion Events
    }
}