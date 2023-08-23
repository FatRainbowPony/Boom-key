using System;
using System.Windows.Input;

namespace AppDevTools.Templates.MVVM.ViewModel.Commands
{
    public class CommandsVM : ICommand
    {
        #region Fields

        #region Private
        private Action<object?> execute;
        private Func<object?, bool>? canExecute;
        #endregion

        #endregion Fields

        #region Constructors

        #region Public
        public CommandsVM(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }
        #endregion Public

        #endregion Constructors

        #region Methods

        #region Public
        public bool CanExecute(object? parameter)
        {
            return canExecute == null || canExecute(parameter);
        }

        public void Execute(object? parameter)
        {
            execute(parameter);
        }
        #endregion Public

        #endregion Methods

        #region Events

        #region Public
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        #endregion Public

        #endregion Events
    }
}