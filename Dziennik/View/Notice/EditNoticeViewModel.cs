using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Dziennik.CommandUtils;
using System.Windows.Input;
using Dziennik.ViewModel;

namespace Dziennik.View
{
    public sealed class EditNoticeViewModel : ObservableObject, IDataErrorInfo
    {
        public enum EditNoticeResult
        {
            Ok,
            Cancel,
            RemoveNotice,
        }

        public EditNoticeViewModel(NoticeViewModel notice, bool isAddingMode = false)
        {
            m_okCommand = new RelayCommand(Ok, CanOk);
            m_cancelCommand = new RelayCommand(Cancel);
            m_removeNoticeCommand = new RelayCommand(RemoveNotice, CanRemoveNotice);

            m_notice = notice;
            m_isAddingMode = isAddingMode;

            if(isAddingMode)
            {
                m_notice.Date = DateTime.Now.Date;
                m_notice.NotifyIn = new TimeSpan(1, 0, 0, 0);
            }

            m_notifyInInput = m_notice.NotifyIn.Days.ToString();
        }

        private EditNoticeResult m_result = EditNoticeResult.Cancel;
        public EditNoticeResult Result
        {
            get { return m_result; }
        }

        private NoticeViewModel m_notice;
        private bool m_isAddingMode;

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

        private RelayCommand m_removeNoticeCommand;
        public ICommand RemoveNoticeCommand
        {
            get { return m_removeNoticeCommand; }
        }


        private bool m_notifyInInputValid = false;
        private string m_notifyInInput;
        public string NotifyInInput
        {
            get { return m_notifyInInput; }
            set { m_notifyInInput = value; RaisePropertyChanged("NotifyInput"); }
        }

        private void Ok(object e)
        {
            m_result = EditNoticeResult.Ok;
            GlobalConfig.Dialogs.Close(this);
        }
        private bool CanOk(object e)
        {
            return m_notifyInInputValid;
        }
        private void Cancel(object e)
        {
            m_result = EditNoticeResult.Cancel;
            GlobalConfig.Dialogs.Close(this);
        }
        private void RemoveNotice(object e)
        {
            m_result = EditNoticeResult.RemoveNotice;
            GlobalConfig.Dialogs.Close(this);
        }
        private bool CanRemoveNotice(object e)
        {
            return !m_isAddingMode;
        }

        public string Error
        {
            get { return string.Empty; }
        }
        public string this[string columnName]
        {
            get
            {
                switch(columnName)
                {
                    case "NotifyInInput": return ValidateNotifyInInput();
                }

                return string.Empty;
            }
        }

        public string ValidateNotifyInInput()
        {
            m_notifyInInputValid = false;

            int result;
            if (!int.TryParse(m_notifyInInput, out result) || result < 0)
            {
                m_okCommand.RaiseCanExecuteChanged();
                return GlobalConfig.GetStringResource("lang_TypeValidNonNegativeInteger");
            }

            m_notice.NotifyIn = new TimeSpan(result, 0, 0, 0);

            m_notifyInInputValid = true;
            m_okCommand.RaiseCanExecuteChanged();
            return string.Empty;
        }
    }
}
