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
        private int m_id;
        public int Id
        {
            get { return m_id; }
            set { m_id = value; OnPropertyChanged("Id"); }
        }

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

        private ObservableCollection<Mark> m_marksFirst = new ObservableCollection<Mark>();
        public ObservableCollection<Mark> MarksFirst
        {
            get { return m_marksFirst; }
        }
        public decimal AverageMarkFirst
        {
            get { return ComputeAverage(m_marksFirst); }
        }

        private ObservableCollection<Mark> m_marksSecond = new ObservableCollection<Mark>();
        public ObservableCollection<Mark> MarksSecond
        {
            get { return m_marksSecond; }
        }
        public decimal AverageMarkSecond
        {
            get { return ComputeAverage(m_marksSecond); }
        }
        
        public decimal AverageMarkAll
        {
            get
            {
                ObservableCollection<Mark> all = new ObservableCollection<Mark>(m_marksFirst.Concat(m_marksSecond));
                return ComputeAverage(all);
            }
        }

        private static decimal ComputeAverage(IEnumerable<Mark> marks)
        {
            if (marks.Count() <= 0) return decimal.Zero;

            decimal sum = 0M;
            foreach (Mark item in marks)
            {
                sum += item.Value;
            }

            return decimal.Round(sum / marks.Count(), 2);
        }
    }
}
