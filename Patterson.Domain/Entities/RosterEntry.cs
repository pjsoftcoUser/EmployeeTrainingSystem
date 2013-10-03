using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace Patterson.Domain.Entities
{
   public class RosterEntry
    {  
       [Key, Column(Order=0)]
       public int StudentID { get; set; }
       [Key, Column(Order = 1)]
       public int ClassID { get; set; }
       [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
       public int RosterID { get; set; }

       
      // public virtual ICollection<Exam> Exams { get; set; }
     /* public virtual Class Class { get; set; }

      public virtual User User { get; set; }*/
    }
}
