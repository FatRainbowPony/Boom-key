using BoomKey.Addons;

namespace BoomKey.Models
{
    public class Combination
    {
        #region Properties

        #region Public
        public Key Key { get; set; }

        public MultModifierKey MultModifierKey { get; set; }
        #endregion Public

        #endregion Properties

        #region Constructors

        #region Public
        public Combination()
        {      
            Key = Key.A;
            MultModifierKey = MultModifierKey.None;
        }

        public Combination(Combination combination)
        {
            combination ??= new Combination();

            Key = combination.Key;
            MultModifierKey = combination.MultModifierKey;
        }

        public Combination(Key key, MultModifierKey multModifierKey)
        {
            Key = key;
            MultModifierKey = multModifierKey;
        }
        #endregion Public

        #endregion Constructors
    }
}