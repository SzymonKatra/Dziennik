using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.Model;

namespace Dziennik.ViewModel
{
    public sealed class StudentInGroupViewModel : ObservableObject, IViewModelExposable<StudentInGroup>
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

            SemesterSubscribe(m_firstSemester);
            SemesterSubscribe(m_secondSemester);
        }

        private StudentInGroup m_model;
        public StudentInGroup Model
        {
            get { return m_model; }
        }

        private SchoolClassViewModel m_schoolClass;
        public SchoolClassViewModel SchoolClass
        {
            get { return m_schoolClass; }
            set { m_schoolClass = value; }
        }

        public int GlobalId
        {
            get { return m_model.GlobalId; }
            set { m_model.GlobalId = value; RaisePropertyChanged("GlobalId"); }
        }
        public int Id
        {
            get { return m_model.Id; }
            set { m_model.Id = value; RaisePropertyChanged("Id"); }
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

        private GlobalStudentViewModel m_globalStudent; // SchoolClassViewModel must synchronize it
        public GlobalStudentViewModel GlobalStudent
        {
            get { return m_globalStudent; }
            set { m_globalStudent = value; RaisePropertyChanged("GlobalStudent"); }
        }
        public decimal AverageMarkAll
        {
            get
            {
                int validMarks = m_firstSemester.CountValidMarks() + m_secondSemester.CountValidMarks();
                if (validMarks <= 0) return 0M;

                decimal sum = 0M;

                foreach (MarkViewModel item in m_firstSemester.Marks) if (item.IsValueValid) sum += item.Value;
                foreach (MarkViewModel item in m_secondSemester.Marks) if (item.IsValueValid) sum += item.Value;

                return Ext.DecimalRoundHalfUp(sum / (decimal)validMarks, GlobalConfig.DecimalRoundingPoints);
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
