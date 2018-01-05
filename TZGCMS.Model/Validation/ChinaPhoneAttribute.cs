using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using TZGCMS.Resources.Admin;

namespace TZGCMS.Model.Validation
{
    public class ChinaPhoneAttribute : ValidationAttribute, IClientValidatable
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string phone = value.ToString();

                if (Regex.IsMatch(phone, @"^(([0-9]{3,4})|[0-9]{3,4}-)?[0-9]{7,8}$", RegexOptions.IgnoreCase))
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
       $.validator.addMethod('chinaphone', function(value, element)
       {
           if(value.length>0)
                return/^(([0-9]{3,4})|[0-9]{3,4}-)?[0-9]{7,8}$/.test(value);
           return true;      
       });

         $.validator.unobtrusive.adapters.addBool("chinaphone");

       */
        //new method
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            yield return new ModelClientValidationRule()
            {
                ValidationType = "chinaphone",
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName())
            };

        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(Messages.CheckFormat, name);
        }
    }
}
