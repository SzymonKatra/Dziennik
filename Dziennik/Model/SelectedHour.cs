using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dziennik.Model
{
    [Serializable]
    public class SelectedHour : ModelBase
    {
        public int Hour { get; set; }
        public string Room { get; set; }
    }
}
