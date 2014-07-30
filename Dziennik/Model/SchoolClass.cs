using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Dziennik.Model
{
    [Serializable]
    public class SchoolClass : ModelBase
    {
        public string Name { get; set; }
        public List<GlobalStudent> Students { get; set; }
        public List<SchoolGroup> Groups { get; set; }
        public DateTime YearBeginning { get; set; }
        public DateTime SemesterSeparator { get; set; }
        public DateTime YearEnding { get; set; }

        public SchoolClass()
        {
            Students = new List<GlobalStudent>();
            Groups = new List<SchoolGroup>();
        }
    }
}