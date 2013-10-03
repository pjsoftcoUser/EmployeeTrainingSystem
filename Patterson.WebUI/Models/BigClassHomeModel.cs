using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Patterson.Domain.Entities;

namespace Patterson.WebUI.Models
{
    public class BigClassHomeModel
    {
        public IEnumerable<Test> TestToTake { get; set; }
        public IEnumerable<Exam> ExamsTaken { get; set; }
        //      public IEnumerable<String> ExamNames { get; set; }
    }
}