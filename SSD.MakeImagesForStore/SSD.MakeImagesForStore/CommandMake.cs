﻿using System;
using System.Windows.Input;

namespace SSD.MakeImagesForStore
{
    class CommandMake : ICommand
    {
        public Predicate<object> CanExecuteDelegate { get; set; } 
        public Action<object> ExecuteDelegate { get; set; }

        public event EventHandler CanExecuteChanged;

        public CommandMake(Action<object> executeMethod, Predicate<object> canExecuteMethod)
        {
            ExecuteDelegate = executeMethod;
            CanExecuteDelegate = canExecuteMethod;
        }

        public bool CanExecute(object parameter)
        {
            if (CanExecuteDelegate != null)
            {
                return CanExecuteDelegate(parameter);
            }

            return false;
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
