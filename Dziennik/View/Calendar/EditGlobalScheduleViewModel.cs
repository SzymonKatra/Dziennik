using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Dziennik.ViewModel;
using Dziennik.CommandUtils;
using System.Windows.Input;
using System.ComponentModel;

namespace Dziennik.View
{
    public sealed class EditGlobalScheduleViewModel : ObservableObject, IDataErrorInfo
    {
        public enum EditGlobalScheduleResult
        {
            Ok,
            Cancel,
        }

        public class SchoolDayItem : ObservableObject
        {
            private string m_title;
            public string Title
            {
                get { return m_title; }
                set { m_title = value; RaisePropertyChanged("Title"); }
            }

            private DayScheduleViewModel m_schedule;
            public DayScheduleViewModel Schedule
            {
                get { return m_schedule; }
                set { m_schedule = value; RaisePropertyChanged("Schedule"); }
            }

            private DayOfWeek m_dayOfWeek;
            public DayOfWeek DayOfWeek
            {
                get { return m_dayOfWeek; }
                set { m_dayOfWeek = value; RaisePropertyChanged("DayOfWeek"); }
            }
        }

        public EditGlobalScheduleViewModel(ObservableCollection<SchoolClassControlViewModel> classes, WeekScheduleViewModel schedule, DateTime minDate, DateTime maxDate, DateTime? validFromOverride)
        {
            m_okCommand = new RelayCommand(Ok, CanOk);
            m_cancelCommand = new RelayCommand(Cancel);

            m_schedule = schedule;
            m_classes = new ObservableCollection<SchoolClassViewModel>();
            foreach (var item in classes) m_classes.Add(item.Database.ViewModel);
            m_minDate = minDate;
            m_maxDate = maxDate;
            m_validFrom = (validFromOverride == null ? schedule.StartDate : (DateTime)validFromOverride);

            m_days = new ObservableCollection<SchoolDayItem>();
            m_days.Add(InitializeDay(DayOfWeek.Monday, m_schedule.Monday));
            m_days.Add(InitializeDay(DayOfWeek.Tuesday, m_schedule.Tuesday));
            m_days.Add(InitializeDay(DayOfWeek.Wednesday, m_schedule.Wednesday));
            m_days.Add(InitializeDay(DayOfWeek.Thursday, m_schedule.Thursday));
            m_days.Add(InitializeDay(DayOfWeek.Friday, m_schedule.Friday));
        }

        private SchoolDayItem InitializeDay(DayOfWeek dayOfWeek, DayScheduleViewModel day)
        {
            SchoolDayItem result = new SchoolDayItem();
            result.Schedule = day;
            result.DayOfWeek = dayOfWeek;
            switch(dayOfWeek)
            {
                case DayOfWeek.Monday: result.Title = GlobalConfig.GetStringResource("lang_MondayShort"); break;
                case DayOfWeek.Tuesday: result.Title = GlobalConfig.GetStringResource("lang_TuesdayShort"); break;
                case DayOfWeek.Wednesday: result.Title = GlobalConfig.GetStringResource("lang_WednesdayShort"); break;
                case DayOfWeek.Thursday: result.Title = GlobalConfig.GetStringResource("lang_ThursdayShort"); break;
                case DayOfWeek.Friday: result.Title = GlobalConfig.GetStringResource("lang_FridayShort"); break;
            }

            return result;
        }

        private WeekScheduleViewModel m_schedule;

        private EditGlobalScheduleResult m_result = EditGlobalScheduleResult.Cancel;
        public EditGlobalScheduleResult Result
        {
            get { return m_result; }
        }

        private ObservableCollection<SchoolClassViewModel> m_classes;
        public ObservableCollection<SchoolClassViewModel> Classes
        {
            get { return m_classes; }
        }

        private bool m_validFromValid = false;
        private DateTime m_validFrom;
        public DateTime ValidFrom
        {
            get { return m_validFrom; }
            set { m_validFrom = value; RaisePropertyChanged("ValidFrom"); }
        }

        private ObservableCollection<SchoolDayItem> m_days;
        public ObservableCollection<SchoolDayItem> Days
        {
            get { return m_days; }
        }

        private DateTime m_minDate;
        private DateTime m_maxDate;

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

        private void Ok(object param)
        {
            m_schedule.StartDate = m_validFrom;

            m_result = EditGlobalScheduleResult.Ok;
            GlobalConfig.Dialogs.Close(this);
        }
        private bool CanOk(object param)
        {
            return m_validFromValid;
        }
        private void Cancel(object param)
        {
            m_result = EditGlobalScheduleResult.Cancel;
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
                switch(columnName)
                {
                    case "ValidFrom": return ValidateValidFrom();
                }

                return string.Empty;
            }
        }
        private string ValidateValidFrom()
        {
            m_validFromValid = false;

            if (m_validFrom <= m_minDate || m_validFrom >= m_maxDate)
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