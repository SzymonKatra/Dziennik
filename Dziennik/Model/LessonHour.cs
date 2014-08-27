using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dziennik.Model
{
    public class LessonHour : ModelBase
    {
        public int Number { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
