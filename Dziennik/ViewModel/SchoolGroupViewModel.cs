using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.Model;

namespace Dziennik.ViewModel
{
    public class SchoolGroupViewModel : ViewModelBase<SchoolGroupViewModel, SchoolGroup>
    {
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
            m_schedules = new SynchronizedObservableCollection<WeekScheduleViewModel, WeekSchedule>(Model.Schedules, m => new WeekScheduleViewModel(m));

            SubscribeStudents();
            SubscribeRealizedSubjects();

            RaiseCurrentScheduleChanged();
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
                            ScheduleChanged(temp, m_schedules[i]);
                        }
                        return m_schedules[i];
                    }
                }

                return new WeekScheduleViewModel();
            }
        }

        private void ScheduleChanged(WeekScheduleViewModel oldSchedule, WeekScheduleViewModel newSchedule)
        {
            if (oldSchedule != null) oldSchedule.HoursCountChanged -= CurrentSchedule_HoursCountChanged;
            if (newSchedule != null) newSchedule.HoursCountChanged += CurrentSchedule_HoursCountChanged;
            RaiseCurrentScheduleChanged();
        }

        private void CurrentSchedule_HoursCountChanged(object sender, EventArgs e)
        {
            RaiseRemainingHoursOfLessonsChanged();
        }
        public void RaiseCurrentScheduleChanged()
        {
            RaisePropertyChanged("CurrentSchedule");
            RaiseRemainingHoursOfLessonsChanged();
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

                DateTime today = DateTime.Now.Date;
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

                    WeekScheduleViewModel schedule = CurrentSchedule;
                    switch(i.DayOfWeek)
                    {
                        case DayOfWeek.Monday: result += schedule.Monday.HoursCount; break;
                        case DayOfWeek.Tuesday: result += schedule.Tuesday.HoursCount; break;
                        case DayOfWeek.Wednesday: result += schedule.Wednesday.HoursCount; break;
                        case DayOfWeek.Thursday: result += schedule.Thursday.HoursCount; break;
                        case DayOfWeek.Friday: result += schedule.Friday.HoursCount; break;
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

        public IEnumerable<DateTime> OverdueSubjects
        {
            get
            {
                List<DateTime> overdues = new List<DateTime>();

                if (OwnerClass == null || OwnerClass.Calendar == null) return overdues;

                for (int i = 0; i < this.Schedules.Count; i++)
                {
                    WeekScheduleViewModel schedule = this.Schedules[i];
                    DateTime endDate = (i < this.Schedules.Count - 1 ? this.Schedules[i + 1].StartDate.AddDays(-1.0) : this.OwnerClass.Calendar.YearEnding);
                    if (endDate > DateTime.Now.Date) endDate = DateTime.Now.Date;

                    for (DateTime date = schedule.StartDate; date <= endDate; date = date.AddDays(1.0))
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

                        int toRealize = 0;
                        switch (date.DayOfWeek)
                        {
                            case DayOfWeek.Monday: toRealize += schedule.Monday.HoursCount; break;
                            case DayOfWeek.Tuesday: toRealize += schedule.Tuesday.HoursCount; break;
                            case DayOfWeek.Wednesday: toRealize += schedule.Wednesday.HoursCount; break;
                            case DayOfWeek.Thursday: toRealize += schedule.Thursday.HoursCount; break;
                            case DayOfWeek.Friday: toRealize += schedule.Friday.HoursCount; break;
                        }

                        int realizedCount = this.RealizedSubjects.Count(x => x.RealizedDate.Date == date.Date);

                        for (int j = 0; j < toRealize - realizedCount; j++)
                        {
                            overdues.Add(date);
                        }
                    }
                }

                return overdues;
            }
        }
        public string OverdueSubjectsDisplay
        {
            get
            {
                return string.Format(GlobalConfig.GetStringResource("lang_OverdueSubjectsFormat"), OverdueSubjects.Count());
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
        }

        private void SubscribeRealizedSubjects()
        {
            m_realizedSubjects.CollectionChanged += m_realizedSubjects_CollectionChanged;
        }
        private void UnsubscribeRealizedSubjects()
        {
            m_realizedSubjects.CollectionChanged -= m_realizedSubjects_CollectionChanged;
        }
        
        private void m_realizedSubjects_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged("RealizedSubjectsDisplay");
            RaisePropertyChanged("OverdueSubjects");
            RaisePropertyChanged("OverdueSubjectsDisplay");
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
            this.Schedules.PushCopy();
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
            this.Schedules.PopCopy(result);
        }
    }
}
