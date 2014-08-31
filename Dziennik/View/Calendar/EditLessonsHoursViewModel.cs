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
    public class EditLessonsHoursViewModel : ObservableObject
    {
        public enum EditLessonsHoursResult
        {
            Ok,
            Cancel,
        }

        public class HourValidator : ObservableObject, IDataErrorInfo
        {
            public HourValidator(IList<HourValidator> owner)
            {
                m_owner = owner;
            }

            private IList<HourValidator> m_owner;

            private int m_number;
            public int Number
            {
                get { return m_number; }
                set { m_number = value; RaisePropertyChanged("Number"); }
            }

            private DateTime m_start;
            public DateTime Start
            {
                get { return m_start; }
                set { m_start = value; RaisePropertyChanged("Start"); }
            }

            private DateTime m_end;
            public DateTime End
            {
                get { return m_end; }
                set { m_end = value; RaisePropertyChanged("End"); }
            }

            private bool m_valid;
            public bool Valid
            {
                get { return m_valid; }
                private set { m_valid = value; RaisePropertyChanged("Valid"); }
            }

            public void Validate()
            {
                ValidateStart();
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
                        case "Start": return ValidateStart();
                    }

                    return string.Empty;
                }
            }
            private string ValidateStart()
            {
                int index = m_owner.IndexOf(this);

                if (index > 0)
                {
                    HourValidator previous = m_owner[index - 1];
                    if (this.Start.TimeOfDay < previous.End.TimeOfDay)
                    {
                        Valid = false;
                        return GlobalConfig.GetStringResource("lang_HourPreviousInvalid");
                    }
                }

                Valid = true;

                return string.Empty;
            }
        }

        public EditLessonsHoursViewModel(LessonsHoursViewModel hours)
        {
            m_okCommand = new RelayCommand(Ok, CanOk);
            m_cancelCommand = new RelayCommand(Cancel);
            m_addNextHourCommand = new RelayCommand(AddNextHour, CanAddNextHour);

            m_databaseHours = hours;
            m_hours = new ObservableCollection<HourValidator>();

            IsEnabled = m_databaseHours.IsEnabled;
            foreach (var item in hours.Hours)
            {
                HourValidator validator = new HourValidator(m_hours);
                validator.Number = item.Number;
                validator.Start = item.Start;
                validator.End = item.End;
                validator.PropertyChanged += validator_PropertyChanged;
                validator.Validate();
                m_hours.Add(validator);
            }
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
        private RelayCommand m_addNextHourCommand;
        public ICommand AddNextHourCommand
        {
            get { return m_addNextHourCommand; }
        }

        private EditLessonsHoursResult m_result = EditLessonsHoursResult.Cancel;
        public EditLessonsHoursResult Result
        {
            get { return m_result; }
        }

        private bool m_isEnabled;
        public bool IsEnabled
        {
            get { return m_isEnabled; }
            set { m_isEnabled = value; RaisePropertyChanged("IsEnabled"); m_okCommand.RaiseCanExecuteChanged(); }
        }

        private LessonsHoursViewModel m_databaseHours;
        public LessonsHoursViewModel DatabaseHours
        {
            get { return m_databaseHours; }
        }

        private ObservableCollection<HourValidator> m_hours;
        public ObservableCollection<HourValidator> Hours
        {
            get { return m_hours; }
        }

        private void Ok(object param)
        {
            m_databaseHours.IsEnabled = m_isEnabled;

            foreach (var item in m_hours)
            {
                LessonHourViewModel h = m_databaseHours.Hours.FirstOrDefault(x => x.Number == item.Number);
                if (h == null)
                {
                    h = new LessonHourViewModel();
                    h.Number = item.Number;
                    m_databaseHours.Hours.Add(h);
                }
                h.Start = item.Start;
                h.End = item.End;
            }

            m_result = EditLessonsHoursResult.Ok;
            GlobalConfig.Dialogs.Close(this);
        }
        private bool CanOk(object param)
        {
            if (!m_databaseHours.IsEnabled) return true;
            foreach (var item in m_hours)
            {
                if (!item.Valid) return false;
            }
            return true;
        }
        private void Cancel(object param)
        {
            m_result = EditLessonsHoursResult.Cancel;
            GlobalConfig.Dialogs.Close(this);
        }
        private void AddNextHour(object param)
        {
            HourValidator validator = new HourValidator(m_hours) { Number = (m_hours.Count > 0 ? m_hours[m_hours.Count - 1].Number + 1 : 1) };
            validator.PropertyChanged += validator_PropertyChanged;
            m_hours.Add(validator);
            m_addNextHourCommand.RaiseCanExecuteChanged();
        }
        private bool CanAddNextHour(object param)
        {
            return m_hours.Count < GlobalConfig.MaxLessonHour && m_databaseHours.IsEnabled;
        }

        private void validator_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Valid")
            {
                m_okCommand.RaiseCanExecuteChanged();
            }
        }
    }
}
