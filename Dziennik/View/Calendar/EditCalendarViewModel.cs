using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Dziennik.ViewModel;
using Dziennik.CommandUtils;

namespace Dziennik.View
{
    public class EditCalendarViewModel : ObservableObject
    {
        public enum EditCalendarResult
        {
            Ok,
            Cancel,
            Remove,
        }

        public EditCalendarViewModel(CalendarViewModel calendar, bool isAddingMode = false)
        {
            m_okCommand = new RelayCommand(Ok);
            m_cancelCommand = new RelayCommand(Cancel);
            m_removeCalendarCommand = new RelayCommand(RemoveCalendar, CanRemoveCalendar);
            m_addOffDayCommand = new RelayCommand(AddOffDay);
            m_editOffDayCommand = new RelayCommand(EditOffDay);

            m_calendar = calendar;
            m_isAddingMode = isAddingMode;

            if(isAddingMode)
            {
                calendar.YearBeginning = calendar.SemesterSeparator = calendar.YearEnding = DateTime.Now.Date;
            }
        }

        private CalendarViewModel m_calendar;
        public CalendarViewModel Calendar
        {
            get { return m_calendar; }
        }

        private EditCalendarResult m_result = EditCalendarResult.Cancel;
        public EditCalendarResult Result
        {
            get { return m_result; }
        }

        private bool m_isAddingMode;
        public bool IsAddingMode
        {
            get { return m_isAddingMode; }
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

        private RelayCommand m_removeCalendarCommand;
        public ICommand RemoveCalendarCommand
        {
            get { return m_removeCalendarCommand; }
        }

        private RelayCommand m_addOffDayCommand;
        public ICommand AddOffDayCommand
        {
            get { return m_addOffDayCommand; }
        }

        private RelayCommand m_editOffDayCommand;
        public ICommand EditOffDayCommand
        {
            get { return m_editOffDayCommand; }
        }

        private OffDayViewModel m_selectedOffDay;
        public OffDayViewModel SelectedOffDay
        {
            get { return m_selectedOffDay; }
            set { m_selectedOffDay = value; RaisePropertyChanged("SelectedOffDay"); }
        }

        private void Ok(object e)
        {
            m_result = EditCalendarResult.Ok;
            GlobalConfig.Dialogs.Close(this);
        }
        private void Cancel(object e)
        {
            m_result = EditCalendarResult.Cancel;
            GlobalConfig.Dialogs.Close(this);
        }
        private void RemoveCalendar(object e)
        {
            m_result = EditCalendarResult.Remove;
            GlobalConfig.Dialogs.Close(this);
        }
        private bool CanRemoveCalendar(object e)
        {
            return !m_isAddingMode;
        }
        private void AddOffDay(object e)
        {
            OffDayViewModel offDay = new OffDayViewModel();
            EditOffDayViewModel dialogViewModel = new EditOffDayViewModel(offDay, true);
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
            if(dialogViewModel.Result == EditOffDayViewModel.EditOffDayResult.Ok)
            {
                m_calendar.OffDays.Add(offDay);    
            }
        }
        private void EditOffDay(object e)
        {
            m_selectedOffDay.PushCopy();
            EditOffDayViewModel dialogViewModel = new EditOffDayViewModel(m_selectedOffDay);
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
            if (dialogViewModel.Result == EditOffDayViewModel.EditOffDayResult.Remove)
            {
                m_selectedOffDay.PopCopy(WorkingCopyResult.Ok);
                m_calendar.OffDays.Remove(m_selectedOffDay);
                SelectedOffDay = null;
            }
            else if(dialogViewModel.Result == EditOffDayViewModel.EditOffDayResult.Ok)
            {
                m_selectedOffDay.PopCopy(WorkingCopyResult.Ok);
                //m_offDaysWorkingCopy.ApplyChange(m_selectedOffDay);
            }
            else if(dialogViewModel.Result == EditOffDayViewModel.EditOffDayResult.Cancel)
            {
                m_selectedOffDay.PopCopy(WorkingCopyResult.Cancel);
            }
        }
    }
}
