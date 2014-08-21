using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Dziennik.Model;
using System.Xml.Linq;

namespace Dziennik.ViewModel
{
    public class MarkChangedEventArgs : EventArgs
    {
        private MarkViewModel m_mark;
        public MarkViewModel Mark
        {
            get { return m_mark; }
            set { m_mark = value; }
        }

        public MarkChangedEventArgs(MarkViewModel mark)
        {
            m_mark = mark;
        }
    }

    public sealed class SemesterViewModel : ViewModelBase<SemesterViewModel, Semester>
    {
        public SemesterViewModel()
            : this(new Semester())
        {
        }
        public SemesterViewModel(Semester model)
            : base(model)
        {
            m_marks = new SynchronizedPerItemObservableCollection<MarkViewModel, Mark>(Model.Marks, (m) => { return new MarkViewModel(m); });
            SubscribeMarks();
        }

        public event EventHandler<MarkChangedEventArgs> MarksChanged;

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


        private void m_marks_Added(object sender, NotifyCollectionChangedSimpleEventArgs<MarkViewModel> e)
        {
            foreach (var item in e.Items)
            {
                PerformMarksChanged(item);
            }
        }
        private void m_marks_Removed(object sender, NotifyCollectionChangedSimpleEventArgs<MarkViewModel> e)
        {
            foreach (var item in e.Items)
            {
                PerformMarksChanged(item);
            }
        }
        private void m_marks_ItemPropertyInCollectionChanged(object sender, ItemPropertyInCollectionChangedEventArgs<MarkViewModel> e)
        {
            if (e.PropertyName == "Value" || e.PropertyName == "IsValueValid" || e.PropertyName == "Weight") PerformMarksChanged(e.Item);
        }
        private void PerformMarksChanged(MarkViewModel mark)
        {
            RaisePropertyChanged("AverageMark");
            OnMarksChanged(new MarkChangedEventArgs(mark));
        }

        private void OnMarksChanged(MarkChangedEventArgs e)
        {
            EventHandler<MarkChangedEventArgs> handler = MarksChanged;
            if (handler != null) handler(this, e);
        }

        private void SubscribeMarks()
        {
            m_marks.Added += m_marks_Added;
            m_marks.Removed += m_marks_Removed;
            m_marks.ItemPropertyInCollectionChanged += m_marks_ItemPropertyInCollectionChanged;
        }
        private void UnsubscribeMarks()
        {
            m_marks.Added -= m_marks_Added;
            m_marks.Removed -= m_marks_Removed;
            m_marks.ItemPropertyInCollectionChanged -= m_marks_ItemPropertyInCollectionChanged;
        }

        public static decimal ProposeMark(decimal average)
        {
            return decimal.Round(average - 0.1M, MidpointRounding.AwayFromZero);
        }

        protected override void OnPushCopy()
        {
            this.Marks.PushCopy();
        }
        protected override void OnPopCopy(WorkingCopyResult result)
        {
            this.Marks.PopCopy(result);
        }
    }
}
