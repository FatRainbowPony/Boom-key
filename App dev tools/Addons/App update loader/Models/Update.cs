namespace AppDevTools.Addons.AppUpdateLoader.Models
{
    public class Update
    {
        #region Properties

        #region Public
        public string Version { get; set; }

        public string Description { get; set; }

        public int Size { get; set; }

        public string DownloadLink { get; set; }
        #endregion Public

        #endregion Properties

        #region Constructors

        #region Public
        public Update()
        {
            Version = string.Empty;
            Description = string.Empty;
            DownloadLink = string.Empty;
        }

        public Update(string version, string description, int size, string downloadLink)
        {
            Version = version;
            Description = description;
            Size = size;
            DownloadLink = downloadLink;
        }
        #endregion Public 

        #endregion Constructors
    }
}