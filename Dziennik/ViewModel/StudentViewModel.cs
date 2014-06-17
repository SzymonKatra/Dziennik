using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Dziennik.Model;

namespace Dziennik.ViewModel
{
    public class StudentViewModel : ObservableObject
    {
        public StudentViewModel()
            : this(new Student())
        {
        }
        public StudentViewModel(Student student)
        {
            m_model = student;
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

        private ObservableCollection<MarkViewModel> m_firstMarks = new ObservableCollection<MarkViewModel>();
        public ObservableCollection<MarkViewModel> FirstMarks
        {
            get { return m_firstMarks; }
        }

        private ObservableCollection<MarkViewModel> m_secondMarks = new ObservableCollection<MarkViewModel>();
        public ObservableCollection<MarkViewModel> SecondMarks
        {
            get { return m_secondMarks; }
        }

        public decimal YearEndingMark
        {
            get { return m_model.YearEndingMark; }
            set { m_model.YearEndingMark = value; OnPropertyChanged("YearEndingMark"); }
        }
    }
}
