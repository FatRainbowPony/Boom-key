using System;
using System.Collections.Generic;
using System.Linq;

namespace BoomKey.Models
{
    public class HotKey
    {
        #region Properties

        #region Public
        public string Name { get; set; }

        public Combination Combination { get; set; }
        #endregion Public

        #endregion Properties

        #region Constructors

        #region Public
        public HotKey()
        {
            Name = GenerateUniqueName();
            Combination = new Combination();
        }

        public HotKey(HotKey hotKey)
        {
            hotKey ??= new HotKey();

            Name = hotKey.Name;
            Combination = new Combination(hotKey.Combination);
        }

        public HotKey(string name, Combination combination)
        {
            combination ??= new Combination();
            
            Name = name;
            Combination = combination;
        }
        #endregion Public

        #endregion Constructors

        #region Methods

        #region Private
        private static string GenerateUniqueName()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }
        #endregion Private

        #region Public
        public static bool ExistsWithSameCombination(Combination combination, List<Shortcut> shortcuts)
        {
            int countExistingShortcuts = (from shortcut in shortcuts
                                          where shortcut.HotKey != null && shortcut.HotKey.Combination.Key == combination.Key && shortcut.HotKey.Combination.MultModifierKey == combination.MultModifierKey
                                          select shortcut).Count();
            if (countExistingShortcuts > 0)
            {
                return true;
            }

            return false;
        }
        #endregion Public

        #endregion Methods
    }
}