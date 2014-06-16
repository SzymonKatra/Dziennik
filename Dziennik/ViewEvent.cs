using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dziennik
{
    public class ViewEvent<T> : ObservableObject
    {
        public ViewEvent(Action<T> action)
            : this(action, null)
        {
        }
        public ViewEvent(Action<T> action, Predicate<T> canExecuteAction)
        {
            m_action = action;
            m_canExecuteAction = canExecuteAction;
        }

        private Action<T> m_action;
        private Predicate<T> m_canExecuteAction;

        private bool m_canExecute = true;
        public bool CanExecute
        {
            get { return m_canExecute; }
        }

        public void Execute(T parametr)
        {
            m_action(parametr);
        }
        public void CheckCanExecute(T parametr)
        {
            if (m_canExecuteAction == null) return;
            m_canExecute = m_canExecuteAction(parametr);
            OnPropertyChanged("CanExecute");
        }
    }
}
