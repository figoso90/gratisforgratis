using GratisForGratis.App_GlobalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;

namespace GratisForGratis.DataAnnotations
{
    public class RangeIntAdvanced : RangeAttribute, IClientValidatable
    {
        public RangeIntAdvanced(string min, string max)
            : base(int.Parse(WebConfigurationManager.AppSettings[min]), int.Parse(WebConfigurationManager.AppSettings[max]))
        {
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = ErrorMessage,
                ValidationType = "range"
            };

            rule.ValidationParameters.Add("min", Minimum);
            rule.ValidationParameters.Add("max", Maximum);

            yield return rule;
        }
    }

    public class RangeDoubleAdvanced : RangeAttribute, IClientValidatable
    {
        public RangeDoubleAdvanced(string min, string max)
            : base(double.Parse(WebConfigurationManager.AppSettings[min]), double.Parse(WebConfigurationManager.AppSettings[max]))
        {
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = ErrorMessage,
                ValidationType = "range"
            };

            rule.ValidationParameters.Add("min", Minimum);
            rule.ValidationParameters.Add("max", Maximum);

            yield return rule;
        }
    }

    public class RangeDate : RangeAttribute
    {
        public RangeDate()
          : base(typeof(DateTime),
                  DateTime.MinValue.ToShortDateString(),
                  DateTime.Now.ToShortDateString())
        { }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = ErrorMessage,
                ValidationType = "range"
            };

            rule.ValidationParameters.Add("min", Minimum);
            rule.ValidationParameters.Add("max", Maximum);

            yield return rule;
        }
    }

    public class RangeTime : RequiredAttribute //ValidationAttribute
    {
        #region ATTRIBUTI
        private string _propertyName;
        #endregion

        #region COSTRUTTORI
        public RangeTime() { }

        public RangeTime(string propertyName)
        {
            this._propertyName = propertyName;
        }
        #endregion

        #region METODI PUBBLICI
        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            try {
                if (value != null)
                {
                    DateTime data = Convert.ToDateTime(value);
                    if (!string.IsNullOrWhiteSpace(this._propertyName))
                    {
                        Object instance = context.ObjectInstance;
                        Type type = instance.GetType();
                        Object propertyvalue = type.GetProperty(this._propertyName).GetValue(instance, null);
                        if (propertyvalue != null)
                        {
                            DateTime dataFine = Convert.ToDateTime(propertyvalue);
                            double oreApertura = (dataFine - data).TotalHours;
                            if (oreApertura <= 0)
                                throw new Exception(ExceptionMessage.OpenedHour);
                        }
                    }
                }
            }
            catch(Exception eccezione)
            {
                return new ValidationResult(eccezione.Message);
            }
            return ValidationResult.Success;

        }
        #endregion
    }

    public class RangeYearPaymentCard : RangeAttribute
    {
        public RangeYearPaymentCard()
          : base(typeof(int),
                  DateTime.Now.Year.ToString(),
                  DateTime.Now.AddYears(20).Year.ToString())
        { }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = ErrorMessage,
                ValidationType = "range"
            };

            rule.ValidationParameters.Add("min", Minimum);
            rule.ValidationParameters.Add("max", Maximum);

            yield return rule;
        }
    }
}
