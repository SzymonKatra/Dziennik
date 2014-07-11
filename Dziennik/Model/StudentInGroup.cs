using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Dziennik.Model
{
    public class StudentInGroup
    {
        [Key]
        public int Id { get; set; }

        public virtual GlobalStudent Global { get; set; }
        public int Number { get; set; }
        
        public virtual Semester FirstSemester { get; set; }
        public virtual Semester SecondSemester { get; set; }
        public decimal HalfEndingMark { get; set; }
        public decimal YearEndingMark { get; set; }
    }
}