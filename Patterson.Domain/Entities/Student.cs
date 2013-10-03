using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Patterson.Domain.Entities
{
   public class Student
    {
        public string Name { get; set; }
        public long ID { get; set; }

        public virtual ICollection<RosterEntry> roster { get; set; }

        public virtual ICollection<User> User { get; set; }
    }
}
