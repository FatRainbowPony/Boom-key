using System;

namespace BoomKey.Models
{
    public class FavoriteShortcut : Shortcut
    {
        #region Constructors

        #region Public
        public FavoriteShortcut() : base()
        {

        }

        public FavoriteShortcut(NormalShortcut shortcut)
        {
            shortcut ??= new NormalShortcut();
            Name = shortcut.Name;
            CreationDate = DateTime.Now.ToString("F");
            PathToExetubaleObj = shortcut.PathToExetubaleObj;
            Icon = shortcut.Icon;
            HotKey = new HotKey();
        }

        public FavoriteShortcut(Shortcut shortcut)
        {
            shortcut ??= new FavoriteShortcut();
            Name = shortcut.Name;
            CreationDate = shortcut.CreationDate;
            PathToExetubaleObj = shortcut.PathToExetubaleObj;
            Icon = shortcut.Icon;
            HotKey = new HotKey(shortcut.HotKey);
        }

        public FavoriteShortcut(string name, string pathToExetubaleFile)
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