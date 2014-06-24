using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dziennik
{
    public class NotifyCollectionChangedSimpleEventArgs<T> : EventArgs
    {
        public NotifyCollectionChangedSimpleEventArgs()
            : this(null)
        {
        }
        public NotifyCollectionChangedSimpleEventArgs(IEnumerable<T> items, bool trueChange = true)
        {
            m_items = items;
            m_trueChange = trueChange;
        }

        private IEnumerable<T> m_items;
        public IEnumerable<T> Items
        {
            get { return m_items; }
            set { m_items = value; }
        }

        private bool m_trueChange;
        public bool TrueChange
        {
            get { return m_trueChange; }
            set { m_trueChange = value; }
        }
    }
}
