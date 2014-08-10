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
            m_marksCategories = new SynchronizedObservableCollection<MarksCategoryViewModel, MarksCategory>(m_model.MarksCategories, m => new MarksCategoryViewModel(m));
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
        private SynchronizedObservableCollection<MarksCategoryViewModel, MarksCategory> m_marksCategories;
        public SynchronizedObservableCollection<MarksCategoryViewModel, MarksCategory> MarksCategories
        {
            get { return m_marksCategories; }
            set
            {
                m_marksCategories = value;
                m_model.MarksCategories = value.ModelCollection;
                RaisePropertyChanged("MarksCategories");
            }
        }
    }
}
