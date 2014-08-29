using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dziennik.Model
{
    [Serializable]
    public class LessonsHours : ModelBase
    {
        public List<LessonHour> Hours { get; set; }
        public bool IsEnabled { get; set; }

        public LessonsHours()
        {
            Hours = new List<LessonHour>();
        }
    }
}
