using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Dziennik.Model
{
    [Serializable]
    public class StudentInGroup : ModelBase
    {
        public ulong? GlobalStudentId { get; set; }
        public int Number { get; set; }
        
        public Semester FirstSemester { get; set; }
        public Semester SecondSemester { get; set; }
        public decimal HalfEndingMark { get; set; }
        public decimal YearEndingMark { get; set; }
        public List<RealizedSubjectPresence> Presence { get; set; }

        public StudentInGroup()
        {
            FirstSemester = new Semester();
            SecondSemester = new Semester();
        }
    }
}