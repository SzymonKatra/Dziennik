using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dziennik.Model
{
    [Serializable]
    public sealed class SchoolGroup
    {
        private string m_name;
        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        private List<StudentInGroup> m_students = new List<StudentInGroup>();
        public List<StudentInGroup> Students
        {
            get { return m_students; }
            set { m_students = value; }
        }
    }
}
