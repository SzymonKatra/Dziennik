using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Dziennik.ViewModel;
using Dziennik.CommandUtils;
using System.Windows.Input;

namespace Dziennik.View
{
    public class EditLessonsHoursViewModel
    {
        public enum EditLessonsHoursResult
        {
            Ok,
            Cancel,
        }

        public EditLessonsHoursViewModel(LessonsHoursViewModel hours)
        {
            m_okCommand = new RelayCommand(Ok);
            m_cancelCommand = new RelayCommand(Cancel);
            m_addNextHourCommand = new RelayCommand(AddNextHour, CanAddNextHour);

            m_hours = hours;
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

        private LessonsHoursViewModel m_hours;
        public LessonsHoursViewModel Hours
        {
            get { return m_hours; }
        }

        private void Ok(object param)
        {
            m_result = EditLessonsHoursResult.Ok;
            GlobalConfig.Dialogs.Close(this);
        }
        private void Cancel(object param)
        {
            m_result = EditLessonsHoursResult.Cancel;
            GlobalConfig.Dialogs.Close(this);
        }
        private void AddNextHour(object param)
        {
            m_hours.Hours.Add(new LessonHourViewModel() { Number = (m_hours.Hours.Count > 0 ? m_hours.Hours[m_hours.Hours.Count - 1].Number + 1 : 1) });
            m_addNextHourCommand.RaiseCanExecuteChanged();
        }
        private bool CanAddNextHour(object param)
        {
            return m_hours.Hours.Count < GlobalConfig.MaxLessonHour;
        }
    }
}
