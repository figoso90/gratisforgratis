using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GratisForGratis.DataAnnotations
{
    public class AnnuncioCompletoAttribute : ValidationAttribute
    {
        public AnnuncioCompletoAttribute() { }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            return ValidationResult.Success;
        }
    }
}
