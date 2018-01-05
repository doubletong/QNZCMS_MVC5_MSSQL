using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using TZGCMS.Resources.Admin;

namespace TZGCMS.Model.Validation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class IdentityCardAttribute : ValidationAttribute, IClientValidatable
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string idCard = value.ToString();

                if (Regex.IsMatch(idCard, @"^[1-9][0-9]{5}((19[0-9]{2})|(200[0-9])|2011)(0[1-9]|1[0-2])(0[1-9]|[12][0-9]|3[01])[0-9]{3}[0-9xX]$", RegexOptions.IgnoreCase)
                    || Regex.IsMatch(idCard, @"^[1-9][0-9]{5}([0-9]{2})(0[1-9]|1[0-2])(0[1-9]|[12][0-9]|3[01])[0-9]{3}$", RegexOptions.IgnoreCase))
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
       $.validator.addMethod('identitycard', function(value, element)
       {
           return (/^[1-9][0-9]{5}((19[0-9]{2})|(200[0-9])|2011)(0[1-9]|1[0-2])(0[1-9]|[12][0-9]|3[01])[0-9]{3}[0-9xX]$/.test(value) || /^[1-9][0-9]{5}([0-9]{2})(0[1-9]|1[0-2])(0[1-9]|[12][0-9]|3[01])[0-9]{3}$/.test(value)            
       });

         $.validator.unobtrusive.adapters.addBool("identitycard");

       */
        //new method
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            yield return new ModelClientValidationRule()
            {
                ValidationType = "identitycard",
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName())
            };

        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(Messages.CheckFormat, name);
        }



    }
}