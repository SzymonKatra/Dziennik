using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Dziennik.CommandUtils;
using System.Windows.Input;
using Dziennik.ViewModel;

namespace Dziennik.View
{
    public sealed class SchedulesListViewModel : ObservableObject
    {
        public SchedulesListViewModel(ObservableCollection<WeekScheduleViewModel> schedules, IEnumerable<CalendarViewModel> calendars, ObservableCollection<SchoolClassControlViewModel> classes, ICommand autoSaveCommand)
        {
            m_addScheduleCommand = new RelayCommand(AddSchedule);
            m_editScheduleCommand = new RelayCommand<WeekScheduleViewModel>(EditSchedule);
            m_closeCommand = new RelayCommand(Close);

            m_schedules = schedules;
            m_calendars = calendars;
            m_classes = classes;
            m_autoSaveCommand = autoSaveCommand;

            foreach (var item in m_calendars)
            {
                if (item.YearBeginning < m_minDate) m_minDate = item.YearBeginning;
                if (item.YearEnding > m_maxDate) m_maxDate = item.YearEnding;
            }
        }

        private ICommand m_autoSaveCommand;

        private DateTime m_minDate = DateTime.MaxValue;
        private DateTime m_maxDate = DateTime.MinValue;
        private IEnumerable<CalendarViewModel> m_calendars;
        private ObservableCollection<SchoolClassControlViewModel> m_classes;

        private ObservableCollection<WeekScheduleViewModel> m_schedules;
        public ObservableCollection<WeekScheduleViewModel> Schedules
        {
            get { return m_schedules; }
        }

        private RelayCommand m_addScheduleCommand;
        public ICommand AddScheduleCommand
        {
            get { return m_addScheduleCommand; }
        }
        private RelayCommand<WeekScheduleViewModel> m_editScheduleCommand;
        public ICommand EditScheduleCommand
        {
            get { return m_editScheduleCommand; }
        }
        private RelayCommand m_closeCommand;
        public ICommand CloseCommand
        {
            get { return m_closeCommand; }
        }

        private void AddSchedule(object param)
        {
            WeekScheduleViewModel schedule = new WeekScheduleViewModel();
            schedule.Monday.PadHours(GlobalConfig.GlobalDatabase.ViewModel.Hours.Hours.Count);
            schedule.Tuesday.PadHours(GlobalConfig.GlobalDatabase.ViewModel.Hours.Hours.Count);
            schedule.Wednesday.PadHours(GlobalConfig.GlobalDatabase.ViewModel.Hours.Hours.Count);
            schedule.Thursday.PadHours(GlobalConfig.GlobalDatabase.ViewModel.Hours.Hours.Count);
            schedule.Friday.PadHours(GlobalConfig.GlobalDatabase.ViewModel.Hours.Hours.Count);
            DateTime? validFromOverride = null;
            if (m_schedules.Count <= 0) validFromOverride = m_minDate;
            EditGlobalScheduleViewModel dialogViewModel = new EditGlobalScheduleViewModel(m_classes, schedule, GetMinValidFrom(schedule), GetMaxValidFrom(schedule), validFromOverride);
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
            if (dialogViewModel.Result == EditGlobalScheduleViewModel.EditGlobalScheduleResult.Ok)
            {
                m_schedules.Add(schedule);
            }
            if (dialogViewModel.Result != EditGlobalScheduleViewModel.EditGlobalScheduleResult.Cancel) CompleteEdit();
        }
        private void EditSchedule(WeekScheduleViewModel param)
        {
            param.PushCopy();
            EditGlobalScheduleViewModel dialogViewModel = new EditGlobalScheduleViewModel(m_classes, param, GetMinValidFrom(param), GetMaxValidFrom(param), null);
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
            if (dialogViewModel.Result == EditGlobalScheduleViewModel.EditGlobalScheduleResult.Cancel)
            {
                param.PopCopy(WorkingCopyResult.Cancel);
            }
            else if (dialogViewModel.Result == EditGlobalScheduleViewModel.EditGlobalScheduleResult.Ok)
            {
                param.PopCopy(WorkingCopyResult.Ok);
            }

            if (dialogViewModel.Result != EditGlobalScheduleViewModel.EditGlobalScheduleResult.Cancel) CompleteEdit();
        }
        private void Close(object param)
        {
            GlobalConfig.Dialogs.Close(this);
        }

        private void CompleteEdit()
        {
            m_autoSaveCommand.Execute(null);
            foreach (var item in m_classes)
            {
                foreach (var grp in item.ViewModel.Groups)
                {
                    grp.RaiseRemainingHoursOfLessonsChanged();
                }
            }
        }

        private DateTime GetMinValidFrom(WeekScheduleViewModel schedule)
        {
            if (schedule == null)
            {
                return (m_schedules.Count > 0 ? m_schedules[m_schedules.Count - 1].StartDate : m_minDate.AddDays(-1.0));
            }
            else
            {
                int index = m_schedules.IndexOf(schedule);
                if (index > 0)
                {
                    return m_schedules[index - 1].StartDate;
                }
                else
                {
                    return m_minDate.AddDays(-1.0);
                }
            }

            //int index = -1;
            //if (m_schedules.Count > 0)
            //{
            //    index = m_schedules.Count;
            //}
            //else
            //{
            //    index = (schedule == null ? -1 : m_schedules.IndexOf(schedule));
            //}
            //if (index > 0)
            //{
            //    return m_schedules[index - 1].StartDate;
            //}
            //else
            //{
            //    return m_calendar.YearBeginning.AddDays(-1.0);
            //}
        }
        private DateTime GetMaxValidFrom(WeekScheduleViewModel schedule)
        {
            if (schedule == null)
            {
                return m_maxDate.AddDays(1.0);
            }
            else
            {
                int index = m_schedules.IndexOf(schedule);
                if (index < m_schedules.Count - 1)
                {
                    return m_schedules[index + 1].StartDate;
                }
                else return m_maxDate.AddDays(1.0);
            }

            //int index = (schedule == null ? -1 : m_schedules.IndexOf(schedule));
            //if (index >= 0 && index < m_schedules.Count - 1)
            //{
            //    return m_schedules[index + 1].StartDate;
            //}
            //else
            //{
            //    return m_calendar.YearEnding.AddDays(1.0);
            //}
        }
    }
}
