using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Patterson.Domain.Entities
{
    public class Skillset
    {
        [HiddenInput(DisplayValue = false)]
        public int ID { get; set; }

        public string Identifier { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int CreatedBy { get; set; }

        [HiddenInput(DisplayValue = false)] //name of creator
        public string CreatedByName { get; set; }
        
        [HiddenInput(DisplayValue = false)]
        public DateTime? CreatedOn { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int ModifiedBy { get; set; }

        [HiddenInput(DisplayValue = false)]
        public DateTime? ModifiedOn { get; set; }

        [HiddenInput(DisplayValue = false)] //name of modifier
        public string ModifiedByName { get; set; }

    }
}
