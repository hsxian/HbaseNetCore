using System.Text;

namespace Utilities.Converts
{
    public static class ByteConverts
    {
        public static byte[] ToUTF8Bytes(this string @this)
        {
            return Encoding.UTF8.GetBytes(@this);
        }

        public static string ToUTF8String(this byte[] @this)
        {
            return Encoding.UTF8.GetString(@this);
        }
    }
}