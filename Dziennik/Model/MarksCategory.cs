using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dziennik.Model
{
    [Serializable]
    public class MarksCategory : ModelBase
    {
        public string Name { get; set; }
        public Color Color { get; set; }
    }
}
