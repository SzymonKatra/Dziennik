using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dziennik
{
    public class ObservableCollectionExtended<T> : ObservableCollection<T>, INotifyCollectionChangedSimple<T>
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
            OnRemoved(new NotifyCollectionChangedSimpleEventArgs<T>(items));
            base.ClearItems();
        }
        protected override void InsertItem(int index, T item)
        {
            List<T> items = new List<T>();
            items.Add(item);
            OnAdded(new NotifyCollectionChangedSimpleEventArgs<T>(items));
            base.InsertItem(index, item);
        }
        protected override void RemoveItem(int index)
        {
            List<T> items = new List<T>();
            items.Add(this[index]);
            OnRemoved(new NotifyCollectionChangedSimpleEventArgs<T>(items));
            base.RemoveItem(index);
        }
        protected override void SetItem(int index, T item)
        {
            List<T> items = new List<T>();
            items.Add(this[index]);
            OnRemoved(new NotifyCollectionChangedSimpleEventArgs<T>(items));
            List<T> items = new List<T>();
            items.Add(item);
            OnRemoved(new NotifyCollectionChangedSimpleEventArgs<T>(items));
            base.SetItem(index, item);
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
