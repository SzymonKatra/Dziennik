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
            m_marks.CollectionChanged += m_marks_CollectionChanged;
        }

        private Semester m_model;
        public Semester Model
        {
            get { return m_model; }
        }

        public ObservableCollection<MarkViewModel> m_marks = new ObservableCollection<MarkViewModel>();
        public ObservableCollection<MarkViewModel> Marks
        {
            get { return m_marks; }
        }
        public decimal EndingMark
        {
            get { return m_model.EndingMark; }
            set { m_model.EndingMark = value; OnPropertyChanged("EndingMark"); }
        }

        private void m_marks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch(e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems == null) return;
                    int insertIndex = e.NewStartingIndex;
                    if (insertIndex < 0)
                    {
                        foreach (MarkViewModel mvm in e.NewItems) m_model.Marks.Add(mvm.Model);
                    }
                    else
                    {
                        foreach(MarkViewModel mvm in e.NewItems)
                        {
                            m_model.Marks.Insert(insertIndex, mvm.Model);
                            insertIndex++;
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Move:
                    if (e.OldItems == null || e.OldItems.Count < 1) return;
                    MarkViewModel movedMvm = (MarkViewModel)e.OldItems[0];
                    m_model.Marks.RemoveAt(e.OldStartingIndex);
                    m_model.Marks.Insert(e.NewStartingIndex, movedMvm.Model);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems == null) return;
                    if (e.OldStartingIndex >= 0) m_model.Marks.RemoveAt(e.OldStartingIndex); else
                    {
                        foreach (MarkViewModel mvm in e.OldItems) m_model.Marks.Remove(mvm.Model);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    if (e.OldItems == null || e.NewItems == null || e.NewItems.Count < 1) return;
                    if (e.OldStartingIndex >= 0) m_model.Marks[e.OldStartingIndex] = ((MarkViewModel)e.NewItems[0]).Model; else
                    {
                        for (int i = 0; i < e.OldStartingIndex; i++)
                        {
                            MarkViewModel oldMvm = (MarkViewModel)e.OldItems[i];
                            MarkViewModel newMvm = (MarkViewModel)e.NewItems[i];
                            int index = m_model.Marks.IndexOf(oldMvm.Model);
                            if (index >= 0) m_model.Marks[index] = newMvm.Model;
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    m_model.Marks.Clear();
                    break;
            }
        }
    }
}
