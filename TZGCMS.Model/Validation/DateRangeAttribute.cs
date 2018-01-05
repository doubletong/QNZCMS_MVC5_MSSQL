using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace TZGCMS.Model.Validation
{
    public class DateRangeAttribute : ValidationAttribute, IClientValidatable
    {
        private const string DateFormat = "yyyy/MM/dd";
        private const string DefaultErrorMessage = "'{0}' must be a date between {1:d} and {2:d}.";

        public DateTime MinDate { get; set; }
        public DateTime MaxDate { get; set; }

        public DateRangeAttribute(string minDate, string maxDate)
            : base(DefaultErrorMessage)
        {
            MinDate = ParseDate(minDate);
            MaxDate = ParseDate(maxDate);
        }

        public override bool IsValid(object value)
        {
            if (value == null || !(value is DateTime))
            {
                return true;
            }
            DateTime dateValue = (DateTime)value;
            return MinDate <= dateValue && dateValue <= MaxDate;
        }
        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture, ErrorMessageString,
                name, MinDate, MaxDate);
        }

        private static DateTime ParseDate(string dateValue)
        {
            return DateTime.ParseExact(dateValue, DateFormat, CultureInfo.InvariantCulture);
        }
        /*
        (function ($) {
    // The validator function 只有在type="text"时才有效
    $.validator.addMethod('rangeDate', function (value, element, param) {
        if (!value) {
            return true; // not testing 'is required' here!
        }
        try {
            var dateValue = $.datepicker.parseDate("dd/mm/yy", value); 
        }
        catch (e) {
            return false;
        }
        return param.min <= dateValue && dateValue <= param.max;
    });
 
    // The adapter to support ASP.NET MVC unobtrusive validation
    $.validator.unobtrusive.adapters.add('rangedate', ['min', 'max'], function (options) {
        var params = {
            min: $.datepicker.parseDate("yy/mm/dd", options.params.min),
            max: $.datepicker.parseDate("yy/mm/dd", options.params.max)
        };
 
        options.rules['rangeDate'] = params;
        if (options.message) {
            options.messages['rangeDate'] = options.message;
        }
    });
} (jQuery));

    */

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            return new[]
            {
                new ModelClientValidationRangeDateRule(
                     FormatErrorMessage(metadata.GetDisplayName()), MinDate, MaxDate)
            };
        }
    }

    public class ModelClientValidationRangeDateRule : ModelClientValidationRule
    {
        public ModelClientValidationRangeDateRule(string errorMessage, DateTime minValue, DateTime maxValue)
        {
            ErrorMessage = errorMessage;
            ValidationType = "rangedate";

            ValidationParameters["min"] = minValue.ToString("yyyy/MM/dd");
            ValidationParameters["max"] = maxValue.ToString("yyyy/MM/dd");
        }
    }
}
