using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Dziennik.Model
{
    [Serializable]
    public class Semester : ModelBase
    {
        public List<Mark> Marks { get; set; }

        public Semester()
        {
            Marks = new List<Mark>();
        }
    }
}
