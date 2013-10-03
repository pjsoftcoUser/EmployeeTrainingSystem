using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Collections.Generic;

namespace Patterson.Domain.Entities
{
    public class User
    {
        [HiddenInput(DisplayValue = false)]
        public int id { get; set; }

        [Required]
        [Display(Name = "User name")]
        public string userName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("password", ErrorMessage = "The password and confirmation password do not match.")]
        public string confirmPassword { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int failCount { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string name { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string lastName { get; set; }

        [DataType(DataType.MultilineText)]
        public string address { get; set; }

        [PhoneAttribute]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Telephone")]
        public string telephone { get; set; }

        [EmailAddressAttribute]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string email { get; set; }

        public byte[] ImageData { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string ImageMimeType { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string CreatedBy { get; set; }
        [HiddenInput(DisplayValue = false)]
        public DateTime? CreatedOn { get; set; }
        [HiddenInput(DisplayValue = false)]
        public string ModifiedBy { get; set; }
        [HiddenInput(DisplayValue = false)]
        public DateTime? ModifiedOn { get; set; }

        public static string identity { get; set; }
    }
}
