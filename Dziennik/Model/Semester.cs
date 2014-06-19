using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dziennik.Model
{
    public sealed class Semester
    {
        private List<Mark> m_marks = new List<Mark>();
        public List<Mark> Marks
        {
            get { return m_marks; }
            set { m_marks = value; }
        }

        private decimal m_endingMark;
        public decimal EndingMark
        {
            get { return m_endingMark; }
            set { m_endingMark = value; }
        }
    }
}
