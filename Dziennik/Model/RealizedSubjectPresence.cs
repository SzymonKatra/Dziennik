using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dziennik.Model
{
    [Serializable]
    public class RealizedSubjectPresence : ModelBase
    {
        public ulong? RealizedSubjectId { get; set; }
    }
}
