using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dziennik.Model
{
    [Serializable]
    public class GlobalSchool : ModelBase
    {
        public List<Calendar> Calendars { get; set; }

        public GlobalSchool()
        {
            Calendars = new List<Calendar>();
        }
    }
}
