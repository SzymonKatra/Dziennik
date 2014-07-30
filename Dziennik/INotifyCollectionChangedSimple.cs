using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dziennik
{
    public interface INotifyCollectionChangedSimple<T> : IEnumerable<T>
    {
        event EventHandler<NotifyCollectionChangedSimpleEventArgs<T>> Added;
        event EventHandler<NotifyCollectionChangedSimpleEventArgs<T>> Removed;

        void RaiseAddedForAll();
        void RaiseRemovedForAll();
    }
}
