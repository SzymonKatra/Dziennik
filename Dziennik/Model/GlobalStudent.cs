using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dziennik.Model
{
    public sealed class GlobalStudent
    {
        private int m_id;
        public int Id
        {
            get { return m_id; }
            set { m_id = value; }
        }

        private string m_name;
        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        private string m_surname;
        public string Surname
        {
            get { return m_surname; }
            set { m_surname = value; }
        }

        private string m_email;
        public string Email
        {
            get { return m_email; }
            set { m_email = value; }
        }

        private string m_additionalInformation;
        public string AdditionalInformation
        {
            get { return m_additionalInformation; }
            set { m_additionalInformation = value; }
        }
    }
}
