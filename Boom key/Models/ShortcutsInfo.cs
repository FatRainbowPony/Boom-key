using System.Collections.Generic;

namespace BoomKey.Models
{
    public class ShortcutsInfo
    {
        #region Properties

        #region Public 
        public List<FavoriteShortcut>? FavoriteShortcuts { get; set; }

        public List<Section>? ShortcutSections { get; set; }
        #endregion Public

        #endregion Properties

        #region Constructors

        #region Public
        public ShortcutsInfo()
        {

        }

        public ShortcutsInfo(List<FavoriteShortcut> favoriteShortcuts, List<Section> shortcutSections)
        {
            favoriteShortcuts ??= new List<FavoriteShortcut>();
            shortcutSections ??= new List<Section>();

            FavoriteShortcuts = favoriteShortcuts;
            ShortcutSections = shortcutSections;
        }
        #endregion Public

        #endregion Constructors
    }
}