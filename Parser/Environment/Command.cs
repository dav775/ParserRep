using System;
using System.Windows.Input;

namespace Parser
{
    public class Command : ICommand
    {
        //Поле хранит ссылку на обект реализующий логику работы команды
        private Action<object> execute;
        //Поле хранит информацию о состоянии команды (true - включена, false - отключена)
        private Func<object, bool> canExecute;

        //Событие вызываемое при изменении состояния элемента управления
        public event EventHandler CanExecuteChanged
        {
            //Акцессоры используемые для управления добавления и удаления обработчиков события
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        //Конструктор класса
        public Command(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        //Метод определяет может ли команда выполняться
        public bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute(parameter);
        }
        //Метод реализующий логику работы команды
        public void Execute(object parameter)
        {
            execute(parameter);
        }
    }


    //public class RelayCommand : ICommand
    //{
    //    Action _targetExecuteMethod;
    //    Func<bool> _targetCanExecuteMethod;

    //    public RelayCommand(Action executeMethod)
    //    {
    //        _targetExecuteMethod = executeMethod;
    //    }

    //    public RelayCommand(Action executeMethod, Func<bool> canExecuteMethod)
    //    {
    //        _targetExecuteMethod = executeMethod;
    //        _targetCanExecuteMethod = canExecuteMethod;
    //    }

    //    public void RaiseCanExecuteChanged()
    //    {
    //        CanExecuteChanged(this, EventArgs.Empty);
    //    }
    //    #region ICommand Members

    //    bool ICommand.CanExecute(object parameter)
    //    {
    //        if (_targetCanExecuteMethod != null)
    //        {
    //            return _targetCanExecuteMethod();
    //        }
    //        if (_targetExecuteMethod != null)
    //        {
    //            return true;
    //        }
    //        return false;
    //    }

    //    // Beware - should use weak references if command instance lifetime is longer than lifetime of UI objects that get hooked up to command
    //    // Prism commands solve this in their implementation
    //    public event EventHandler CanExecuteChanged = delegate { };

    //    void ICommand.Execute(object parameter)
    //    {
    //        if (_targetExecuteMethod != null)
    //        {
    //            _targetExecuteMethod();
    //        }
    //    }
    //    #endregion
    //}

    //public class RelayCommand<T> : ICommand
    //{
    //    Action<T> _targetExecuteMethod;
    //    Func<T, bool> _targetCanExecuteMethod;

    //    public RelayCommand(Action<T> executeMethod)
    //    {
    //        _targetExecuteMethod = executeMethod;
    //    }

    //    public RelayCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod)
    //    {
    //        _targetExecuteMethod = executeMethod;
    //        _targetCanExecuteMethod = canExecuteMethod;
    //    }

    //    public void RaiseCanExecuteChanged()
    //    {
    //        CanExecuteChanged(this, EventArgs.Empty);
    //    }
    //    #region ICommand Members

    //    bool ICommand.CanExecute(object parameter)
    //    {
    //        if (_targetCanExecuteMethod != null)
    //        {
    //            T tparm = (T)parameter;
    //            return _targetCanExecuteMethod(tparm);
    //        }
    //        if (_targetExecuteMethod != null)
    //        {
    //            return true;
    //        }
    //        return false;
    //    }

    //    // Beware - should use weak references if command instance lifetime is longer than lifetime of UI objects that get hooked up to command
    //    // Prism commands solve this in their implementation
    //    public event EventHandler CanExecuteChanged = delegate { };

    //    void ICommand.Execute(object parameter)
    //    {
    //        if (_targetExecuteMethod != null)
    //        {
    //            _targetExecuteMethod((T)parameter);
    //        }
    //    }
    //    #endregion
    //}
}
