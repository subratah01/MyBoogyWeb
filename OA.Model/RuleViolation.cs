using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace OA.Model
{
    public class RuleViolation
    {
        [StringLength(4000)]
        [Display(Name ="ErrorMessage")]
        public string ErrorMessage { get; set; }

        [StringLength(50)]
        [Display(Name = "PropertyName")]
        public string PropertyName { get; set; }
        public RuleViolation(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }
        public RuleViolation(string errorMessage, string propertyName)
        {
            ErrorMessage = errorMessage;
            PropertyName = propertyName;
        }
    }
}
