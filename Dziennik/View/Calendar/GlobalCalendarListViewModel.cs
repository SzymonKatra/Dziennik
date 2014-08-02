using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.ViewModel;
using System.Windows.Input;
using Dziennik.CommandUtils;
using System.Collections.ObjectModel;

namespace Dziennik.View
{
    public class GlobalCalendarListViewModel : ObservableObject
    {
        public GlobalCalendarListViewModel(ObservableCollection<CalendarViewModel> calendars, ICommand autoSaveCommand)
        {
            m_addCalendarCommand = new RelayCommand(AddCalendar);
            m_editCalendarCommand = new RelayCommand(EditCalendar);

            m_autoSaveCommand = autoSaveCommand;

            m_calendars = calendars;
        }

        private ICommand m_autoSaveCommand;

        private ObservableCollection<CalendarViewModel> m_calendars;
        public ObservableCollection<CalendarViewModel> Calendars
        {
            get { return m_calendars; }
        }

        private RelayCommand m_addCalendarCommand;
        public ICommand AddCalendarCommand
        {
            get { return m_addCalendarCommand; }
        }

        private RelayCommand m_editCalendarCommand;
        public ICommand EditCalendarCommand
        {
            get { return m_editCalendarCommand; }
        }

        private CalendarViewModel m_selectedCalendar;
        public CalendarViewModel SelectedCalendar
        {
            get { return m_selectedCalendar; }
            set { m_selectedCalendar = value; RaisePropertyChanged("SelectedCalendar"); }
        }

        private void AddCalendar(object e)
        {
            CalendarViewModel calendar = new CalendarViewModel();
            EditCalendarViewModel dialogViewModel = new EditCalendarViewModel(calendar, m_autoSaveCommand, true);
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
            if (dialogViewModel.Result == EditCalendarViewModel.EditCalendarResult.Ok)
            {
                m_calendars.Add(calendar);
            }
            if (dialogViewModel.Result != EditCalendarViewModel.EditCalendarResult.Cancel) m_autoSaveCommand.Execute(null);
        }
        private void EditCalendar(object e)
        {
            EditCalendarViewModel dialogViewModel = new EditCalendarViewModel(m_selectedCalendar, m_autoSaveCommand, true);
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
            if (dialogViewModel.Result == EditCalendarViewModel.EditCalendarResult.Remove)
            {
                m_calendars.Remove(m_selectedCalendar);
                m_selectedCalendar = null;
            }
            if (dialogViewModel.Result != EditCalendarViewModel.EditCalendarResult.Cancel) m_autoSaveCommand.Execute(null);
        }
    }
}
