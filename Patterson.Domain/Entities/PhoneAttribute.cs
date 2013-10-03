using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Text.RegularExpressions;

namespace Patterson.Domain.Entities
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class PhoneAttribute : DataTypeAttribute, IClientValidatable
    {
        private static Regex _regex = new Regex(@"^(?:(?:\+?1\s*(?:[.-]\s*)?)?(?:(\s*([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9]‌​)\s*)|([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9]))\s*(?:[.-]\s*)?)?([2-9]1[02-‌​9]|[2-9][02-9]1|[2-9][02-9]{2})\s*(?:[.-]\s*)?([0-9]{4})$", RegexOptions.Compiled | RegexOptions.IgnoreCase);


        public PhoneAttribute()
            : base(DataType.PhoneNumber)
        {
            ErrorMessage = "Invalid Phone Number";
        }


        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            yield return new ModelClientValidationRule
            {
                ValidationType = "telephone",
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName())
            };
        }
    }
}
