using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GratisForGratis.Models.ExtensionMethods
{
    public static class DecimalExtension
    {
        public static string ToHappyCoin(this Decimal value)
        {
            return value.ToString("C", 
                (IFormatProvider)HttpContext.Current.Application["numberFormatHappyCoin"]);
        }
    }
}