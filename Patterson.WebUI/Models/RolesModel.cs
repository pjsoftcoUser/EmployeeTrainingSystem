using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace Patterson.WebUI.Models
{
    public class RolesModel
    {
        [DisplayName("Roles")]
        public string[] Roles
        {
            get;
            set;
        }
    }
}