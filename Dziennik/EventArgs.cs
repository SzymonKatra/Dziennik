using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dziennik
{
    public class EventArgs<T> : EventArgs
    {
        private T m_value;
        public T Value
        {
            get { return m_value; }
            set { m_value = value; }
        }

        public EventArgs(T value)
            : base()
        {
            m_value = value;
        }
    }
}
