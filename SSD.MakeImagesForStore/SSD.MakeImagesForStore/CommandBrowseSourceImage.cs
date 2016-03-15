using System;
using System.Windows.Input;

namespace SSD.MakeImagesForStore
{
    class CommandBrowseSourceImage : ICommand
    {
        public Action<object> ExecuteDelegate { get; set; }

        public event EventHandler CanExecuteChanged;

        public CommandBrowseSourceImage(Action<object> executeMethod)
        {
            ExecuteDelegate = executeMethod;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            ExecuteDelegate?.Invoke(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
