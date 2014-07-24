using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.Model;

namespace Dziennik.ViewModel
{
    public sealed class StudentInGroupViewModel : ObservableObject, IModelExposable<StudentInGroup>
    {
        public StudentInGroupViewModel()
            : this(new StudentInGroup())
        {
        }
        public StudentInGroupViewModel(StudentInGroup studentInGroup)
        {
            m_model = studentInGroup;

            m_firstSemester = new SemesterViewModel(m_model.FirstSemester);
            m_secondSemester = new SemesterViewModel(m_model.SecondSemester);
            m_presence = new SynchronizedObservableCollection<RealizedSubjectPresenceViewModel, RealizedSubjectPresence>(m_model.Presence, m => new RealizedSubjectPresenceViewModel(m));

            SemesterSubscribe(m_firstSemester);
            SemesterSubscribe(m_secondSemester);
        }

        private StudentInGroup m_model;
        public StudentInGroup Model
        {
            get { return m_model; }
        }

        public int Number
        {
            get { return m_model.Number; }
            set { m_model.Number = value; RaisePropertyChanged("Number"); }
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

                m_model.FirstSemester = value.Model;
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

                m_model.FirstSemester = value.Model;
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
                m_model.Presence = value.ModelCollection;
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
            get { return m_model.HalfEndingMark; }
            set { m_model.HalfEndingMark = value; RaisePropertyChanged("HalfEndingMark"); }
        }
        public decimal YearEndingMark
        {
            get { return m_model.YearEndingMark; }
            set { m_model.YearEndingMark = value; RaisePropertyChanged("YearEndingMark"); }
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
    }
}
