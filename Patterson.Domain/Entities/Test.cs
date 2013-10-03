using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace Patterson.Domain.Entities
{
    public class Test
    {
        [HiddenInput(DisplayValue = false)]
        public int ID { get; set; }
        public string Title { get; set; }
        public string Identifier { get; set; }
        public int TimeLimit { get; set; }
        public int PassingScore { get; set; }
        [HiddenInput(DisplayValue = false)]
        public int PointValue { get; set; }
        [HiddenInput(DisplayValue = false)]
        public int PassingPercentage { get; set; }
        [HiddenInput(DisplayValue = false)]
        public string CreatedBy { get; set; }
        [HiddenInput(DisplayValue = false)]
        public DateTime CreatedOn { get; set; }
        [HiddenInput(DisplayValue = false)]
        public string ModifiedBy { get; set; }
        [HiddenInput(DisplayValue = false)]
        public DateTime ModifiedOn { get; set; }

        

        public virtual ICollection<Question> Questions { get; set; }
      
        public virtual ICollection<TestsInClass> TestsInClasses { get; set; }
    }
}
