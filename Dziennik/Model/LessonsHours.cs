using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dziennik.Model
{
    public class LessonsHours : ModelBase
    {
        public List<LessonHour> Hours { get; set; }

        public LessonsHours()
        {
            Hours = new List<LessonHour>();
        }
    }
}
