using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Dziennik.ViewModel;

namespace Dziennik
{
    public class ChangelogPair<T>
    {
        public ChangeType Change { get; set; }
        public T Value { get; set; }
        public int Index { get; set; }
    }
    public enum ChangeType
    {
        Change,
        Add,
        Remove,
        Clear,
    }
    public class WorkingCollection<T> : ObservableCollection<T> where T : IViewModelShallowCopyable<T>, new()
    {
        private List<ChangelogPair<T>> m_changelogList = new List<ChangelogPair<T>>();
        private Collection<T> m_originalCollection;
        private Dictionary<T, T> m_originalItemsMapping = new Dictionary<T, T>(); // key - copy, value - original

        private bool m_trackingPaused = false;

        public WorkingCollection(Collection<T> originalCollection)
        {
            PauseTracking();

            m_originalCollection = originalCollection;

            foreach (var item in m_originalCollection)
            {
                T copy = new T();
                item.ShallowCopyDataTo(copy);
                this.Add(copy);
                m_originalItemsMapping.Add(copy, item);
            }

            ResumeTracking();
        }

        public void ApplyChange(T item)
        {
            AddChangelog(ChangeType.Change, item);
        }
        public void ApplyChangesToOriginalCollection()
        {
            foreach (var change in m_changelogList)
            {
                switch(change.Change)
                {
                    case ChangeType.Clear:
                        m_originalCollection.Clear();
                        m_originalItemsMapping.Clear();
                        break;

                    case ChangeType.Add:
                        m_originalCollection.Insert(change.Index, change.Value);
                        m_originalItemsMapping.Add(change.Value, change.Value);
                        break;

                    case ChangeType.Remove:
                        m_originalCollection.RemoveAt(change.Index);
                        m_originalItemsMapping.Remove(change.Value);
                        break;

                    case ChangeType.Change:
                        change.Value.ShallowCopyDataTo(m_originalItemsMapping[change.Value]);
                        break;
                }
            }

            m_changelogList.Clear();
        }

        protected override void ClearItems()
        {
            if (!m_trackingPaused) AddChangelog(ChangeType.Clear, default(T));
            base.ClearItems();
        }
        protected override void InsertItem(int index, T item)
        {
            if (!m_trackingPaused) AddChangelog(ChangeType.Add, item, index);
            base.InsertItem(index, item);
        }
        protected override void RemoveItem(int index)
        {
            if (!m_trackingPaused) AddChangelog(ChangeType.Remove, m_originalCollection[index], index);
            base.RemoveItem(index);
        }
        protected override void SetItem(int index, T item)
        {
            throw new InvalidOperationException("Not allowed");
        }

        protected void AddChangelog(ChangeType change, T value)
        {
            AddChangelog(change, value, -1);
        }
        protected void AddChangelog(ChangeType change, int index)
        {
            AddChangelog(change, default(T), index);
        }
        protected void AddChangelog(ChangeType change, T value, int index)
        {
            m_changelogList.Add(new ChangelogPair<T>() { Change = change, Value = value, Index = index });
        }

        protected void PauseTracking()
        {
            m_trackingPaused = true;
        }
        protected void ResumeTracking()
        {
            m_trackingPaused = false;
        }
    }
}
