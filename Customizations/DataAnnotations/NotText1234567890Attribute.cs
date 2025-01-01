using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MyCourse.Customizations.DataAnnotations
{
    public class NotText1234567890Attribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if ((string)value != "1234567890")
            {
                return ValidationResult.Success;
            }
            return new ValidationResult(ErrorMessage, new[] { validationContext.MemberName });
        }
    }
}