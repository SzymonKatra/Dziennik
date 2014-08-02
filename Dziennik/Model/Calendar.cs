using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dziennik.Model
{
    [Serializable]
    public class Calendar : ModelBase
    {
        public string Name { get; set; }
        public DateTime YearBeginning { get; set; }
        public DateTime SemesterSeparator { get; set; }
        public DateTime YearEnding { get; set; }
        public List<OffDay> OffDays { get; set; }
    }
}
