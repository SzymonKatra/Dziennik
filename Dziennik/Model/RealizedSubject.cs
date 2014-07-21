using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dziennik.Model
{
    [Serializable]
    public class RealizedSubject : ModelBase
    {
        public ulong? GlobalSubjectId { get; set; }
    }
}