using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using TZGCMS.Resources.Admin;

namespace TZGCMS.Model.Validation
{
    public class ChinaMobileAttribute : ValidationAttribute, IClientValidatable
    {
      
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string email = value.ToString();

                if (Regex.IsMatch(email, @"^0{0,1}(13[0-9]|15[7-9]|153|156|170|178|18[0-9])[0-9]{8}$", RegexOptions.IgnoreCase))
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
       $.validator.addMethod('chinamobile', function(value, element)
       {
           return/^0{0,1}(13[0-9]|15[7-9]|153|156|170|178|18[0-9])[0-9]{8}$/.test(value)           
       });

         $.validator.unobtrusive.adapters.addBool("chinamobile");

       */
        //new method
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            yield return new ModelClientValidationRule()
            {
                ValidationType = "chinamobile",
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName())
            };

        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(Messages.CheckFormat,name);
        }
    }
}