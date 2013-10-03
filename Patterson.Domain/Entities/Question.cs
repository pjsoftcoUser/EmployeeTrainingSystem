using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Patterson.Domain.Entities
{
    public class Question
    {
        public int ID { get; set; }
        public int TestID { get; set; }
        public string QuestionText { get; set; }

        public virtual ICollection<Answer> Answers { get; set; }
    }
}
