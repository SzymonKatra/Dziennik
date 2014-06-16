using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Dziennik.CommandUtils
{
    public class RelayCommand<T> : ICommand
    {
        public RelayCommand(Action<T> execute)
            : this(execute, null)
        {
        }
        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            m_execute = execute;
            m_canExecute = canExecute;
        }

        private Action<T> m_execute;
        private Predicate<T> m_canExecute;
        
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (m_execute == null) return true;
            if (parameter == null && typeof(T).IsValueType)
            {
                return m_canExecute(default(T));
            }
            else
            {
                return m_canExecute((T)parameter);
            }
        }
        public void Execute(object parameter)
        {
            if (m_execute == null) return;
            if (parameter == null && typeof(T).IsValueType)
            {
                m_execute(default(T));
            }
            else
            {
                m_execute((T)parameter);
            }
        }
        public void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged(EventArgs.Empty);
        }

        protected virtual void OnCanExecuteChanged(EventArgs e)
        {
            EventHandler handler = CanExecuteChanged;
            if (handler != null) handler(this, e);
        }
    }
}
