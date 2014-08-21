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
            m_schedule = new WeekScheduleViewModel(Model.Schedule);

            SubscribeSchedule();
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

        private WeekScheduleViewModel m_schedule;
        public WeekScheduleViewModel Schedule
        {
            get { return m_schedule; }
            set
            {
                UnsubscribeSchedule();

                m_schedule = value;

                SubscribeSchedule();

                Model.Schedule = value.Model;
                RaisePropertyChanged("Schedule");
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

                    switch(i.DayOfWeek)
                    {
                        case DayOfWeek.Monday: result += m_schedule.Monday; break;
                        case DayOfWeek.Tuesday: result += m_schedule.Tuesday; break;
                        case DayOfWeek.Wednesday: result += m_schedule.Wednesday; break;
                        case DayOfWeek.Thursday: result += m_schedule.Thursday; break;
                        case DayOfWeek.Friday: result += m_schedule.Friday; break;
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
        }

        private void SubscribeSchedule()
        {
            m_schedule.PropertyChanged += m_schedule_PropertyChanged;
        }
        private void UnsubscribeSchedule()
        {
            m_schedule.PropertyChanged += m_schedule_PropertyChanged;
        }

        private void m_schedule_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RaiseRemainingHoursOfLessonsChanged();
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
            this.Schedule.PushCopy();
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
            this.Schedule.PopCopy(result);
        }
    }
}
