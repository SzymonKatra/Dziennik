using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Dziennik.Model;

namespace Dziennik.ViewModel
{
    public class SemesterViewModel : ObservableObject, IViewModelExposable<Semester>
    {
        public SemesterViewModel()
            : this(new Semester())
        {
        }
        public SemesterViewModel(Semester semester)
        {
            m_model = semester;

            m_marks = new SynchronizedPerItemObservableCollection<MarkViewModel, Mark>(m_model.Marks, (m) => { return new MarkViewModel(m); });
            m_marks.CollectionChanged += m_marks_CollectionChanged;
            m_marks.ItemPropertyInCollectionChanged += m_marks_ItemPropertyInCollectionChanged;
        }

        private Semester m_model;
        public Semester Model
        {
            get { return m_model; }
        }

        public event EventHandler MarksChanged;

        private SynchronizedPerItemObservableCollection<MarkViewModel, Mark> m_marks;
        public SynchronizedPerItemObservableCollection<MarkViewModel, Mark> Marks
        {
            get { return m_marks; }
            set
            {
                m_marks.CollectionChanged -= m_marks_CollectionChanged;
                m_marks.ItemPropertyInCollectionChanged -= m_marks_ItemPropertyInCollectionChanged;

                m_marks = value;

                m_marks.CollectionChanged += m_marks_CollectionChanged;
                m_marks.ItemPropertyInCollectionChanged += m_marks_ItemPropertyInCollectionChanged;

                m_model.Marks = value.ModelCollection;
                OnPropertyChanged("Marks");
            }
        }
        public decimal AverageMark
        {
            get
            {
                if (m_marks.Count <= 0) return 0M;

                decimal sum = 0M;

                foreach (MarkViewModel item in m_marks) sum += item.Value;

                return decimal.Round(sum / (decimal)m_marks.Count, 2);
            }
        }
        public decimal EndingMark
        {
            get { return m_model.EndingMark; }
            set { m_model.EndingMark = value; OnPropertyChanged("EndingMark"); }
        }

        private void m_marks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            PerformMarksChanged();
        }
        private void m_marks_ItemPropertyInCollectionChanged(object sender, ItemPropertyInCollectionChangedEventArgs<MarkViewModel> e)
        {
            if (e.PropertyName == "Value") PerformMarksChanged();
        }
        private void PerformMarksChanged()
        {
            OnPropertyChanged("AverageMark");
            OnMarksChanged(EventArgs.Empty);
        }

        protected virtual void OnMarksChanged(EventArgs e)
        {
            EventHandler handler = MarksChanged;
            if (handler != null) handler(this, e);
        }
    }
}
