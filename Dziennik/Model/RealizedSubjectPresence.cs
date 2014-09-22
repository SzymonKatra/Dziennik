using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dziennik.Model
{
    public enum PresenceType
    {
        None,
        Present,
        Absent,
        Late,
        AbsentJustified,
    }

    [Serializable]
    public class RealizedSubjectPresence : ModelBase
    {
        public ulong? RealizedSubjectId { get; set; }

        public PresenceType Presence { get; set; }
        //public bool WasPresent { get; set; }
    }
}
