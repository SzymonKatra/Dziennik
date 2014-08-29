using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dziennik.Controls
{
    public class ChangedStateEventArgs : EventArgs
    {
        private bool m_hasError = false;
        public bool HasError
        {
            get { return m_hasError; }
            set { m_hasError = value; }
        }

        public ChangedStateEventArgs()
            : this(false)
        {
        }
        public ChangedStateEventArgs(bool hasError)
        {
            m_hasError = hasError;
        }
    }
}
