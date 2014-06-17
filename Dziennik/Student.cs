using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Dziennik.x
{
    public class Student : ObservableObject
    {
        public Student()
        {
            m_marksFirst.CollectionChanged += m_marksFirst_CollectionChanged;
            m_marksSecond.CollectionChanged += m_marksSecond_CollectionChanged;
        }

        private void m_marksFirst_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SubscribeMarksIfNeed(e, MarkPropertyFirstChanged);
            RaiseOnChangedMarkFirst();
        }
        private void MarkPropertyFirstChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                RaiseOnChangedMarkFirst();
            }
        }

        private void m_marksSecond_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SubscribeMarksIfNeed(e, MarkPropertySecondChanged);
            RaiseOnChangedMarkSecond();
        }
        private void MarkPropertySecondChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                RaiseOnChangedMarkSecond();
            }
        }

        private void SubscribeMarksIfNeed(NotifyCollectionChangedEventArgs e, PropertyChangedEventHandler handler)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Mark x in e.NewItems)
                {
                    x.PropertyChanged += handler;
                }
            }
        }
        private void RaiseOnChangedMarkFirst()
        {
            OnPropertyChanged("AverageMarkFirst");
            OnPropertyChanged("AverageMarkAll");
        }
        private void RaiseOnChangedMarkSecond()
        {
            OnPropertyChanged("AverageMarkSecond");
            OnPropertyChanged("AverageMarkAll");
        }

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
