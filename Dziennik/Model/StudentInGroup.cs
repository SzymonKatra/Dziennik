using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dziennik.Model
{
    [Serializable]
    public sealed class StudentInGroup
    {
        private int m_globalId;
        public int GlobalId
        {
            get { return m_globalId; }
            set { m_globalId = value; }
        }

        private int m_id;
        public int Id
        {
            get { return m_id; }
            set { m_id = value; }
        }

        private Semester m_firstSemester = new Semester();
        public Semester FirstSemester
        {
            get { return m_firstSemester; }
            set { m_firstSemester = value; }
        }

        private Semester m_secondSemester = new Semester();
        public Semester SecondSemester
        {
            get { return m_secondSemester; }
            set { m_secondSemester = value; }
        }

        private decimal m_yearEndingMark;
        public decimal YearEndingMark
        {
            get { return m_yearEndingMark; }
            set { m_yearEndingMark = value; }
        }
    }
}