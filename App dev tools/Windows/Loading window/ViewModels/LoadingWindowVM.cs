using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;
using AppDevTools.Templates.MVVM.ViewModel.Base;
using AppDevTools.Windows.LoadingWindow.Models;
using sysTimers = System.Timers;

namespace AppDevTools.Windows.LoadingWindow.ViewModels
{
    public class LoadingWindowVM : ViewModel
    {
        #region Fields

        #region Private
        private string windowName;
        private ImageSource? icon;
        private string title;
        private string annotation;
        private int loadingPercentage;
        private SolidColorBrush? elementBrush;
        private SolidColorBrush? elementBrush0;
        private SolidColorBrush? elementBrush1;
        private SolidColorBrush? elementBrush2;
        private SolidColorBrush? elementBrush6;
        private SolidColorBrush? elementBrush5;
        private SolidColorBrush? elementBrush4;
        private SolidColorBrush? elementBrush3;
        private sysTimers.Timer? animValueTimer;
        #endregion Private

        #endregion Fields

        #region Properties

        #region Public
        public string WindowName
        {
            get => windowName;
            set => Set(ref windowName, value);
        }

        public ImageSource? Icon
        {
            get => icon;
            set => Set(ref icon, value);
        }

        public string Title
        {
            get => title;
            set => Set(ref title, value);
        }

        public string Annotation
        {
            get => annotation;
            set => Set(ref annotation, value);
        }

        public int LoadingPercentage
        {
            get => loadingPercentage;
            set => Set(ref loadingPercentage, value);
        }

        public SolidColorBrush? ElementBrush
        {
            get => elementBrush;
            set => Set(ref elementBrush, value);
        }

        public SolidColorBrush? ElementBrush0
        {
            get => elementBrush0;
            set => Set(ref elementBrush0, value);
        }

        public SolidColorBrush? ElementBrush1
        {
            get => elementBrush1;
            set => Set(ref elementBrush1, value);
        }

        public SolidColorBrush? ElementBrush2
        {
            get => elementBrush2;
            set => Set(ref elementBrush2, value);
        }

        public SolidColorBrush? ElementBrush3
        {
            get => elementBrush3;
            set => Set(ref elementBrush3, value);
        }

        public SolidColorBrush? ElementBrush4
        {
            get => elementBrush4;
            set => Set(ref elementBrush4, value);
        }

        public SolidColorBrush? ElementBrush5
        {
            get => elementBrush5;
            set => Set(ref elementBrush5, value);
        }

        public SolidColorBrush? ElementBrush6
        {
            get => elementBrush6;
            set => Set(ref elementBrush6, value);
        }

        public Thread OwnerThread { get; private set; }
        #endregion Public

        #endregion Properties

        #region Constructors

        #region Public
        public LoadingWindowVM()
        {
            Settings settings= new();

            WindowName = settings.WindowName;
            Title = settings.Title;
            Annotation = settings.Annotation;
            OwnerThread = Thread.CurrentThread;

            SetDefElementsColors();
            SetAnimTimer();
        }

        public LoadingWindowVM(Settings settings)
        {
            settings ??= new Settings();

            WindowName = settings.WindowName;
            Icon = settings.Icon;
            Title = settings.Title;
            Annotation = settings.Annotation;

            if (settings.ElementBrush != null)
            {
                ElementBrush = new SolidColorBrush(Color.FromArgb(255, settings.ElementBrush.Color.R, settings.ElementBrush.Color.G, settings.ElementBrush.Color.B));
                ElementBrush0 = new SolidColorBrush(Color.FromArgb(223, settings.ElementBrush.Color.R, settings.ElementBrush.Color.G, settings.ElementBrush.Color.B));
                ElementBrush1 = new SolidColorBrush(Color.FromArgb(191, settings.ElementBrush.Color.R, settings.ElementBrush.Color.G, settings.ElementBrush.Color.B));
                ElementBrush2 = new SolidColorBrush(Color.FromArgb(159, settings.ElementBrush.Color.R, settings.ElementBrush.Color.G, settings.ElementBrush.Color.B));
                ElementBrush3 = new SolidColorBrush(Color.FromArgb(128, settings.ElementBrush.Color.R, settings.ElementBrush.Color.G, settings.ElementBrush.Color.B));
                ElementBrush4 = new SolidColorBrush(Color.FromArgb(97, settings.ElementBrush.Color.R, settings.ElementBrush.Color.G, settings.ElementBrush.Color.B));
                ElementBrush5 = new SolidColorBrush(Color.FromArgb(66, settings.ElementBrush.Color.R, settings.ElementBrush.Color.G, settings.ElementBrush.Color.B));
                ElementBrush6 = new SolidColorBrush(Color.FromArgb(35, settings.ElementBrush.Color.R, settings.ElementBrush.Color.G, settings.ElementBrush.Color.B));
            }

            OwnerThread = Thread.CurrentThread;

            if (settings.ElementBrush == null)
            {
                SetDefElementsColors();
            }

            SetAnimTimer();
        }
        #endregion Public

        #endregion Constructors

        #region Methods

        #region Private
        private void SetAnimTimer()
        {
            animValueTimer = new sysTimers.Timer(80);
            animValueTimer.Elapsed += (source, e) =>
            {
                try
                {
                    Dispatcher.FromThread(OwnerThread)?.Invoke(() => UpdateElementsColors());
                }
                catch (TaskCanceledException)
                {
                    return;
                }
            };
            animValueTimer.Enabled = true;
        }

        private void SetDefElementsColors()
        {
            Color defElementColor = Settings.DefElementColor;
            ElementBrush = new SolidColorBrush(new Color { A = 255, R = defElementColor.R, G = defElementColor.G, B = defElementColor.B });
            ElementBrush6 = new SolidColorBrush(new Color { A = 223, R = defElementColor.R, G = defElementColor.G, B = defElementColor.B });
            ElementBrush5 = new SolidColorBrush(new Color { A = 191, R = defElementColor.R, G = defElementColor.G, B = defElementColor.B });
            ElementBrush4 = new SolidColorBrush(new Color { A = 159, R = defElementColor.R, G = defElementColor.G, B = defElementColor.B });
            ElementBrush3 = new SolidColorBrush(new Color { A = 128, R = defElementColor.R, G = defElementColor.G, B = defElementColor.B });
            ElementBrush2 = new SolidColorBrush(new Color { A = 97, R = defElementColor.R, G = defElementColor.G, B = defElementColor.B });
            ElementBrush1 = new SolidColorBrush(new Color { A = 66, R = defElementColor.R, G = defElementColor.G, B = defElementColor.B });
            ElementBrush0 = new SolidColorBrush(new Color { A = 35, R = defElementColor.R, G = defElementColor.G, B = defElementColor.B });
        }

        private void UpdateElementsColors()
        {
            ElementBrush = GetBrush(elementBrush);
            ElementBrush0 = GetBrush(elementBrush0);
            ElementBrush1 = GetBrush(elementBrush1);
            ElementBrush2 = GetBrush(elementBrush2);
            ElementBrush3 = GetBrush(elementBrush3);
            ElementBrush4 = GetBrush(elementBrush4);
            ElementBrush5 = GetBrush(elementBrush5);
            ElementBrush6 = GetBrush(elementBrush6);

            static SolidColorBrush? GetBrush(SolidColorBrush? oldBrush)
            {
                if (oldBrush == null)
                {
                    return null;
                }

                Color newColor = oldBrush.Color;

                switch (oldBrush.Color.A)
                {
                    case 255:
                        newColor.A = 223;
                        break;

                    case 223:
                        newColor.A = 191;
                        break;

                    case 191:
                        newColor.A = 159;
                        break;

                    case 159:
                        newColor.A = 128;
                        break;

                    case 128:
                        newColor.A = 97;
                        break;

                    case 97:
                        newColor.A = 66;
                        break;

                    case 66:
                        newColor.A = 35;
                        break;

                    case 35:
                        newColor.A = 255;
                        break;

                }

                return new SolidColorBrush(newColor);
            }
        }
        #endregion Private

        #region Public
        public void StopAnimation()
        {
            animValueTimer?.Stop();
            animValueTimer?.Close();
            animValueTimer?.Dispose();
            animValueTimer = null;
        }
        #endregion Public

        #endregion Methods
    }
}