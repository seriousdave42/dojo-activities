using System.ComponentModel.DataAnnotations;
using System;
namespace BeltExam.Models
{
    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if ((DateTime)value <= DateTime.Now)
            {
                return new ValidationResult("Activity start time must be in the future");
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }
}