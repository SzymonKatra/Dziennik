using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Dziennik.WPFControls;
using Dziennik.CommandUtils;

namespace Dziennik
{
    public class MainViewModel : ObservableObject
    {
        public MainViewModel()
        {
            m_addMarkCommand = new RelayCommand<Student>(AddMark, (x) => { return true; });
            m_editMarkCommand = new RelayCommand<EditMarkEventArgs>(EditMark, (x) => { return true; });

            Student s;

            s = new Student();
            s.Id = 3;
            s.Name = "aaaaaaaaaaaaaaaa";
            s.Surname = "bbb";
            s.Email = "eaaa";
            s.MarksFirst.Add(new Mark() { Value = 5M });
            s.MarksFirst.Add(new Mark() { Value = 3.5M });
            m_students.Add(s);

            s = new Student();
            s.Id = 1;
            s.Name = "ccc";
            s.Surname = "ddd";
            s.Email = "eccc";
            s.MarksFirst.Add(new Mark() { Value = 3M });
            s.MarksFirst.Add(new Mark() { Value = 2.5M });
            m_students.Add(s);

            s = new Student();
            s.Id = 4;
            s.Name = "ggg";
            s.Surname = "hhh";
            s.Email = "eggg";
            s.MarksFirst.Add(new Mark() { Value = 4M });
            s.MarksFirst.Add(new Mark() { Value = 4.5M });
            s.MarksFirst.Add(new Mark() { Value = 2.5M });
            m_students.Add(s);

            s = new Student();
            s.Id = 2;
            s.Name = "eee";
            s.Surname = "fff";
            s.Email = "eeee";
            s.MarksFirst.Add(new Mark() { Value = 1M });
            s.MarksFirst.Add(new Mark() { Value = 6M });
            m_students.Add(s);

            m_students = new ObservableCollection<Student>(m_students.OrderBy(x => x.Id));
        }

        private ObservableCollection<Student> m_students = new ObservableCollection<Student>();
        public ObservableCollection<Student> Students
        {
            get { return m_students; }
        }

        private RelayCommand<Student> m_addMarkCommand;
        public ICommand AddMarkCommand
        {
            get { return m_addMarkCommand; }
        }
        private RelayCommand<EditMarkEventArgs> m_editMarkCommand;
        public ICommand EditMarkCommand
        {
            get { return m_editMarkCommand; }
        }

        public void AddMark(Student context)
        {
            Console.WriteLine("add mark ");
        }
        public void EditMark(EditMarkEventArgs e)
        {
            Console.WriteLine(e.Handled);
        }
    }
}
