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
        public GlobalSchoolViewModel(GlobalSchool model)
            : base(model)
        {
            m_calendars = new SynchronizedObservableCollection<CalendarViewModel, Calendar>(Model.Calendars, m => new CalendarViewModel(m));
            m_marksCategories = new SynchronizedObservableCollection<MarksCategoryViewModel, MarksCategory>(Model.MarksCategories, m => new MarksCategoryViewModel(m));
            m_notices = new SynchronizedObservableCollection<NoticeViewModel, Notice>(Model.Notices, m => new NoticeViewModel(m));
            m_hours = new LessonsHoursViewModel(Model.Hours);
            m_schedules = new SynchronizedObservableCollection<WeekScheduleViewModel, WeekSchedule>(Model.Schedules, m => new WeekScheduleViewModel(m));

            SubscribeHours();
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

        private LessonsHoursViewModel m_hours;
        public LessonsHoursViewModel Hours
        {
            get { return m_hours; }
            set
            {
                UnsubscribeHours();

                m_hours = value;

                SubscribeHours();

                Model.Hours = value.Model;
                RaisePropertyChanged("Hours");
            }
        }

        private SynchronizedObservableCollection<WeekScheduleViewModel, WeekSchedule> m_schedules;
        public SynchronizedObservableCollection<WeekScheduleViewModel, WeekSchedule> Schedules
        {
            get { return m_schedules; }
            set
            {
                m_schedules = value;
                Model.Schedules = value.ModelCollection;
                RaisePropertyChanged("Schedules");
            }
        }

        private WeekScheduleViewModel m_previousSchedule;
        public WeekScheduleViewModel CurrentSchedule
        {
            get
            {
                DateTime dateNow = DateTime.Now.Date;
                for (int i = m_schedules.Count - 1; i >= 0; i--)
                {
                    if (m_schedules[i].StartDate <= dateNow)
                    {
                        if (m_previousSchedule != m_schedules[i])
                        {
                            WeekScheduleViewModel temp = m_previousSchedule;
                            m_previousSchedule = m_schedules[i];
                            //ScheduleChanged(temp, m_schedules[i]);
                        }
                        return m_schedules[i];
                    }
                }

                return new WeekScheduleViewModel();
            }
        }

        public DateTime LastArchivedDate
        {
            get { return Model.LastArchivedDate; }
            set { Model.LastArchivedDate = value; RaisePropertyChanged("LastArchivedDate"); }
        }

        private void SubscribeHours()
        {
            m_hours.HoursAdded += m_hours_HoursAdded;
        }
        private void UnsubscribeHours()
        {
            m_hours.HoursAdded -= m_hours_HoursAdded;
        }

        private void m_hours_HoursAdded(object sender, EventArgs e)
        {
            foreach (var item in m_schedules)
            {
                item.Monday.PadHours(m_hours.Hours.Count);
                item.Tuesday.PadHours(m_hours.Hours.Count);
                item.Wednesday.PadHours(m_hours.Hours.Count);
                item.Thursday.PadHours(m_hours.Hours.Count);
                item.Friday.PadHours(m_hours.Hours.Count);
                //if (item.Monday.HoursCount < m_hours.Hours.Count) item.Monday.HoursCount = m_hours.Hours.Count;
                //if (item.Tuesday.HoursCount < m_hours.Hours.Count) item.Tuesday.HoursCount = m_hours.Hours.Count;
                //if (item.Wednesday.HoursCount < m_hours.Hours.Count) item.Wednesday.HoursCount = m_hours.Hours.Count;
                //if (item.Thursday.HoursCount < m_hours.Hours.Count) item.Thursday.HoursCount = m_hours.Hours.Count;
                //if (item.Friday.HoursCount < m_hours.Hours.Count) item.Friday.HoursCount = m_hours.Hours.Count;
            }
        }

        protected override void OnPushCopy()
        {
            ObjectsPack pack = new ObjectsPack();
            pack.Write(this.LastArchivedDate);

            CopyStack.Push(pack);

            this.Calendars.PushCopy();
            this.MarksCategories.PushCopy();
            this.Notices.PushCopy();
            this.Hours.PushCopy();
            this.Schedules.PushCopy();
        }
        protected override void OnPopCopy(WorkingCopyResult result)
        {
            ObjectsPack pack = CopyStack.Pop();

            if (result == WorkingCopyResult.Cancel)
            {
                this.LastArchivedDate = (DateTime)pack.Read();
            }

            this.Calendars.PopCopy(result);
            this.MarksCategories.PopCopy(result);
            this.Notices.PopCopy(result);
            this.Hours.PopCopy(result);
            this.Schedules.PopCopy(result);
        }
    }
}
