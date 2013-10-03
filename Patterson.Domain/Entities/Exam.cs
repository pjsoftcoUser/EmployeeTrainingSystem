using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace Patterson.Domain.Entities
{
    public class Exam
    {
        [Key, Column(Order = 0)]
        //,ForeignKey("roster")]
        public int RosterID { get; set; }
        [Key, Column(Order = 1), ForeignKey("Test")]
        public int TestID { get; set; }
        /* public int QuestionID { get; set; }
         public string Answer { get; set; } */

        public int score { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ExamID { get; set; }


        //public virtual ICollection<ExamRecord> ExamRecords { get; set; }

 
        public virtual Test Test { get; set; }
        
       // public virtual ICollection<ExamRecord> ExamRecords { get; set; }
       // public virtual RosterEntry roster { get; set; }


    }
}
