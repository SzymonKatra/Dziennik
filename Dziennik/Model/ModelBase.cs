using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dziennik.Model
{
    [Serializable]
    public abstract class ModelBase
    {
        private ulong? m_id;
        public ulong? Id
        {
            get
            {
                return m_id;
            }
            set
            {
                if (m_id != null) throw new InvalidOperationException("Id already set");
                m_id = value;
            }
        }
    }
}
