using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Utility;

namespace eIVOGo.Models.ViewModel
{
    public class DateLessThanAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public DateLessThanAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ErrorMessage = ErrorMessageString;
            //wait to do...判斷空值
            if (value == null)
                return ValidationResult.Success;

            var currentValue = (DateTime)value;

            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

            if (property == null)
                throw new ArgumentException("Property with this name not found");

            var comparisonValue = (DateTime)property.GetValue(validationContext.ObjectInstance);

            return (currentValue > comparisonValue) ? new ValidationResult(ErrorMessage) : ValidationResult.Success;

        }
    }
}