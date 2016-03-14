using System;
using System.Windows.Input;

namespace SSD.MakeImagesForStore
{
    class CommandBrowseTarget : ICommand
    {
        public Action<object> ExecuteDelegate { get; set; }

        public event EventHandler CanExecuteChanged;

        public CommandBrowseTarget(Action<object> executeMethod)
        {
            ExecuteDelegate = executeMethod;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (ExecuteDelegate != null)
            {
                ExecuteDelegate(parameter);
            }
        }

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }
    }
}
