using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Patterson.Domain.Entities
{
    public class SMTP
    {
        [HiddenInput(DisplayValue = false)]
        public int SMTPid { get; set; }

        [Required(ErrorMessage = "Please enter a server")]
        public string server { get; set; }

        [Required(ErrorMessage = "Please enter a port")]
        public int port { get; set; }

        [Required(ErrorMessage = "Please enter a user")]
        public string user { get; set; }

        [Required(ErrorMessage = "Please enter a password")]
        public string password { get; set; }
    }
}
