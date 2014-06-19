using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Dziennik.Model;

namespace Dziennik.ViewModel
{
    public sealed class SemesterViewModel : ObservableObject, IViewModelExposable<Semester>
    {
        public SemesterViewModel()
            : this(new Semester())
        {
        }
        public SemesterViewModel(Semester semester)
        {
            m_model = semester;

            m_marks = new SynchronizedPerItemObservableCollection<MarkViewModel, Mark>(m_model.Marks, (m) => { return new MarkViewModel(m); });
            MarksSubscribe();
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
                MarksUnsubscribe();

                m_marks = value;

                MarksSubscribe();

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

                return decimal.Round(sum / (decimal)m_marks.Count, GlobalConfig.DecimalRoundingPoints);
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

        private void OnMarksChanged(EventArgs e)
        {
            EventHandler handler = MarksChanged;
            if (handler != null) handler(this, e);
        }

        private void MarksSubscribe()
        {
            m_marks.CollectionChanged += m_marks_CollectionChanged;
            m_marks.ItemPropertyInCollectionChanged += m_marks_ItemPropertyInCollectionChanged;
        }
        private void MarksUnsubscribe()
        {
            m_marks.CollectionChanged -= m_marks_CollectionChanged;
            m_marks.ItemPropertyInCollectionChanged -= m_marks_ItemPropertyInCollectionChanged;
        }
    }
}
