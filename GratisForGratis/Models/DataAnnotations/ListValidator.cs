using GratisForGratis.App_GlobalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GratisForGratis.Models.DataAnnotations
{
    public class ListValidator : ValidationAttribute, IClientValidatable
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            List<string> list = (List<string>)value;

            if (list.Count <= 0)
                return new ValidationResult(Language.ErrorRequiredPhote);

            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            ModelClientValidationRule mcvrTwo = new ModelClientValidationRule();
            mcvrTwo.ValidationType = "listvalidator";
            mcvrTwo.ErrorMessage = Language.ErrorRequiredPhote;
            mcvrTwo.ValidationParameters.Add
            ("param", "Foto");
            return new List<ModelClientValidationRule> { mcvrTwo };
        }
    }
}
