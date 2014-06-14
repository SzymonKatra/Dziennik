using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Dziennik
{
    public class MainViewModel : ObservableObject
    {
        private ObservableCollection<Student> m_students = new ObservableCollection<Student>();
        public ObservableCollection<Student> Students
        {
            get { return m_students; }
        }

        public MainViewModel()
        {
            Student s;
            s = new Student();
            s.Name = "aaa";
            s.Surname = "bbb";
            s.Email = "eaaa";
            s.Marks.Add(new Mark() { Value = 5M });
            s.Marks.Add(new Mark() { Value = 3.5M });
            m_students.Add(s);

            s = new Student();
            s.Name = "ccc";
            s.Surname = "ddd";
            s.Email = "eccc";
            s.Marks.Add(new Mark() { Value = 3M });
            s.Marks.Add(new Mark() { Value = 2.5M });
            m_students.Add(s);
        }
    }
}
