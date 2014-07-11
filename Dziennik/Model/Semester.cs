using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Dziennik.Model
{
    public class Semester
    {
        [Key]
        public int Id { get; set; }

        public int Dummy { get; set; }
        public virtual List<Mark> Marks { get; set; }

        public Semester()
        {
            Marks = new List<Mark>();
        }
    }
}
