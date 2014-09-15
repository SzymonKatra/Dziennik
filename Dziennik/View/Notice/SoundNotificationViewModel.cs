using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Dziennik.CommandUtils;
using Microsoft.Win32;

namespace Dziennik.View
{
    public sealed class SoundNotificationViewModel : ObservableObject
    {
        public SoundNotificationViewModel()
        {
            m_okCommand = new RelayCommand(Ok);
            m_cancelCommand = new RelayCommand(Cancel);

            m_endLessonNotifyMinutes = GlobalConfig.Notifier.EndLessonNotifyMinutes;
            m_endLessonNotify = (m_endLessonNotifyMinutes > 0);
            m_endLessonNotifyPath = GlobalConfig.Notifier.EndLessonNotifyPath;
            m_endBreakNotify = GlobalConfig.Notifier.EndBreakNotify;
            m_endLessonNotifyPath = GlobalConfig.Notifier.EndBreakNotifyPath;
        }

        private RelayCommand m_okCommand;
        public ICommand OkCommand
        {
            get { return m_okCommand; }
        }

        private RelayCommand m_cancelCommand;
        public ICommand CancelCommand
        {
            get { return m_cancelCommand; }
        }

        private bool m_endLessonNotify;
        public bool EndLessonNotify
        {
            get { return m_endLessonNotify; }
            set { m_endLessonNotify = value; RaisePropertyChanged("EndLessonNotify"); }
        }

        private int m_endLessonNotifyMinutes;
        public int EndLessonNotifyMinutes
        {
            get { return m_endLessonNotifyMinutes; }
            set { m_endLessonNotifyMinutes = value; RaisePropertyChanged("EndLessonNotifyMinutes"); }
        }

        private string m_endLessonNotifyPath;
        public string EndLessonNotifyPath
        {
            get { return m_endLessonNotifyPath; }
            set { m_endLessonNotifyPath = value; RaisePropertyChanged("EndLessonNotifyPath"); }
        }

        private bool m_endBreakNotify;
        public bool EndBreakNotify
        {
            get { return m_endBreakNotify; }
            set { m_endBreakNotify = value; RaisePropertyChanged("EndBreakNotify"); }
        }

        private string m_endBreakNotifyPath;
        public string EndBreakNotifyPath
        {
            get { return m_endBreakNotifyPath; }
            set { m_endBreakNotifyPath = value; RaisePropertyChanged("EndBreakNotifyPath"); }
        }

        private void Ok(object param)
        {
            GlobalConfig.Notifier.EndLessonNotifyMinutes = (m_endLessonNotify ? m_endLessonNotifyMinutes : 0);
            GlobalConfig.Notifier.EndLessonNotifyPath = m_endLessonNotifyPath;
            GlobalConfig.Notifier.EndBreakNotify = m_endBreakNotify;
            GlobalConfig.Notifier.EndBreakNotifyPath = m_endBreakNotifyPath;

            GlobalConfig.Dialogs.Close(this);
        }
        private void Cancel(object param)
        {
            GlobalConfig.Dialogs.Close(this);
        }
    }
}
