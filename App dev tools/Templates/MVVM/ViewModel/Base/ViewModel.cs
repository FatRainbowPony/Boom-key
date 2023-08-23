using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AppDevTools.Templates.MVVM.ViewModel.Base
{
    public abstract class ViewModel : INotifyPropertyChanged
    {
        #region Methods

        #region Protected
        protected virtual void OnPropertyChanged([CallerMemberName] string? PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        protected virtual bool Set<T>(ref T field, T value, [CallerMemberName] string? PropertyName = null)
        {
            if (Equals(field, value))
            { 
                return false; 
            }
            
            field = value;
            OnPropertyChanged(PropertyName);

            return true;
        }
        #endregion Protected

        #endregion Methods

        #region Events

        #region Public
        public event PropertyChangedEventHandler? PropertyChanged;
        #endregion Public

        #endregion Events
    }
}