using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Patterson.Domain.Entities
{
   public class ExamRecord
    {
        [Key, Column(Order = 0)]
        public int ExamID { get; set; }
        [Key, Column(Order = 1),ForeignKey("Question")]
        public int QuestionID { get; set; }

        public int answer { get; set; }

        public virtual Question Question { get; set; }

  
    }
}
