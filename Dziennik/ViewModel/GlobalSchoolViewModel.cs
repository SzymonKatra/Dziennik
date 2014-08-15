using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.Model;

namespace Dziennik.ViewModel
{
    public sealed class GlobalSchoolViewModel : ViewModelBase<GlobalSchoolViewModel, GlobalSchool>
    {
        public GlobalSchoolViewModel()
            : this(new GlobalSchool())
        {
        }
        public GlobalSchoolViewModel(GlobalSchool model) : base(model)
        {
            m_calendars = new SynchronizedObservableCollection<CalendarViewModel, Calendar>(Model.Calendars, m => new CalendarViewModel(m));
            m_marksCategories = new SynchronizedObservableCollection<MarksCategoryViewModel, MarksCategory>(Model.MarksCategories, m => new MarksCategoryViewModel(m));
            m_notices = new SynchronizedObservableCollection<NoticeViewModel, Notice>(Model.Notices, m => new NoticeViewModel(m));
        }

        private SynchronizedObservableCollection<CalendarViewModel, Calendar> m_calendars;
        public SynchronizedObservableCollection<CalendarViewModel, Calendar> Calendars
        {
            get { return m_calendars; }
            set
            {
                m_calendars = value;
                Model.Calendars = value.ModelCollection;
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
                Model.MarksCategories = value.ModelCollection;
                RaisePropertyChanged("MarksCategories");
            }
        }

        private SynchronizedObservableCollection<NoticeViewModel, Notice> m_notices;
        public SynchronizedObservableCollection<NoticeViewModel, Notice> Notices
        {
            get { return m_notices; }
            set
            {
                m_notices = value;
                Model.Notices = value.ModelCollection;
                RaisePropertyChanged("Notices");
            }
        }

        protected override void OnWorkingCopyStarted()
        {
            m_calendars.StartWorkingCopy();
            m_marksCategories.StartWorkingCopy();
            m_notices.StartWorkingCopy();
        }
        protected override void OnWorkingCopyEnded(WorkingCopyResult result)
        {
            m_calendars.EndWorkingCopy(result);
            m_marksCategories.EndWorkingCopy(result);
            m_notices.EndWorkingCopy(result);
        }
    }
}
