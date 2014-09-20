using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Dziennik.CommandUtils;
using Microsoft.Win32;
using System.IO;

namespace Dziennik.View
{
    public sealed class SoundNotificationViewModel : ObservableObject
    {
        public SoundNotificationViewModel()
        {
            m_okCommand = new RelayCommand(Ok);
            m_cancelCommand = new RelayCommand(Cancel);
            m_selectEndLessonNotifyPathCommand = new RelayCommand(SelectEndLessonNotifyPath);
            m_selectEndBreakNotifyPathCommand = new RelayCommand(SelectEndBreakNotifyPath);

            m_endLessonNotifySeconds = GlobalConfig.Notifier.EndLessonNotifySeconds;
            m_endLessonNotify = (m_endLessonNotifySeconds >= 0);
            m_endLessonNotifyPath = GlobalConfig.Notifier.EndLessonNotifyPath;
            m_endBreakNotifySeconds = GlobalConfig.Notifier.EndBreakNotifySeconds;
            m_endBreakNotify = (m_endBreakNotifySeconds >= 0);
            m_endBreakNotifyPath = GlobalConfig.Notifier.EndBreakNotifyPath;
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

        private RelayCommand m_selectEndLessonNotifyPathCommand;
        public ICommand SelectEndLessonNotifyPathCommand
        {
            get { return m_selectEndLessonNotifyPathCommand; }
        }

        private RelayCommand m_selectEndBreakNotifyPathCommand;
        public ICommand SelectEndBreakNotifyPathCommand
        {
            get { return m_selectEndBreakNotifyPathCommand; }
        }

        private bool m_endLessonNotify;
        public bool EndLessonNotify
        {
            get { return m_endLessonNotify; }
            set { m_endLessonNotify = value; RaisePropertyChanged("EndLessonNotify"); EndLessonNotifySeconds = 0; }
        }

        private int m_endLessonNotifySeconds;
        public int EndLessonNotifySeconds
        {
            get { return m_endLessonNotifySeconds; }
            set { m_endLessonNotifySeconds = value; RaisePropertyChanged("EndLessonNotifySeconds"); }
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

        private int m_endBreakNotifySeconds;
        public int EndBreakNotifySeconds
        {
            get { return m_endBreakNotifySeconds; }
            set { m_endBreakNotifySeconds = value; RaisePropertyChanged("EndBreakNotifySeconds"); }
        }

        private string m_endBreakNotifyPath;
        public string EndBreakNotifyPath
        {
            get { return m_endBreakNotifyPath; }
            set { m_endBreakNotifyPath = value; RaisePropertyChanged("EndBreakNotifyPath"); }
        }

        private void Ok(object param)
        {
            GlobalConfig.Notifier.EndLessonNotifySeconds = (m_endLessonNotify ? m_endLessonNotifySeconds : -1);
            GlobalConfig.Notifier.EndLessonNotifyPath = m_endLessonNotifyPath;
            GlobalConfig.Notifier.EndBreakNotifySeconds = (m_endBreakNotify ? m_endBreakNotifySeconds : -1);
            GlobalConfig.Notifier.EndBreakNotifyPath = m_endBreakNotifyPath;

            GlobalConfig.Dialogs.Close(this);
        }
        private void Cancel(object param)
        {
            GlobalConfig.Dialogs.Close(this);
        }
        private void SelectEndLessonNotifyPath(object param)
        {
            string result = OpenAudio();
            if(!string.IsNullOrWhiteSpace(result))
            {
                EndLessonNotifyPath = result;
            }
        }
        private void SelectEndBreakNotifyPath(object param)
        {
            string result = OpenAudio();
            if (!string.IsNullOrWhiteSpace(result))
            {
                EndBreakNotifyPath = result;
            }
        }

        private string OpenAudio()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Filter = "Pliki audio|*.wav";
            ofd.ShowDialog();

            if(File.Exists(ofd.FileName))
            {
                return ofd.FileName;
            }
            else return string.Empty;
        }
    }
}
