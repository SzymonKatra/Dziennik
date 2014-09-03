using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dziennik
{
    public class ObservableCollectionWorkingCopy<T> : ObservableCollectionNotifySimple<T>, IWorkingCopyAvailable where T : IWorkingCopyAvailable
    {
        private enum CollectionChangeType
        {
            Added,
            Removed,
            Changed,
            Cleared,
            Moved,
        }
        private class CollectionChangedPair
        {
            public CollectionChangeType Change { get; set; }
            public int OldIndex { get; set; }
            public T OldValue { get; set; }
            public int NewIndex { get; set; }
            public T NewValue { get; set; }
            public List<T> ClearedList { get; set; }
        }
        private class CollectionCopy
        {
            public List<CollectionChangedPair> Changelog { get; set; }
            public List<T> PushedItems { get; set; }

            public static readonly CollectionCopy Separator = new CollectionCopy();
        }

        public int CopyDepth
        {
            get { return m_copyStack.Count; }
        }

        private Stack<CollectionCopy> m_waitingStack = new Stack<CollectionCopy>(); // contains all changes that were made in past. used when PopCopy Cancel is requested and undos all changes
        private Stack<CollectionCopy> m_copyStack = new Stack<CollectionCopy>();
        private List<CollectionChangedPair> m_currentChangelog;

        protected override void ClearItems()
        {
            if (m_currentChangelog != null)
            {
                m_currentChangelog.Add(new CollectionChangedPair() { Change = CollectionChangeType.Cleared, ClearedList = new List<T>(this) });
            }

            base.ClearItems();
        }
        protected override void InsertItem(int index, T item)
        {
            if (m_currentChangelog != null)
            {
                m_currentChangelog.Add(new CollectionChangedPair() { Change = CollectionChangeType.Added, NewIndex = index, NewValue = item });
            }

            base.InsertItem(index, item);
        }
        protected override void RemoveItem(int index)
        {
            if (m_currentChangelog != null)
            {
                m_currentChangelog.Add(new CollectionChangedPair() { Change = CollectionChangeType.Removed, OldIndex = index, OldValue = this[index] });
            }

            base.RemoveItem(index);
        }
        protected override void SetItem(int index, T item)
        {
            if (m_currentChangelog != null)
            {
                m_currentChangelog.Add(new CollectionChangedPair() { Change = CollectionChangeType.Changed, OldIndex = index, OldValue = this[index], NewIndex = index, NewValue = item });
            }

            base.SetItem(index, item);
        }
        protected override void MoveItem(int oldIndex, int newIndex)
        {
            if (m_currentChangelog != null)
            {
                m_currentChangelog.Add(new CollectionChangedPair() { Change = CollectionChangeType.Moved, OldIndex = oldIndex, NewIndex = newIndex });
            }

            base.MoveItem(oldIndex, newIndex);
        }

        public void PushCopy()
        {
            CollectionCopy copy = new CollectionCopy();
            copy.Changelog = new List<CollectionChangedPair>();
            copy.PushedItems = new List<T>(this);

            m_copyStack.Push(copy);
            m_currentChangelog = copy.Changelog;
            foreach (var item in this) item.PushCopy();
        }
        public void PopCopy(WorkingCopyResult result)
        {
            CollectionCopy copy = m_copyStack.Pop();
            m_waitingStack.Push(copy);
            foreach (var item in copy.PushedItems) item.PopCopy(result);
            if (result == WorkingCopyResult.Cancel)
            {
                while (m_waitingStack.Count > 0) // undo all changed that were made in past
                {
                    CollectionCopy oldCopy = m_waitingStack.Pop();
                    m_currentChangelog = oldCopy.Changelog;
                    Revert();
                }
            }
            m_currentChangelog = (m_copyStack.Count >= 1 ? m_copyStack.Peek().Changelog : null);
            if (m_currentChangelog == null) m_waitingStack.Clear(); //m_waitingList.Clear();
        }

        private void Revert()
        {
            for (int i = m_currentChangelog.Count - 1; i >= 0; i--)
            {
                CollectionChangedPair change = m_currentChangelog[i];

                switch (change.Change)
                {
                    case CollectionChangeType.Added:
                        base.RemoveItem(change.NewIndex);
                        break;

                    case CollectionChangeType.Removed:
                        base.InsertItem(change.OldIndex, change.OldValue);
                        break;

                    case CollectionChangeType.Changed:
                        base.SetItem(change.OldIndex, change.OldValue);
                        break;

                    case CollectionChangeType.Cleared:
                        foreach (var item in change.ClearedList)
                        {
                            base.InsertItem(base.Count, item);
                        }
                        break;

                    case CollectionChangeType.Moved:
                        base.MoveItem(change.NewIndex, change.OldIndex);
                        break;
                }
            }
        }
    }
}
