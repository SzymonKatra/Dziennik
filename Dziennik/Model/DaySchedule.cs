using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dziennik.Model
{
    [Serializable]
    public class DaySchedule : ModelBase
    {
        //public int HoursCount { get; set; }
        public List<SelectedHour> HoursSchedule { get; set; }

        public DaySchedule()
        {
            HoursSchedule = new List<SelectedHour>();
        }
    }
}
