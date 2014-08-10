using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Dziennik.Model
{
    [Serializable]
    public class Mark : ModelBase
    {
        public decimal Value { get; set; }
        public string Note { get; set; }
        public int Weight { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime LastChangeDate { get; set; }
        public string Description { get; set; }
        public ulong? GlobalCategoryId { get; set; }
    }
}
