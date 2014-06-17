using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dziennik.Model
{
    public class Mark
    {
        private decimal m_value;
        public decimal Value
        {
            get { return m_value; }
            set { m_value = value; }
        }

        private DateTime m_addDate;
        public DateTime AddDate
        {
            get { return m_addDate; }
            set { m_addDate = value; }
        }

        private DateTime m_lastChangeDate;
        public DateTime LastChangeDate
        {
            get { return m_lastChangeDate; }
            set { m_lastChangeDate = value; }
        }

        private string m_description;
        public string Description
        {
            get { return m_description; }
            set { m_description = value; }
        }
    }
}
