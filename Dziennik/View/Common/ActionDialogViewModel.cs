using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Dziennik.CommandUtils;
using System.Diagnostics;

namespace Dziennik.View
{
    public class ActionDialogViewModel : ObservableObject
    {
        public ActionDialogViewModel(Action<ActionDialogViewModel, object> workAction, object parameter, string content)
            : this(workAction, parameter, content, content)
        {
        }
        public ActionDialogViewModel(Action<ActionDialogViewModel, object> workAction, object parameter, string content, string title)
        {
            m_doWorkCommand = new RelayCommand(DoWork, CanDoWork);
            
            m_workAction = workAction;
            m_parameter = parameter;
            m_content = content;
            m_title = title;
        }

        private RelayCommand m_doWorkCommand;
        public ICommand DoWorkCommand
        {
            get { return m_doWorkCommand; }
        }

        private bool m_executed = false;

        private Action<ActionDialogViewModel, object> m_workAction;

        private object m_parameter;

        private string m_content;
        public string Content
        {
            get { return m_content; }
            set { m_content = value; RaisePropertyChanged("Content"); }
        }

        private string m_title;
        public string Title
        {
            get { return m_title; }
            set { m_title = value; RaisePropertyChanged("Title"); }
        }

        private void DoWork(object e)
        {
            m_executed = true;
            if (m_workAction != null)
            {
                m_workAction(this, m_parameter);
            }
            GlobalConfig.Dialogs.Close(this);
        }
        private bool CanDoWork(object e)
        {
            return !m_executed;
        }
    }
}
