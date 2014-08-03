﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Dziennik.Model
{
    [Serializable]
    public class SchoolGroup : ModelBase
    {
        public string Name { get; set; }
        public List<StudentInGroup> Students { get; set; }
        public List<GlobalSubject> Subjects { get; set; }
        public List<RealizedSubject> RealizedSubjects { get; set; }
        public WeekSchedule Schedule { get; set; }

        public SchoolGroup()
        {
            Students = new List<StudentInGroup>();
            Subjects = new List<GlobalSubject>();
            RealizedSubjects = new List<RealizedSubject>();
            Schedule = new WeekSchedule();
        }
    }
}
