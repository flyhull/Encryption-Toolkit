using System.Security.Cryptography;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Globalization;
using System.Text;

namespace Common_Support
{
    public static class CommonSupport
    {
        public const Int32 PracticalInt32Max = Int32.MaxValue - 512;
        public const Int32 PracticalInt32Min = 1;

        public static string GetRandomString(int minLen, int extraLen)
        {
            string result = string.Empty;

            StringBuilder sb = new StringBuilder();
            {
                Random rand = new Random((Int32)(DateTime.UtcNow.Ticks % Int32.MaxValue));

                int stringlen = rand.Next(minLen, minLen + extraLen);
                for (int i = 0; i < stringlen; i++)
                {
                    sb.Append(Convert.ToChar(rand.Next(0, 26) + 65));
                }
            }

            result = sb.ToString();

            return result;
        }
        public static void RedactByteArray(ref byte[] bytes, bool truncate = true)
        {
            if (bytes == null)
            {
                // null, nothing to redact
            }
            else
            {
                if (bytes.Length > 0)
                {
                    for (int i = 0; i < bytes.Length; i++) bytes[i] = 0;
                    if (truncate)
                    {
                        bytes = Array.Empty<byte>();
                    }
                }
                else
                {
                    // already truncated
                }
            } 
        }
    }
}
