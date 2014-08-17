using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.Model;
using System.Globalization;

namespace Dziennik.ViewModel
{
    public sealed class StudentInGroupViewModel : ViewModelBase<StudentInGroupViewModel, StudentInGroup>
    {
        public StudentInGroupViewModel()
            : this(new StudentInGroup())
        {
        }
        public StudentInGroupViewModel(StudentInGroup model)
            : base(model)
        {
            m_firstSemester = new SemesterViewModel(Model.FirstSemester);
            m_secondSemester = new SemesterViewModel(Model.SecondSemester);
            m_presence = new SynchronizedObservableCollection<RealizedSubjectPresenceViewModel, RealizedSubjectPresence>(Model.Presence, m => new RealizedSubjectPresenceViewModel(m));

            SemesterSubscribe(m_firstSemester);
            SemesterSubscribe(m_secondSemester);
        }

        private SchoolGroupViewModel m_ownerGroup;
        [DatabaseIgnoreSearchRelations]
        public SchoolGroupViewModel OwnerGroup
        {
            get { return m_ownerGroup; }
            set { m_ownerGroup = value; }
        }

        public int Number
        {
            get { return Model.Number; }
            set { Model.Number = value; RaisePropertyChanged("Number"); }
        }
        private SemesterViewModel m_firstSemester;
        public SemesterViewModel FirstSemester
        {
            get { return m_firstSemester; }
            set
            {
                SemesterUnsubscribe(m_firstSemester);

                m_firstSemester = value;

                SemesterSubscribe(m_firstSemester);

                Model.FirstSemester = value.Model;
                RaisePropertyChanged("FirstSemester");
            }
        }
        private SemesterViewModel m_secondSemester;
        public SemesterViewModel SecondSemester
        {
            get { return m_secondSemester; }
            set
            {
                SemesterUnsubscribe(m_secondSemester);

                m_secondSemester = value;

                SemesterSubscribe(m_secondSemester);

                Model.FirstSemester = value.Model;
                RaisePropertyChanged("SecondSemester");
            }
        }
        private SynchronizedObservableCollection<RealizedSubjectPresenceViewModel, RealizedSubjectPresence> m_presence;
        public SynchronizedObservableCollection<RealizedSubjectPresenceViewModel, RealizedSubjectPresence> Presence
        {
            get { return m_presence; }
            set
            {
                m_presence = value;
                Model.Presence = value.ModelCollection;
                RaisePropertyChanged("Presence");
            }
        }
        private GlobalStudentViewModel m_globalStudent;
        [DatabaseRelationProperty("GlobalStudents", "GlobalStudentId")]
        public GlobalStudentViewModel GlobalStudent
        {
            get { return m_globalStudent; }
            set { m_globalStudent = value; RaisePropertyChanged("GlobalStudent"); }
        }
        public bool IsRemoved
        {
            get { return Model.IsRemoved; }
            set { Model.IsRemoved = value; RaisePropertyChanged("IsRemoved"); }
        }

        public decimal AverageMarkAll
        {
            get
            {
                int validMarksWeight = m_firstSemester.CountValidMarksWeight() + m_secondSemester.CountValidMarksWeight();
                if (validMarksWeight <= 0) return 0M;

                decimal sum = 0M;

                foreach (MarkViewModel item in m_firstSemester.Marks) if (item.IsValueValid) sum += item.Value * item.Weight;
                foreach (MarkViewModel item in m_secondSemester.Marks) if (item.IsValueValid) sum += item.Value * item.Weight;

                return decimal.Round(sum / (decimal)validMarksWeight, GlobalConfig.DecimalRoundingPoints, MidpointRounding.AwayFromZero);
            }
        }

        public decimal HalfEndingMark
        {
            get { return Model.HalfEndingMark; }
            set { Model.HalfEndingMark = value; RaisePropertyChanged("HalfEndingMark"); }
        }
        public decimal YearEndingMark
        {
            get { return Model.YearEndingMark; }
            set { Model.YearEndingMark = value; RaisePropertyChanged("YearEndingMark"); }
        }

        public IEnumerable<RealizedSubjectPresenceViewModel> FirstPresence
        {
            get
            {
                if (Presence.Count > 0 && (Presence[0].RealizedSubject == null || OwnerGroup.OwnerClass.Calendar == null)) return null; // to prevent errors while loading database
                IEnumerable<RealizedSubjectPresenceViewModel> valid = Presence.Where((x) => x.RealizedSubject.RealizedDate >= OwnerGroup.OwnerClass.Calendar.YearBeginning && x.RealizedSubject.RealizedDate < OwnerGroup.OwnerClass.Calendar.SemesterSeparator);
                return valid;
            }
        }
        public IEnumerable<RealizedSubjectPresenceViewModel> SecondPresence
        {
            get
            {
                if (Presence.Count > 0 && (Presence[0].RealizedSubject == null || OwnerGroup.OwnerClass.Calendar == null)) return null; // to prevent errors while loading database
                IEnumerable<RealizedSubjectPresenceViewModel> valid = Presence.Where((x) => x.RealizedSubject.RealizedDate >= OwnerGroup.OwnerClass.Calendar.SemesterSeparator && x.RealizedSubject.RealizedDate <= OwnerGroup.OwnerClass.Calendar.YearEnding);
                return valid;
            }
        }
        public IEnumerable<RealizedSubjectPresenceViewModel> YearPresence
        {
            get
            {
                if (Presence.Count > 0 && (Presence[0].RealizedSubject == null || OwnerGroup.OwnerClass.Calendar == null)) return null; // to prevent errors while loading database   
                IEnumerable<RealizedSubjectPresenceViewModel> valid = Presence.Where((x) => x.RealizedSubject.RealizedDate >= OwnerGroup.OwnerClass.Calendar.YearBeginning && x.RealizedSubject.RealizedDate <= OwnerGroup.OwnerClass.Calendar.YearEnding);
                return valid;
            }
        }
        public decimal AttendanceFirst
        {
            get
            {
                var valid = FirstPresence;
                if (valid == null) return -1M;
                return ComputeAttendance(valid);
            }
        }
        public decimal AttendanceSecond
        {
            get
            {
                var valid = SecondPresence;
                if (valid == null) return -1M;
                return ComputeAttendance(valid);
            }
        }
        public decimal AttendanceYear
        {
            get
            {
                var valid = YearPresence;
                if (valid == null) return -1M;
                return ComputeAttendance(valid);
            }
        }
        public string AttendanceFirstDisplay
        {
            get
            {
                //TODO: find better solution for preventing exceptions while loading (NullReferenceExeption)
                var valid = FirstPresence;
                if (valid == null) return null;
                int wasPresentCount = valid.Count((x) => x.WasPresent);

                return string.Format(GlobalConfig.GetStringResource("lang_AttendanceDisplayFormat"), wasPresentCount, valid.Count(), (AttendanceFirst * 100M).ToString("G29" ,CultureInfo.InvariantCulture));
            }
        }
        public string AttendanceSecondDisplay
        {
            get
            {
                var valid = SecondPresence;
                if (valid == null) return null;
                int wasPresentCount = valid.Count((x) => x.WasPresent);

                return string.Format(GlobalConfig.GetStringResource("lang_AttendanceDisplayFormat"), wasPresentCount, valid.Count(), (AttendanceSecond * 100M).ToString("G29", CultureInfo.InvariantCulture));
            }
        }
        public string AttendanceYearDisplay
        {
            get
            {
                var valid = YearPresence;
                if (valid == null) return null;
                int wasPresentCount = valid.Count((x) => x.WasPresent);

                return string.Format(GlobalConfig.GetStringResource("lang_AttendanceDisplayFormat"), wasPresentCount, valid.Count(), (AttendanceYear * 100M).ToString("G29", CultureInfo.InvariantCulture));
            }
        }

        private decimal ComputeAttendance(IEnumerable<RealizedSubjectPresenceViewModel> presence)
        {
            int presenceCount = presence.Count();
            if (presenceCount == 0) return 1M;

            int wasPresentCount = presence.Count(x => x.WasPresent);
            decimal result = (decimal)wasPresentCount / (decimal)presenceCount;
            result = decimal.Round(result, 4, MidpointRounding.AwayFromZero);
            return result;
        }


        public void RaiseAttendanceChanged()
        {
            RaisePropertyChanged("FirstPresence");
            RaisePropertyChanged("SecondPresence");
            RaisePropertyChanged("YearPresence");
            RaisePropertyChanged("AttendanceFirst");
            RaisePropertyChanged("AttendanceSecond");
            RaisePropertyChanged("AttendanceYear");
            RaisePropertyChanged("AttendanceFirstDisplay");
            RaisePropertyChanged("AttendanceSecondDisplay");
            RaisePropertyChanged("AttendanceYearDisplay");
        }
        private void SemesterMarksChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged("AverageMarkAll");
        }

        private void SemesterSubscribe(SemesterViewModel semester)
        {
            semester.MarksChanged += SemesterMarksChanged;
        }
        private void SemesterUnsubscribe(SemesterViewModel semester)
        {
            semester.MarksChanged -= SemesterMarksChanged;
        }

        protected override void OnPushCopy()
        {
            ObjectsPack pack = new ObjectsPack();
            pack.Write(this.Number);
            pack.Write(this.GlobalStudent);
            pack.Write(this.IsRemoved);

            CopyStack.Push(pack);

            this.FirstSemester.PushCopy();
            this.SecondSemester.PushCopy();
            this.Presence.PushCopy();
        }
        protected override void OnPopCopy(WorkingCopyResult result)
        {
            ObjectsPack pack = CopyStack.Pop();

            if(result == WorkingCopyResult.Cancel)
            {
                this.Number = (int)pack.Read();
                this.GlobalStudent = (GlobalStudentViewModel)pack.Read();
                this.IsRemoved = (bool)pack.Read();
            }

            this.FirstSemester.PopCopy(result);
            this.SecondSemester.PopCopy(result);
            this.Presence.PopCopy(result);
        }
    }
}
