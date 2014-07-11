using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Dziennik.Model;
using System.Xml.Linq;

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
            SubscribeMarks();
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
                UnsubscribeMarks();

                m_marks = value;

                SubscribeMarks();

                m_model.Marks = value.ModelCollection;
                RaisePropertyChanged("Marks");
            }
        }
        public decimal AverageMark
        {
            get
            {
                int validMarksWeight = CountValidMarksWeight();
                if (validMarksWeight <= 0) return 0M;

                decimal sum = 0M;

                foreach (MarkViewModel item in m_marks) if (item.IsValueValid) sum += item.Value * item.Weight;

                return Ext.DecimalRoundHalfUp(sum / (decimal)validMarksWeight, GlobalConfig.DecimalRoundingPoints);
            }
        }

        public int CountValidMarksWeight()
        {
            return m_marks.Count((m) => { return m.IsValueValid; });
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
            RaisePropertyChanged("AverageMark");
            OnMarksChanged(EventArgs.Empty);
        }

        private void OnMarksChanged(EventArgs e)
        {
            EventHandler handler = MarksChanged;
            if (handler != null) handler(this, e);
        }

        private void SubscribeMarks()
        {
            m_marks.CollectionChanged += m_marks_CollectionChanged;
            m_marks.ItemPropertyInCollectionChanged += m_marks_ItemPropertyInCollectionChanged;
            //m_marks.Removed += m_marks_Removed;
        }
        private void UnsubscribeMarks()
        {
            m_marks.CollectionChanged -= m_marks_CollectionChanged;
            m_marks.ItemPropertyInCollectionChanged -= m_marks_ItemPropertyInCollectionChanged;
            //m_marks.Removed -= m_marks_Removed;
        }

        //private void m_marks_Removed(object sender, NotifyCollectionChangedSimpleEventArgs<MarkViewModel> e)
        //{
        //    foreach (var item in e.Items)
        //    {
        //        GlobalConfig.Database.Marks.Remove(item.Model);
        //    }
        //}
    }
}
