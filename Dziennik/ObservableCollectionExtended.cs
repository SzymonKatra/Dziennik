using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dziennik
{
    public class ObservableCollectionExtended<T> : ObservableCollection<T>, INotifyCollectionChangedSimple<T>, IWorkingCopyAvailable where T : IWorkingCopyAvailable
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
        }

        private bool m_workingCopyStarted = false;
        public bool WorkingCopyStarted
        {
            get { return m_workingCopyStarted; }
        }
        private Stack<CollectionCopy> m_copyStack = new Stack<CollectionCopy>();
        private List<CollectionChangedPair> m_currentChangelog;

        public event EventHandler<NotifyCollectionChangedSimpleEventArgs<T>> Added;
        public event EventHandler<NotifyCollectionChangedSimpleEventArgs<T>> Removed;

        public void RaiseAddedForAll()
        {
            if (Added != null) // to save time
            {
                List<T> items = new List<T>(this);
                OnAdded(new NotifyCollectionChangedSimpleEventArgs<T>(items, false));
            }
        }
        public void RaiseRemovedForAll()
        {
            if (Removed != null) // to save time
            {
                List<T> items = new List<T>(this);
                OnRemoved(new NotifyCollectionChangedSimpleEventArgs<T>(items, false));
            }
        }

        protected override void ClearItems()
        {
            if (m_currentChangelog != null)
            {
                m_currentChangelog.Add(new CollectionChangedPair() { Change = CollectionChangeType.Cleared, ClearedList = new List<T>(this) });
            }

            List<T> items = new List<T>(this);
            OnRemoved(new NotifyCollectionChangedSimpleEventArgs<T>(items));
            base.ClearItems();
        }
        protected override void InsertItem(int index, T item)
        {
            if (m_currentChangelog != null)
            {
                m_currentChangelog.Add(new CollectionChangedPair() { Change = CollectionChangeType.Added, NewIndex = index, NewValue = item });
            }

            List<T> items = new List<T>();
            items.Add(item);
            OnAdded(new NotifyCollectionChangedSimpleEventArgs<T>(items));
            base.InsertItem(index, item);
        }
        protected override void RemoveItem(int index)
        {
            if (m_currentChangelog != null)
            {
                m_currentChangelog.Add(new CollectionChangedPair() { Change = CollectionChangeType.Removed, OldIndex = index, OldValue = this[index] });
            }

            List<T> items = new List<T>();
            items.Add(this[index]);
            OnRemoved(new NotifyCollectionChangedSimpleEventArgs<T>(items));
            base.RemoveItem(index);
        }
        protected override void SetItem(int index, T item)
        {
            if (m_currentChangelog != null)
            {
                m_currentChangelog.Add(new CollectionChangedPair() { Change = CollectionChangeType.Changed, OldIndex = index, OldValue = this[index], NewIndex = index, NewValue = item });
            }

            List<T> items = new List<T>();
            items.Add(this[index]);
            OnRemoved(new NotifyCollectionChangedSimpleEventArgs<T>(items));
            items = new List<T>();
            items.Add(item);
            OnRemoved(new NotifyCollectionChangedSimpleEventArgs<T>(items));
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

        protected virtual void OnAdded(NotifyCollectionChangedSimpleEventArgs<T> e)
        {
            EventHandler<NotifyCollectionChangedSimpleEventArgs<T>> handler = Added;
            if (handler != null) handler(this, e);
        }
        protected virtual void OnRemoved(NotifyCollectionChangedSimpleEventArgs<T> e)
        {
            EventHandler<NotifyCollectionChangedSimpleEventArgs<T>> handler = Removed;
            if (handler != null) handler(this, e);
        }

        public void PushCopy()
        {
            //if (m_workingCopyStarted) throw new InvalidOperationException("Already started");
            //m_workingCopyStarted = true;
            CollectionCopy copy = new CollectionCopy();
            copy.Changelog = new List<CollectionChangedPair>();
            copy.PushedItems = new List<T>(this);

            m_copyStack.Push(copy);
            m_currentChangelog = copy.Changelog;
            foreach (var item in this) item.PushCopy();
        }
        public void PopCopy(WorkingCopyResult result)
        {
            ///if (!m_workingCopyStarted) throw new InvalidOperationException("Must be started to end");
            //m_workingCopyStarted = false;
            CollectionCopy copy = m_copyStack.Pop();
            foreach (var item in copy.PushedItems) item.PopCopy(result);
            if (result == WorkingCopyResult.Cancel) Revert();
            m_currentChangelog = null;
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
