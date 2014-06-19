using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Dziennik.Model;

namespace Dziennik.ViewModel
{
    public sealed class StudentViewModel : ObservableObject, IViewModelExposable<Student>
    {
        public StudentViewModel()
            : this(new Student())
        {
        }
        public StudentViewModel(Student student)
        {
            m_model = student;

            m_firstSemester = new SemesterViewModel(m_model.FirstSemester);
            m_secondSemester = new SemesterViewModel(m_model.SecondSemester);

            SemesterSubscribe(m_firstSemester);
            SemesterSubscribe(m_secondSemester);
        }

        private Student m_model;
        public Student Model
        {
            get { return m_model; }
        }

        public int Id
        {
            get { return m_model.Id; }
            set { m_model.Id = value; OnPropertyChanged("Id"); }
        }
        public string Name
        {
            get { return m_model.Name; }
            set { m_model.Name = value; OnPropertyChanged("Name"); }
        }
        public string Surname
        {
            get { return m_model.Surname; }
            set { m_model.Surname = value; OnPropertyChanged("Surname"); }
        }
        public string Email
        {
            get { return m_model.Email; }
            set { m_model.Email = value; OnPropertyChanged("Email"); }
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
                OnPropertyChanged("FirstSemester");
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
                OnPropertyChanged("SecondSemester");
            }
        }

        public decimal AverageMarkAll
        {
            get
            {
                if (m_firstSemester.Marks.Count + m_secondSemester.Marks.Count <= 0) return 0M;

                decimal sum = 0M;

                foreach (MarkViewModel item in m_firstSemester.Marks) sum += item.Value;
                foreach (MarkViewModel item in m_secondSemester.Marks) sum += item.Value;

                return decimal.Round(sum / (decimal)(m_firstSemester.Marks.Count + m_secondSemester.Marks.Count), GlobalConfig.DecimalRoundingPoints);
            }
        }
        public decimal YearEndingMark
        {
            get { return m_model.YearEndingMark; }
            set { m_model.YearEndingMark = value; OnPropertyChanged("YearEndingMark"); }
        }

        private void SemesterMarksChanged(object sender, EventArgs e)
        {
            OnPropertyChanged("AverageMarkAll");
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
