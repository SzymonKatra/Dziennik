using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Dziennik
{
    public class ObservableCollectionNotifySimple<T> : ObservableCollection<T>, INotifyCollectionChangedSimple<T>
    {
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
            List<T> items = new List<T>(this);
            
            base.ClearItems();

            OnRemoved(new NotifyCollectionChangedSimpleEventArgs<T>(items));
        }
        protected override void InsertItem(int index, T item)
        {
            List<T> items = new List<T>();
            items.Add(item);
            
            base.InsertItem(index, item);

            OnAdded(new NotifyCollectionChangedSimpleEventArgs<T>(items));
        }
        protected override void RemoveItem(int index)
        {
            List<T> items = new List<T>();
            items.Add(this[index]);
            
            base.RemoveItem(index);

            OnRemoved(new NotifyCollectionChangedSimpleEventArgs<T>(items));
        }
        protected override void SetItem(int index, T item)
        {
            List<T> removedItems = new List<T>();
            removedItems.Add(this[index]);
            
            List<T> addedItems = new List<T>();
            addedItems.Add(item);
            
            base.SetItem(index, item);

            OnRemoved(new NotifyCollectionChangedSimpleEventArgs<T>(removedItems));
            OnAdded(new NotifyCollectionChangedSimpleEventArgs<T>(addedItems));
        }
        protected override void MoveItem(int oldIndex, int newIndex)
        {
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
    }
}
