using System;
using System.ComponentModel;
using System.Text;
using Newtonsoft.Json;

namespace Utilities.Converts
{
    public static class UTF8BytesConverter
    {
        #region object
        public static byte[] ToUTF8Bytes(this object @this)
        {
            if (@this is string str)
            {
                return Encoding.UTF8.GetBytes(str);
            }
            var json = JsonConvert.SerializeObject(@this);
            return Encoding.UTF8.GetBytes(json);
        }

        public static string ToUTF8String(this byte[] @this)
        {
            return Encoding.UTF8.GetString(@this);
        }
        public static T ToReal<T>(this byte[] @this)
        {
            var utf8str = Encoding.UTF8.GetString(@this);
            if (typeof(string) == typeof(T))
            {
                return (T)(object)utf8str;
            }
            else if (typeof(int) == typeof(T) && int.TryParse(utf8str, out int resint))
            {
                return (T)(object)resint;
            }
            else if (typeof(double) == typeof(T) && double.TryParse(utf8str, out double resdbl))
            {
                return (T)(object)resdbl;
            }
            else if (typeof(bool) == typeof(T) && bool.TryParse(utf8str, out bool resbol))
            {
                return (T)(object)resbol;
            }
            return default(T);
        }
        #endregion

        public static TDestination Convert<TSource, TDestination>(TSource value)
        {
            var converter = TypeDescriptor.GetConverter(typeof(TSource));

            TDestination result = default(TDestination);

            // 判斷能不能轉型
            if (converter.CanConvertTo(typeof(TDestination)))
            {
                result = (TDestination)(converter.ConvertTo(value, typeof(TDestination)));
            }

            return result;
        }
    }
}