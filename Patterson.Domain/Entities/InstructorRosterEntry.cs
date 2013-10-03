using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Patterson.Domain.Entities
{
    public class InstructorRosterEntry
    {
        [Key, Column(Order = 0)]
        public int InstuctorID { get; set; } // user id of instructor

        [Key, Column(Order = 1)]
        public int ClassID { get; set; }
    }
}
