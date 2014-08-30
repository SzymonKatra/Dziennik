using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Dziennik.CommandUtils;
using System.ComponentModel;
using Dziennik.ViewModel;

namespace Dziennik.View
{
    public sealed class EditScheduleViewModel : ObservableObject, IDataErrorInfo
    {
        public enum EditScheduleResult
        {
            Ok,
            Cancel,
        }

        public EditScheduleViewModel(WeekScheduleViewModel schedule, DateTime minValidFrom, DateTime maxValidFrom, bool isAddingMode = false)
        {
            m_okCommand = new RelayCommand(Ok, CanOk);
            m_cancelCommand = new RelayCommand(Cancel);

            m_schedule = schedule;
            m_isAddingMode = isAddingMode;
            m_minValidFrom = minValidFrom;
            m_maxValidFrom = maxValidFrom;

            m_validFrom = (isAddingMode ? DateTime.Now.Date : schedule.StartDate);
        }

        private EditScheduleResult m_result = EditScheduleResult.Cancel;
        public EditScheduleResult Result
        {
            get { return m_result; }
        }
        private bool m_isAddingMode;
        public bool IsAddingMode
        {
            get { return m_isAddingMode; }
        }
        private DateTime m_minValidFrom;
        private DateTime m_maxValidFrom;

        private WeekScheduleViewModel m_schedule;
        public WeekScheduleViewModel Schedule
        {
            get { return m_schedule; }
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

        private bool m_validFromValid = false;
        private DateTime m_validFrom = DateTime.Now.Date;
        public DateTime ValidFrom
        {
            get { return m_validFrom; }
            set { m_validFrom = value; RaisePropertyChanged("ValidFrom"); }
        }

        private void Ok(object param)
        {
            m_schedule.StartDate = m_validFrom.Date;

            m_result = EditScheduleResult.Ok;
            GlobalConfig.Dialogs.Close(this);
        }
        private bool CanOk(object param)
        {
            return m_validFromValid;
        }
        private void Cancel(object param)
        {
            m_result = EditScheduleResult.Cancel;
            GlobalConfig.Dialogs.Close(this);
        }

        public string Error
        {
            get { return string.Empty; }
        }

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "ValidFrom": return ValidateValidFrom();
                }

                return string.Empty;
            }
        }

        private string ValidateValidFrom()
        {
            m_validFromValid = false;

            if (m_validFrom.Date <= m_minValidFrom.Date || m_validFrom.Date >= m_maxValidFrom)
            {
                m_okCommand.RaiseCanExecuteChanged();
                return GlobalConfig.GetStringResource("lang_InvalidDate");
            }

            m_validFromValid = true;
            m_okCommand.RaiseCanExecuteChanged();
            return string.Empty;
        }
    }
}
