using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.Model;

namespace Dziennik.ViewModel
{
    public class SchoolGroupViewModel : ViewModelBase<SchoolGroupViewModel, SchoolGroup>
    {
        public class Overdue
        {
            public DateTime Date { get; set; }
            public int Hour { get; set; }

            public Overdue(DateTime date, int hour)
            {
                this.Date = date;
                this.Hour = hour;
            }
        }

        public SchoolGroupViewModel()
            : this(new SchoolGroup())
        {
        }
        public SchoolGroupViewModel(SchoolGroup model)
            : base(model)
        {
            m_students = new SynchronizedPerItemObservableCollection<StudentInGroupViewModel, StudentInGroup>(Model.Students, (m) => { return new StudentInGroupViewModel(m); });;
            m_globalSubjects = new SynchronizedObservableCollection<GlobalSubjectViewModel, GlobalSubject>(Model.Subjects, m => new GlobalSubjectViewModel(m));
            m_realizedSubjects = new SynchronizedObservableCollection<RealizedSubjectViewModel, RealizedSubject>(Model.RealizedSubjects, m => new RealizedSubjectViewModel(m));

            SubscribeStudents();
            SubscribeRealizedSubjects();
        }

        private SchoolClassViewModel m_ownerClass;
        [DatabaseIgnoreSearchRelations]
        public SchoolClassViewModel OwnerClass
        {
            get { return m_ownerClass; }
            set
            {
                m_ownerClass = value;
                m_statistics = new StatisticsViewModel(this);
                RaisePropertyChanged("Statistics");
            }
        }

        public string Name
        {
            get { return Model.Name; }
            set { Model.Name = value; RaisePropertyChanged("Name"); }
        }
        private SynchronizedPerItemObservableCollection<StudentInGroupViewModel, StudentInGroup> m_students;
        [DatabaseInversePropertyOwner("OwnerGroup", "SubscribeStudents")]
        public SynchronizedPerItemObservableCollection<StudentInGroupViewModel, StudentInGroup> Students
        {
            get { return m_students; }
            set
            {
                UnsubscribeStudents();
                m_students = value;
                SubscribeStudents();
                Model.Students = value.ModelCollection;
                RaisePropertyChanged("Students");
            }
        }
        private SynchronizedObservableCollection<GlobalSubjectViewModel, GlobalSubject> m_globalSubjects;
        [DatabaseRelationCollection("GlobalSubjects")]
        public SynchronizedObservableCollection<GlobalSubjectViewModel, GlobalSubject> GlobalSubjects
        {
            get { return m_globalSubjects; }
            set
            {
                m_globalSubjects = value;
                Model.Subjects = value.ModelCollection;
                RaisePropertyChanged("GlobalSubjects");
            }
        }
        private SynchronizedObservableCollection<RealizedSubjectViewModel, RealizedSubject> m_realizedSubjects;
        [DatabaseRelationCollection("GroupRealizedSubjects")]
        [DatabaseInversePropertyOwner("OwnerGroup", "xxx")]
        public SynchronizedObservableCollection<RealizedSubjectViewModel, RealizedSubject> RealizedSubjects
        {
            get { return m_realizedSubjects; }
            set
            {
                UnsubscribeRealizedSubjects();
                m_realizedSubjects = value;
                SubscribeRealizedSubjects();
                Model.RealizedSubjects = value.ModelCollection;
                RaisePropertyChanged("RealizedSubjects");
            }
        }

        public string RealizedSubjectsDisplay
        {
            get
            {
                int realizedFromCurriculum = m_realizedSubjects.Count(x => !x.IsCustom);
                int realizedOutsideCurriculum = m_realizedSubjects.Count(x => x.IsCustom);

                return string.Format(GlobalConfig.GetStringResource("lang_RealizedSubjectsCountFormat"), realizedFromCurriculum, m_globalSubjects.Count, realizedOutsideCurriculum);
            }
        }

        public int RemainingHoursOfLessons
        {
            get
            {
                if (OwnerClass == null || OwnerClass.Calendar == null) return 0;

                DateTime now = DateTime.Now;
                DateTime today = now.Date;
                if (today < OwnerClass.Calendar.YearBeginning) today = OwnerClass.Calendar.YearBeginning;
                int result = 0;

                for (DateTime i = today; i < OwnerClass.Calendar.YearEnding; i = i.AddDays(1.0))
                {
                    bool nextDay = false;
                    foreach (var offDay in OwnerClass.Calendar.OffDays)
                    {
                        if (i >= offDay.Start && i <= offDay.End)
                        {
                            nextDay = true;
                            break;
                        }
                    }
                    if (nextDay) continue;

                    WeekScheduleViewModel schedule = GlobalConfig.GlobalDatabase.ViewModel.CurrentSchedule;
                    DayScheduleViewModel day = null;
                    switch(i.DayOfWeek)
                    {
                        case DayOfWeek.Monday: day = schedule.Monday; break;
                        case DayOfWeek.Tuesday: day = schedule.Tuesday; break;
                        case DayOfWeek.Wednesday: day = schedule.Wednesday; break;
                        case DayOfWeek.Thursday: day = schedule.Thursday; break;
                        case DayOfWeek.Friday: day = schedule.Friday; break;
                    }

                    if (day != null)
                    {
                        int currentHour = GlobalConfig.GetCurrentHourNumber(now);
                        if (i == today)
                        {
                            if (currentHour != -2) result += day.HoursSchedule.Count(x => x.SelectedGroup == this && (x.Hour > currentHour));
                        }
                        else
                        {
                            result += day.HoursSchedule.Count(x => x.SelectedGroup == this);
                        }
                    }
                }

                return result;
            }
        }
        public string RemainingHoursOfLessonsDisplay
        {
            get
            {
                return string.Format(GlobalConfig.GetStringResource("lang_RemainingHoursOfLessonsFormat"), RemainingHoursOfLessons);
            }
        }

        [DatabaseIgnoreSearchRelations]
        public IEnumerable<Overdue> OverdueSubjects
        {
            get
            {
                List<Overdue> overdues = new List<Overdue>();

                if (OwnerClass == null || OwnerClass.Calendar == null) return overdues;

                for (int i = 0; i < GlobalConfig.GlobalDatabase.ViewModel.Schedules.Count; i++)
                {
                    WeekScheduleViewModel schedule = GlobalConfig.GlobalDatabase.ViewModel.Schedules[i];
                    DateTime now = DateTime.Now;
                    DateTime endDate = (i < GlobalConfig.GlobalDatabase.ViewModel.Schedules.Count - 1 ? GlobalConfig.GlobalDatabase.ViewModel.Schedules[i + 1].StartDate.AddDays(-1.0) : this.OwnerClass.Calendar.YearEnding);
                    if (endDate > now) endDate= now.Date;

                    for (DateTime date = schedule.StartDate.Date; date <= endDate; date = date.AddDays(1.0))
                    {
                        bool nextDay = false;
                        foreach (var offDay in this.OwnerClass.Calendar.OffDays)
                        {
                            if (date >= offDay.Start && date <= offDay.End)
                            {
                                nextDay = true;
                                break;
                            }
                        }
                        if (nextDay) continue;

                        DayScheduleViewModel day = null;
                        switch (date.DayOfWeek)
                        {
                            case DayOfWeek.Monday: day = schedule.Monday; break;
                            case DayOfWeek.Tuesday: day = schedule.Tuesday; break;
                            case DayOfWeek.Wednesday: day = schedule.Wednesday; break;
                            case DayOfWeek.Thursday: day = schedule.Thursday; break;
                            case DayOfWeek.Friday: day = schedule.Friday; break;
                        }
                        if (day != null)
                        {
                            List<SelectedHourViewModel> hours = new List<SelectedHourViewModel>(day.HoursSchedule.Where(x => x.SelectedGroup == this));
                            if (date == now.Date)
                            {
                                int currentHour = GlobalConfig.GetCurrentHourNumber(now);
                                if (currentHour == -1) hours.Clear();
                                if (currentHour > 0) hours.RemoveAll(x => currentHour < x.Hour);
                            }
                            IEnumerable<RealizedSubjectViewModel> currentDateRealized = this.RealizedSubjects.Where(x => x.RealizedDate.Date == date.Date);
                            foreach (var item in currentDateRealized)
                            {
                                SelectedHourViewModel found = hours.FirstOrDefault(x=> x.Hour == item.RealizedHour);
                                if (found != null) hours.Remove(found);
                            }

                            foreach (var h in hours)
                            {
                                overdues.Add(new Overdue(date, h.Hour));
                            }
                        }
                    }
                }

                return overdues;
            }
        }
        [DatabaseIgnoreSearchRelations]
        public string OverdueSubjectsDisplay
        {
            get
            {
                return string.Format(GlobalConfig.GetStringResource("lang_OverdueSubjectsFormat"), OverdueSubjects.Count());
            }
        }
        [DatabaseIgnoreSearchRelations]
        public bool HasOverdueSubjects
        {
            get
            {
                return (OverdueSubjects.Count() > 0);
            }
        }

        private StatisticsViewModel m_statistics;
        public StatisticsViewModel Statistics
        {
            get { return m_statistics; }
        }

        //http://stackoverflow.com/questions/248273/count-number-of-mondays-in-a-given-date-range
        private static int CountDays(DayOfWeek day, DateTime start, DateTime end)
        {
            TimeSpan ts = end - start;                       // Total duration
            int count = (int)Math.Floor(ts.TotalDays / 7);   // Number of whole weeks
            int remainder = (int)(ts.TotalDays % 7);         // Number of remaining days
            int sinceLastDay = (int)(end.DayOfWeek - day);   // Number of days since last [day]
            if (sinceLastDay < 0) sinceLastDay += 7;         // Adjust for negative days since last [day]

            // If the days in excess of an even week are greater than or equal to the number days since the last [day], then count this one, too.
            if (remainder >= sinceLastDay) count++;

            return count;
        }

        public void RaiseRemainingHoursOfLessonsChanged()
        {
            RaisePropertyChanged("RemainingHoursOfLessons");
            RaisePropertyChanged("RemainingHoursOfLessonsDisplay");
            RaisePropertyChanged("OverdueSubjects");
            RaisePropertyChanged("OverdueSubjectsDisplay");
            RaisePropertyChanged("HasOverdueSubjects");
        }

        private void SubscribeRealizedSubjects()
        {
            m_realizedSubjects.CollectionChanged += m_realizedSubjects_CollectionChanged;
            m_realizedSubjects.Added += m_realizedSubjects_Added;
            m_realizedSubjects.Removed += m_realizedSubjects_Removed;
        }
        private void UnsubscribeRealizedSubjects()
        {
            m_realizedSubjects.CollectionChanged -= m_realizedSubjects_CollectionChanged;
            m_realizedSubjects.Added -= m_realizedSubjects_Added;
            m_realizedSubjects.Removed -= m_realizedSubjects_Removed;
        }

        private void m_realizedSubjects_Added(object sender, NotifyCollectionChangedSimpleEventArgs<RealizedSubjectViewModel> e)
        {
            foreach (var item in e.Items)
            {
                item.OwnerGroup = this;
            }
        }
        private void m_realizedSubjects_Removed(object sender, NotifyCollectionChangedSimpleEventArgs<RealizedSubjectViewModel> e)
        {
            foreach (var item in e.Items)
            {
                item.OwnerGroup = null;
            }
        } 
        
        private void m_realizedSubjects_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged("RealizedSubjectsDisplay");
            RaisePropertyChanged("OverdueSubjects");
            RaisePropertyChanged("OverdueSubjectsDisplay");
            RaisePropertyChanged("HasOverdueSubjects");
        }

        private void SubscribeStudents()
        {
            m_students.Added += m_students_Added;
            m_students.Removed += m_students_Removed;
        }
        private void UnsubscribeStudents()
        {
            m_students.Added -= m_students_Added;
            m_students.Removed -= m_students_Removed;
        }
        private void m_students_Added(object sender, NotifyCollectionChangedSimpleEventArgs<StudentInGroupViewModel> e)
        {
            foreach (var item in e.Items)
            {
                item.OwnerGroup = this;
            }
        }
        private void m_students_Removed(object sender, NotifyCollectionChangedSimpleEventArgs<StudentInGroupViewModel> e)
        {
            foreach (var item in e.Items)
            {
                item.OwnerGroup = null;
            }
        }

        protected override void OnPushCopy()
        {
            ObjectsPack pack = new ObjectsPack();
            pack.Write(this.Name);

            CopyStack.Push(pack);

            this.Students.PushCopy();
            this.GlobalSubjects.PushCopy();
            this.RealizedSubjects.PushCopy();
        }
        protected override void OnPopCopy(WorkingCopyResult result)
        {
            ObjectsPack pack = CopyStack.Pop();

            if (result == WorkingCopyResult.Cancel)
            {
                this.Name = (string)pack.Read();
            }

            this.Students.PopCopy(result);
            this.GlobalSubjects.PopCopy(result);
            this.RealizedSubjects.PopCopy(result);
        }
    }
}
