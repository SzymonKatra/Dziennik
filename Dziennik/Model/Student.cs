﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dziennik.Model
{
    public class Student
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