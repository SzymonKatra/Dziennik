using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Dziennik
{
    public class Student : ObservableObject
    {
        private string m_name;
        public string Name
        {
            get { return m_name; }
            set { m_name = value; OnPropertyChanged("Name"); }
        }

        private string m_surname;
        public string Surname
        {
            get { return m_surname; }
            set { m_surname = value; OnPropertyChanged("Surname"); }
        }

        private string m_email;
        public string Email
        {
            get { return m_email; }
            set { m_email = value; OnPropertyChanged("Email"); }
        }

        private ObservableCollection<Mark> m_marks = new ObservableCollection<Mark>();
        public ObservableCollection<Mark> Marks
        {
            get { return m_marks; }
        }
    }
}
