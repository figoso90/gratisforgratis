using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GratisForGratis.Models.ExtensionMethods
{
    public static class StringExtension
    {

        public static decimal ParseFromHappyCoin(this string value)
        {
            return decimal.Parse(value, System.Globalization.NumberStyles.Currency, (IFormatProvider)HttpContext.Current.Application["numberFormatHappyCoin"]);
        }

        public static decimal ParseFromPayPal(this string value)
        {
            return decimal.Parse(value, System.Globalization.NumberStyles.Currency, System.Globalization.CultureInfo.GetCultureInfo("en-US"));
        }

        public static string Join(this System.Collections.Specialized.NameValueCollection collection, Func<string, string> selector, string separator)
        {
            return String.Join(separator, collection.Cast<string>().Select(e => selector(e)));
        }
    }
}