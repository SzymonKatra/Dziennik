using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dziennik.Model
{
    [Serializable]
    public class WeekSchedule : ModelBase
    {
        public DateTime StartDate { get; set; }
        public DaySchedule Monday { get; set; }
        public DaySchedule Tuesday { get; set; }
        public DaySchedule Wednesday { get; set; }
        public DaySchedule Thursday { get; set; }
        public DaySchedule Friday { get; set; }

        public WeekSchedule()
        {
            Monday = new DaySchedule();
            Tuesday = new DaySchedule();
            Wednesday = new DaySchedule();
            Thursday = new DaySchedule();
            Friday = new DaySchedule();
        }
    }
}
