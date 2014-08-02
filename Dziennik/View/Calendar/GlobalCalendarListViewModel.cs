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
        public GlobalCalendarListViewModel(ObservableCollection<CalendarViewModel> calendars)
        {
            m_addCalendarCommand = new RelayCommand(AddCalendar);
            m_editCalendarCommand = new RelayCommand(EditCalendar);

            m_calendars = calendars;
        }

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

        private void AddCalendar(object e)
        {

        }
        private void EditCalendar(object e)
        {

        }
    }
}
