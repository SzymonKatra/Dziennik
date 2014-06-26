using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dziennik.Model
{
    [Serializable]
    public sealed class SchoolClass
    {
        private string m_name;
        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        private List<GlobalStudent> m_students = new List<GlobalStudent>();
        public List<GlobalStudent> Students
        {
            get { return m_students; }
            set { m_students = value; }
        }

        private List<SchoolGroup> m_groups = new List<SchoolGroup>();
        public List<SchoolGroup> Groups
        {
            get { return m_groups; }
            set { m_groups = value; }
        }
    }
}
