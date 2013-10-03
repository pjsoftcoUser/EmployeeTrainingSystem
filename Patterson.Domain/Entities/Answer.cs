using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Patterson.Domain.Entities
{
    public class Answer
    {
        public int ID { get; set; }
        public int QuestionID { get; set; }
        public string AnswerText { get; set; }
        public int Correctness { get; set; }
    }
}
