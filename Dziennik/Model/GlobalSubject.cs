using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dziennik.Model
{
    [Serializable]
    public class GlobalSubject : ModelBase
    {
        public int Number { get; set; }
        public string Name { get; set; }
    }
}
