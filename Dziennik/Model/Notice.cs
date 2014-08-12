using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dziennik.Model
{
    public class Notice : ModelBase
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan NotifyIn { get; set; }
    }
}
