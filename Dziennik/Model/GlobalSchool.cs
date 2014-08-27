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
        public List<MarksCategory> MarksCategories { get; set; }
        public List<Notice> Notices { get; set; }
        public DateTime LastArchivedDate { get; set; }
        public LessonsHours Hours { get; set; }

        public GlobalSchool()
        {
            Calendars = new List<Calendar>();
            MarksCategories = new List<MarksCategory>();
            Notices = new List<Notice>();
            Hours = new LessonsHours();
        }
    }
}
