using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace Patterson.Domain.Entities
{
    public class Course
    {
        public string Title { get; set; }
        [HiddenInput(DisplayValue = false)]
        public int CourseID { get; set; }
        public string CourseNumber { get; set; }
        public string Description { get; set; }
        public int Credits { get; set; }
        [HiddenInput(DisplayValue = false)]
        public string CreatedBy { get; set; }
        [HiddenInput(DisplayValue = false)]
        public DateTime? CreatedOn { get; set; }
        [HiddenInput(DisplayValue = false)]
        public string ModifiedBy { get; set; }
        [HiddenInput(DisplayValue = false)]
        public DateTime? ModifiedOn { get; set; }

        public string SkillSet { get; set; }

        public bool Active { get; set; }
        public bool SelfStudy { get; set; }

        public virtual ICollection<Class> Classes { get; set; }
    }
}
