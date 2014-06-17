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
                    break;

                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems == null) return;
                    int removeIndex = e.OldStartingIndex;
                    if (removeIndex < 0)
                    {
                        foreach (MarkViewModel mvm in e.OldItems) m_model.Marks.Remove(mvm.Model);
                    }
                    else
                    {
                        foreach(MarkViewModel mvm in e.OldItems)
                        {
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    for (int i = 0; i < e.OldItems.Count; i++)
                    {
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    break;
            }
        }
    }
}
