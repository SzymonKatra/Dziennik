﻿using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dziennik
{
    public class ObservableCollectionExtended<T> : ObservableCollection<T>, INotifyCollectionChangedSimple<T>, IWorkingCopyAvailable
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

        private List<CollectionChangedPair> m_changelog = new List<CollectionChangedPair>();

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
            if(m_workingCopyStarted)
            {
                m_changelog.Add(new CollectionChangedPair() { Change = CollectionChangeType.Cleared, ClearedList = new List<T>(this) });
            }

            List<T> items = new List<T>(this);
            OnRemoved(new NotifyCollectionChangedSimpleEventArgs<T>(items));
            base.ClearItems();
        }
        protected override void InsertItem(int index, T item)
        {
            if(m_workingCopyStarted)
            {
                m_changelog.Add(new CollectionChangedPair() { Change = CollectionChangeType.Added, NewIndex = index, NewValue = item });
            }

            List<T> items = new List<T>();
            items.Add(item);
            OnAdded(new NotifyCollectionChangedSimpleEventArgs<T>(items));
            base.InsertItem(index, item);
        }
        protected override void RemoveItem(int index)
        {
            if(m_workingCopyStarted)
            {
                m_changelog.Add(new CollectionChangedPair() { Change = CollectionChangeType.Removed, OldIndex = index, OldValue = this[index] });
            }

            List<T> items = new List<T>();
            items.Add(this[index]);
            OnRemoved(new NotifyCollectionChangedSimpleEventArgs<T>(items));
            base.RemoveItem(index);
        }
        protected override void SetItem(int index, T item)
        {
            if(m_workingCopyStarted)
            {
                m_changelog.Add(new CollectionChangedPair() { Change = CollectionChangeType.Changed, OldIndex = index, OldValue = this[index], NewIndex = index, NewValue = item });
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
            if(m_workingCopyStarted)
            {
                m_changelog.Add(new CollectionChangedPair() { Change = CollectionChangeType.Moved, OldIndex = oldIndex, NewIndex = newIndex });
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

        private bool m_workingCopyStarted = false;
        public void StartWorkingCopy()
        {
            if (m_workingCopyStarted) throw new InvalidOperationException("Already started");
            m_workingCopyStarted = true;
            m_changelog.Clear();
        }
        public void EndWorkingCopy(WorkingCopyResult result)
        {
            if (!m_workingCopyStarted) throw new InvalidOperationException("Must be started to end");
            m_workingCopyStarted = false;
            Revert();
        }

        private void Revert()
        {
            for (int i = m_changelog.Count - 1; i >= 0; i--)
            {
                CollectionChangedPair change = m_changelog[i];

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
