namespace AppDevTools.Addons.AppUpdateLoader.Models
{
    public class Settings
    {
        #region Properties

        #region Public
        public string AppVersion { get; set; }

        public string Token { get; set; }

        public string OwnerName { get; set; }

        public string RepositoryName { get; set; }

        public string SourceName { get; set; }

        public string PathToLoading { get; set; }
        #endregion Public

        #endregion Properties

        #region Constructors

        #region Public
        public Settings()
        {
            AppVersion = string.Empty;
            Token = string.Empty;
            OwnerName = string.Empty;
            RepositoryName = string.Empty;
            SourceName = string.Empty;
            PathToLoading = string.Empty;
        }

        public Settings(string appVersion, string ownerName, string repositoryName, string sourceName, string pathToLoading)
        {
            AppVersion = appVersion;
            Token = string.Empty;
            OwnerName = ownerName;
            RepositoryName = repositoryName;
            SourceName = sourceName;
            PathToLoading = pathToLoading;
        }

        public Settings(string appVersion, string token, string ownerName, string repositoryName, string sourceName, string pathToLoading)
        {
            AppVersion = appVersion;
            Token = token;
            OwnerName = ownerName;
            RepositoryName = repositoryName;
            SourceName = sourceName;
            PathToLoading = pathToLoading;
        }
        #endregion Public

        #endregion Constructors
    }
}