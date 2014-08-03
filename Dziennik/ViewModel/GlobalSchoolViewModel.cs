using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.Model;

namespace Dziennik.ViewModel
{
    public sealed class GlobalSchoolViewModel : ObservableObject, IModelExposable<GlobalSchool>
    {
        public GlobalSchoolViewModel()
            : this(new GlobalSchool())
        {
        }
        public GlobalSchoolViewModel(GlobalSchool model)
        {
            m_model = model;

            m_calendars = new SynchronizedObservableCollection<CalendarViewModel, Calendar>(m_model.Calendars, m => new CalendarViewModel(m));
        }

        private GlobalSchool m_model;
        public GlobalSchool Model
        {
            get { return m_model; }
        }

        private SynchronizedObservableCollection<CalendarViewModel, Calendar> m_calendars;
        public SynchronizedObservableCollection<CalendarViewModel, Calendar> Calendars
        {
            get { return m_calendars; }
            set
            {
                m_calendars = value;
                m_model.Calendars = value.ModelCollection;
                RaisePropertyChanged("Calendars");
            }
        }
    }
}
