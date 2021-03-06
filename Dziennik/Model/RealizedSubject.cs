﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dziennik.Model
{
    [Serializable]
    public class RealizedSubject : ModelBase
    {
        public ulong? GlobalSubjectId { get; set; }

        public int RealizedHour { get; set; }
        public DateTime RealizedDate { get; set; }
        public string CustomSubject { get; set; }
    }
}