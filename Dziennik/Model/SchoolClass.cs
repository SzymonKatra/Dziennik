using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Dziennik.Model
{
    public class SchoolClass
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        public virtual List<GlobalStudent> Students { get; set; }
        public virtual List<SchoolGroup> Groups { get; set; }

        public SchoolClass()
        {
            Students = new List<GlobalStudent>();
            Groups = new List<SchoolGroup>();
        }
    }
}