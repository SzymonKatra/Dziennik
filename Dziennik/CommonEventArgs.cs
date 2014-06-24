using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dziennik
{
    public class CommonEventArgs<T> : EventArgs
    {
        public CommonEventArgs()
        {
        }
        public CommonEventArgs(T item)
        {
            m_item = item;
        }

        private T m_item;
        public T Item
        {
            get { return m_item; }
            set { m_item = value; }
        }
    }
}
