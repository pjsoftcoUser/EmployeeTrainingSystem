using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;


namespace Patterson.Domain.Entities
{
    public class Team
    {
        [HiddenInput(DisplayValue = false)]
        public int TeamID { get; set; }

        public string Identifier { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int ManagerID { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int CreatedByID { get; set; }

        [HiddenInput(DisplayValue = false)]
        public DateTime CreatedOn { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int ModifiedByID { get; set; }

        [HiddenInput(DisplayValue = false)]
        public DateTime ModifiedOn { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string CreatedByName { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string ManagerName { get; set; }


    }
}
