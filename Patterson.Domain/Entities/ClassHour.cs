using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.ComponentModel;

namespace Patterson.Domain.Entities
{
    public class ClassHour
    {
        public ClassHour()
        {
            location = "Virtual";
        }

        [HiddenInput(DisplayValue = false)]
        public int classId { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int id { get; set; }

        [HiddenInput(DisplayValue = false)]
        
        public string day { get; set; }
        public string location { get; set; }

        public TimeSpan startTime { get; set; }
        public TimeSpan endTime { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string CreatedBy { get; set; }
        [HiddenInput(DisplayValue = false)]
        public DateTime? CreatedOn { get; set; }
        [HiddenInput(DisplayValue = false)]
        public string ModifiedBy { get; set; }
        [HiddenInput(DisplayValue = false)]
        public DateTime? ModifiedOn { get; set; }

        public virtual Class Class { get; set; }

        public virtual ICollection<Attendance> Attendances { get; set; }
    }
}
