using System;
using System.ComponentModel;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace HbaseNetCore.Converts
{
    public static class BytesConverter
    {
        /// <summary>
        /// 需要和java对应,也和网络字节对应,所以大小端反转。
        /// </summary>
        /// <param name="@this"></param>
        /// <returns></returns>
        public static byte[] ReverseBytes(this byte[] @this)
        {
            Array.Reverse(@this);
            return @this;
        }
        public static byte[] ToBytes(this object obj)
        {
            if (obj == null)
                return null;
            switch (obj)
            {
                case int @int:
                    return BitConverter.GetBytes(@int).ReverseBytes();
                case string @string:
                    return Encoding.UTF8.GetBytes(@string);
                case double @double:
                    return BitConverter.GetBytes(@double).ReverseBytes();
                case long @long:
                    return BitConverter.GetBytes(@long).ReverseBytes();
                case bool @bool:
                    return BitConverter.GetBytes(@bool);
                case float @float:
                    return BitConverter.GetBytes(@float).ReverseBytes();
                //在Java中char使用Unicode码，是四字节的。但在csharp中为两个字节
                case char @char:
                    return BitConverter.GetBytes(@char);
                case short @short:
                    return BitConverter.GetBytes(@short).ReverseBytes();
                default:
                    return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
            }
        }
        public static T ToObject<T>(this byte[] bits)
        {
            return (T)bits.ToObject(typeof(T));
        }
        public static object ToObject(this byte[] bits, Type type)
        {
            if (bits?.Length < 1)
                return null;
            switch (type.Name)
            {
                case nameof(Int32):
                    return BitConverter.ToInt32(bits.ReverseBytes(), 0);
                case nameof(String):
                    return Encoding.UTF8.GetString(bits);
                case nameof(Double):
                    return BitConverter.ToDouble(bits.ReverseBytes(), 0);
                case nameof(Int64):
                    return BitConverter.ToInt64(bits.ReverseBytes(), 0);
                case nameof(Boolean):
                    return BitConverter.ToBoolean(bits, 0);
                case nameof(Single):
                    return BitConverter.ToSingle(bits.ReverseBytes(), 0);
                case nameof(Char):
                    return BitConverter.ToChar(bits, 0);
                case nameof(Int16):
                    return BitConverter.ToInt16(bits.ReverseBytes(), 0);
                default:
                    var str = Encoding.UTF8.GetString(bits);
                    if (string.IsNullOrWhiteSpace(str)) return null;
                    return JsonConvert.DeserializeObject(str, type);
            }
        }
        #region objec

        // public static string ToUTF8String(this byte[] @this)
        // {
        //     return Encoding.UTF8.GetString(@this);
        // }

        #endregion
    }
}