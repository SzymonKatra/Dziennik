using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dziennik
{
    public class ObjectsPack
    {
        private List<object> m_list = new List<object>();
        private int m_pointer = 0;

        public void Reset()
        {
            m_list.Clear();
            m_pointer = 0;
        }

        public void Write(object data)
        {
            m_list.Add(data);
        }

        public object Read()
        {
            return m_list[m_pointer++];
        }
    }
}
