using System;
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
        public List<RealizedSubject> RealizedSubjects { get; set; }

        public SchoolGroup()
        {
            Students = new List<StudentInGroup>();
            RealizedSubjects = new List<RealizedSubject>();
        }
    }
}
