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

        public SchoolClass()
        {
            Students = new List<GlobalStudent>();
            Groups = new List<SchoolGroup>();
        }
    }
}