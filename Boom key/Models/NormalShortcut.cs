using System;

namespace BoomKey.Models
{
    public class NormalShortcut : Shortcut
    {
        #region Constructors

        #region Public
        public NormalShortcut() : base()
        {

        }

        public NormalShortcut(FavoriteShortcut favoriteShortcut)
        {
            favoriteShortcut ??= new FavoriteShortcut();
            Name = favoriteShortcut.Name;
            CreationDate = DateTime.Now.ToString("F");
            PathToExetubaleObj = favoriteShortcut.PathToExetubaleObj;
            Icon = favoriteShortcut.Icon;
            HotKey = new HotKey();
        }

        public NormalShortcut(NormalShortcut shortcut)
        {
            shortcut ??= new NormalShortcut();
            Name = shortcut.Name;
            CreationDate = DateTime.Now.ToString("F");
            PathToExetubaleObj = shortcut.PathToExetubaleObj;
            Icon = shortcut.Icon;
            HotKey = new HotKey();
        }

        public NormalShortcut(string name, string pathToExetubaleFile)
        {
            Name = name;
            CreationDate = DateTime.Now.ToString("F");
            PathToExetubaleObj = pathToExetubaleFile;
            Icon = GetIcon(pathToExetubaleFile);
            HotKey = new HotKey();
        }
        #endregion Public

        #endregion Constructors
    }
}