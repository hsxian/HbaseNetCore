using System;

namespace HbaseNetCore.Utility
{
    public static class StringUtility
    {
        public static string Reverse2String(this string original)
        {
            char[] arr = original.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }
    }
}