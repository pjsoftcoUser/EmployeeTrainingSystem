using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Patterson.WebUI.Models
{
    public class CourseModel
    {
        public string Name { get; set; }
        public int CourseID { get; set; }
        public int Credits { get; set; }
        public string SkillSet { get; set; }

    }
}