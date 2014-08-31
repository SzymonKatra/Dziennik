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
        public SchedulesListViewModel(ObservableCollection<WeekScheduleViewModel> schedules, CalendarViewModel calendar)
        {
            m_addScheduleCommand = new RelayCommand(AddSchedule);
            m_editScheduleCommand = new RelayCommand<WeekScheduleViewModel>(EditSchedule);
            m_closeCommand = new RelayCommand(Close);

            m_schedules = schedules;
            m_calendar = calendar;
        }

        private CalendarViewModel m_calendar;

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
            DateTime? validFromOverride = null;
            if (m_schedules.Count <= 0) validFromOverride = m_calendar.YearBeginning;
            EditScheduleViewModel dialogViewModel = new EditScheduleViewModel(schedule, GetMinValidFrom(schedule), GetMaxValidFrom(schedule), true, validFromOverride);
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
            if (dialogViewModel.Result == EditScheduleViewModel.EditScheduleResult.Ok)
            {
                m_schedules.Add(schedule);
            }
        }
        private void EditSchedule(WeekScheduleViewModel param)
        {
            param.PushCopy();
            EditScheduleViewModel dialogViewModel = new EditScheduleViewModel(param, GetMinValidFrom(param), GetMaxValidFrom(param));
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
            if(dialogViewModel.Result == EditScheduleViewModel.EditScheduleResult.Cancel)
            {
                param.PopCopy(WorkingCopyResult.Cancel);
            }
            else if (dialogViewModel.Result == EditScheduleViewModel.EditScheduleResult.Ok)
            {
                param.PopCopy(WorkingCopyResult.Ok);
            }
        }
        private void Close(object param)
        {
            GlobalConfig.Dialogs.Close(this);
        }

        private DateTime GetMinValidFrom(WeekScheduleViewModel schedule)
        {
            if (schedule == null)
            {
                return (m_schedules.Count > 0 ? m_schedules[m_schedules.Count - 1].StartDate : m_calendar.YearBeginning.AddDays(-1.0));
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
                    return m_calendar.YearBeginning.AddDays(-1.0);
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
                return m_calendar.YearEnding.AddDays(1.0);
            }
            else
            {
                int index = m_schedules.IndexOf(schedule);
                if (index < m_schedules.Count - 1)
                {
                    return m_schedules[index + 1].StartDate;
                }
                else return m_calendar.YearEnding.AddDays(1.0);
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
