using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Dziennik.Model;
using System.Xml.Linq;

namespace Dziennik.ViewModel
{
    public sealed class SemesterViewModel : ViewModelBase<SemesterViewModel, Semester>
    {
        public SemesterViewModel()
            : this(new Semester())
        {
        }
        public SemesterViewModel(Semester model) : base(model)
        {
            m_marks = new SynchronizedPerItemObservableCollection<MarkViewModel, Mark>(Model.Marks, (m) => { return new MarkViewModel(m); });
            SubscribeMarks();
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

                Model.Marks = value.ModelCollection;
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

                return decimal.Round(sum / (decimal)validMarksWeight, GlobalConfig.DecimalRoundingPoints, MidpointRounding.AwayFromZero);
            }
        }

        public int CountValidMarksWeight()
        {
            return m_marks.Sum((m) => { return (m.IsValueValid ? m.Weight : 0); });
        }

        private void m_marks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            PerformMarksChanged();
        }
        private void m_marks_ItemPropertyInCollectionChanged(object sender, ItemPropertyInCollectionChangedEventArgs<MarkViewModel> e)
        {
            if (e.PropertyName == "Value" || e.PropertyName == "IsValueValid" || e.PropertyName == "Weight") PerformMarksChanged();
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
        }
        private void UnsubscribeMarks()
        {
            m_marks.CollectionChanged -= m_marks_CollectionChanged;
            m_marks.ItemPropertyInCollectionChanged -= m_marks_ItemPropertyInCollectionChanged;
        }

        public static decimal ProposeMark(decimal average)
        {
            return decimal.Round(average - 0.1M, MidpointRounding.AwayFromZero);
        }

        public override void CopyDataTo(SemesterViewModel viewModel)
        {
            viewModel.Marks = this.Marks;
        }
    }
}
