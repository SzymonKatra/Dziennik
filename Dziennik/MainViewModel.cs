using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Dziennik.WPFControls;
using Dziennik.CommandUtils;
using Dziennik.ViewModel;

namespace Dziennik
{
    public class MainViewModel : ObservableObject
    {
        public MainViewModel()
        {
            m_addMarkCommand = new RelayCommand<object>(AddMark, (x) => { return true; });
            m_editMarkCommand = new RelayCommand<object>(EditMark, (x) => { return true; });

            StudentViewModel s;

            s = new StudentViewModel();
            s.Id = 3;
            s.Name = "aaaaaaaaaaaaaaaa";
            s.Surname = "bbb";
            s.Email = "eaaa";
            s.FirstSemester.Marks.Add(new MarkViewModel() { Value = 5M });
            s.FirstSemester.Marks.Add(new MarkViewModel() { Value = 3.5M });
            m_students.Add(s);

            s = new StudentViewModel();
            s.Id = 1;
            s.Name = "ccc";
            s.Surname = "ddd";
            s.Email = "eccc";
            s.FirstSemester.Marks.Add(new MarkViewModel() { Value = 3M });
            s.FirstSemester.Marks.Add(new MarkViewModel() { Value = 2.5M });
            m_students.Add(s);

            s = new StudentViewModel();
            s.Id = 4;
            s.Name = "ggg";
            s.Surname = "hhh";
            s.Email = "eggg";
            s.FirstSemester.Marks.Add(new MarkViewModel() { Value = 4M });
            s.FirstSemester.Marks.Add(new MarkViewModel() { Value = 4.5M });
            s.FirstSemester.Marks.Add(new MarkViewModel() { Value = 2.5M });
            m_students.Add(s);

            s = new StudentViewModel();
            s.Id = 2;
            s.Name = "eee";
            s.Surname = "fff";
            s.Email = "eeee";
            s.FirstSemester.Marks.Add(new MarkViewModel() { Value = 1M });
            s.FirstSemester.Marks.Add(new MarkViewModel() { Value = 6M });
            m_students.Add(s);

            m_students = new ObservableCollection<StudentViewModel>(m_students.OrderBy(x => x.Id));
        }

        private ObservableCollection<StudentViewModel> m_students = new ObservableCollection<StudentViewModel>();
        public ObservableCollection<StudentViewModel> Students
        {
            get { return m_students; }
        }

        private StudentViewModel m_selectedStudent;
        public StudentViewModel SelectedStudent
        {
            get { return m_selectedStudent; }
            set { m_selectedStudent = value; OnPropertyChanged("SelectedStudent"); }
        }

        private MarkViewModel m_selectedMark;
        public MarkViewModel SelectedMark
        {
            get { return m_selectedMark; }
            set { m_selectedMark = value; OnPropertyChanged("SelectedMark"); }
        }

        private RelayCommand<object> m_addMarkCommand;
        public ICommand AddMarkCommand
        {
            get { return m_addMarkCommand; }
        }
        private RelayCommand<object> m_editMarkCommand;
        public ICommand EditMarkCommand
        {
            get { return m_editMarkCommand; }
        }

        public void AddMark(object e)
        {
            Console.WriteLine("add mark ");
            //var mark0 = SelectedStudent.FirstSemester.Marks[0];
            //SelectedStudent.FirstSemester.Marks[0] = SelectedStudent.FirstSemester.Marks[1];
            //SelectedStudent.FirstSemester.Marks[1] = mark0;
            SelectedStudent.FirstSemester.Marks = new SynchronizedObservableCollection<MarkViewModel, Model.Mark>(new List<Model.Mark>(), (m) => { return new MarkViewModel(m); });
            SelectedStudent.FirstSemester.Marks.Add(new MarkViewModel() { Value = 5M });
            SelectedStudent.FirstSemester.Marks.Add(new MarkViewModel() { Value = 2.5M });
            SelectedStudent.FirstSemester.Marks.Add(new MarkViewModel() { Value = 6M });
            SelectedStudent.FirstSemester.Marks.Add(new MarkViewModel() { Value = 1M });
        }
        public void EditMark(object e)
        {
            SelectedStudent.FirstSemester.Marks.Remove(SelectedMark);
        }
    }
}
