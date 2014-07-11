using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Dziennik.Model
{
    public class SchoolGroup
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        public virtual List<StudentInGroup> Students { get; set; }

        public SchoolGroup()
        {
            Students = new List<StudentInGroup>();
        }
    }
}
