using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace FollowYourSelfMobile.Models
{
    public static class Enums
    {
        public enum ActivityTypes
        {
            [Description("Sayı")]
            Sayi,
            [Description("Yapıldı/Yapılmadı")]
            YapildiYapilmadi
        }

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                    typeof(DescriptionAttribute),
                    false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
       
    }
}
