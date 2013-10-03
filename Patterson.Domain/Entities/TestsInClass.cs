using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Patterson.Domain.Entities
{
    public class TestsInClass
    {
        [Key, Column(Order = 0), ForeignKey("Test")]
        public int TestID { get; set; }
        [Key, Column(Order = 1), ForeignKey("Class") ]
        public int ClassID { get; set; }

        public int Active { get; set; }
       public virtual Class Class {get;set;}

       public virtual Test Test { get; set; }
    }
}
