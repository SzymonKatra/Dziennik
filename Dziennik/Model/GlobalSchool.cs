using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dziennik.Model
{
    [Serializable]
    public class GlobalSchool : ModelBase
    {
        public DateTime YearBeginning { get; set; }
        public DateTime SemesterSeparator { get; set; }
        public DateTime YearEnding { get; set; }
    }
}
