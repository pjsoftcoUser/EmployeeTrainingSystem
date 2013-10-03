using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Patterson.WebUI.Models
{
    public class ClassModel 
    {
        public int SizeLimit { get; set; }
        public string Instructor { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ClassTitle { get; set; } // defaults from course, but can be edited
        public string Description { get; set; } // defaults from course, but can be edited
        public string location { get; set; } //defaults to virtual if course is self study
        public bool OpenRegistration { get; set; }

    }
}
