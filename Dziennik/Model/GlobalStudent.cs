using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Dziennik.Model
{
    public class GlobalStudent
    {
        [Key]
        public int Id { get; set; }

        public int Number { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string AdditionalInformation { get; set; }
    }
}
