using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using TZGCMS.Resources.Admin;

namespace SIG.Model.Validation
{
    public class CNAndENAttribute : ValidationAttribute, IClientValidatable
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string cnanden = value.ToString();

                if (Regex.IsMatch(cnanden, @"^[\u4e00-\u9fa5_a-zA-Z]+$", RegexOptions.IgnoreCase))
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult(string.Format(Messages.CheckFormat, validationContext.DisplayName));
                }
            }
            return ValidationResult.Success;
        }

        /* Javascript client 只有在type="text"时才有效
       $.validator.addMethod('cnanden', function(value, element)
       {
           if(value.length>0)
                return/^[\u4e00-\u9fa5_a-zA-Z]+$/.test(value);
           return true;      
       });

         $.validator.unobtrusive.adapters.addBool("cnanden");

       */
        //new method
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            yield return new ModelClientValidationRule()
            {
                ValidationType = "cnanden",
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName())
            };

        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(Messages.CheckFormat, name);
        }
    }
}
