using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Dziennik.CommandUtils;
using System.Diagnostics;
using System.Windows;

namespace Dziennik.View
{
    public class ActionDialogViewModel : ObservableObject
    {
        public ActionDialogViewModel(Action<ActionDialogViewModel, object> workAction, object parameter, string content, bool progressVisible = false)
            : this(workAction, parameter, content, content, null, progressVisible)
        {
        }
        public ActionDialogViewModel(Action<ActionDialogViewModel, object> workAction, object parameter, string content, string title, bool progressVisible = false)
            : this(workAction, parameter, content, title, null, progressVisible)
        {
        }
        public ActionDialogViewModel(Action<ActionDialogViewModel, object> workAction, object parameter, string content, string title, Size? size, bool progressVisible = false)
        {
            m_doWorkCommand = new RelayCommand(DoWork, CanDoWork);

            m_workAction = workAction;
            m_parameter = parameter;
            m_content = content;
            m_title = title;
            m_progressVisible = progressVisible;
            m_size = size;
        }

        private RelayCommand m_doWorkCommand;
        public ICommand DoWorkCommand
        {
            get { return m_doWorkCommand; }
        }

        private bool m_executed = false;

        private Action<ActionDialogViewModel, object> m_workAction;

        private object m_parameter;

        private Size? m_size;
        public Size? Size
        {
            get { return m_size; }
        }

        private string m_content;
        public string Content
        {
            get { return m_content; }
            set { m_content = value; RaisePropertyChanged("Content"); }
        }

        private int m_progressValue = 0;
        public int ProgressValue
        {
            get { return m_progressValue; }
            set { m_progressValue = value; RaisePropertyChanged("ProgressValue"); }
        }

        private bool m_progressVisible = false;
        public bool ProgressVisible
        {
            get { return m_progressVisible; }
            set { m_progressVisible = value; RaisePropertyChanged("ProgressVisible"); }
        }

        private double m_currentProgress = -1.0;
        private double m_progressStep = 0.0;
        public double ProgressStep
        {
            get { return m_progressStep; }
            set { m_progressStep = value; RaisePropertyChanged("ProgressStep"); }
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

        public void StepProgress()
        {
            if (m_currentProgress < 0.0) m_currentProgress = m_progressValue;
            m_currentProgress += m_progressStep;
            ProgressValue = (int)Math.Round(m_currentProgress, MidpointRounding.AwayFromZero);
        }
    }
}
