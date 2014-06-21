using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dziennik.Model
{
    public sealed class SchoolClass
    {
        private string m_name;
        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        private List<Student> m_students = new List<Student>();
        public List<Student> Students
        {
            get { return m_students; }
            set { m_students = value; }
        }
    }
}
