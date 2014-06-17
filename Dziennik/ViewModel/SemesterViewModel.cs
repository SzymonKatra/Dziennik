using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Dziennik.Model;

namespace Dziennik.ViewModel
{
    public class SemesterViewModel : ObservableObject
    {
        public SemesterViewModel()
            : this(new Semester())
        {
        }
        public SemesterViewModel(Semester semester)
        {
            m_model = semester;

            m_marks = new SynchronizedObservableCollection<MarkViewModel, Mark>(m_model.Marks, (m) => { return new MarkViewModel(m); });
        }

        private Semester m_model;
        public Semester Model
        {
            get { return m_model; }
        }

        private SynchronizedObservableCollection<MarkViewModel, Mark> m_marks;
        public SynchronizedObservableCollection<MarkViewModel, Mark> Marks
        {
            get { return m_marks; }
            //set { m_marks = value; OnPropertyChanged("Marks"); }
        }
        public decimal EndingMark
        {
            get { return m_model.EndingMark; }
            set { m_model.EndingMark = value; OnPropertyChanged("EndingMark"); }
        }
    }
}
