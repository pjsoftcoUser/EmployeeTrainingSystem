using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace Patterson.Domain.Entities
{
    public class Class
    {
        public int SizeLimit { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int CurrentEnrolled { get; set; } // defaults to 0

        public string Instructor { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int ID { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int CourseID { get; set; } //defaults from course

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string Title { get; set; } // defaults from course, but can be edited
        public string Description { get; set; } // defaults from course, but can be edited
        public string location { get; set; } //defaults to virtual if course is self study

        [HiddenInput(DisplayValue = false)]
        public string CourseNumber { get; set; }//defaults from course, can't edit.

        [HiddenInput(DisplayValue = false)]
        public string CreatedBy { get; set; }
        [HiddenInput(DisplayValue = false)]
        public DateTime? CreatedOn { get; set; }
        [HiddenInput(DisplayValue = false)]
        public string ModifiedBy { get; set; }
        [HiddenInput(DisplayValue = false)]
        public DateTime? ModifiedOn { get; set; }

        public bool OpenRegistration { get; set; }


        public virtual Course Course { get; set; }

        public virtual ICollection<ClassHour> ClassHours { get; set; }
        public virtual ICollection<RosterEntry> roster { get; set; }
        public virtual ICollection<InstructorRosterEntry> InstructorRosterEntries { get; set; }
        [InverseProperty("Class")]
        public virtual ICollection<TestsInClass> TestsInClasses { get; set; }


    }
}
