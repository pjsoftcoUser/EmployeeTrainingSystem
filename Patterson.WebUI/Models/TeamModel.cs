using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Patterson.WebUI.Models
{
    public class TeamModel 
    {
        public int TeamID { get; set; }
        public string Identifier { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int ManagerID { get; set; }
        public int CreatedByID { get; set; }
        public DateTime CreatedOn { get; set; }
        public int ModifiedByID { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string CreatedByName { get; set; }
        public string ManagerName { get; set; }
    }
}
