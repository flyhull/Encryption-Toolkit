using Common_Support;
using System.Text;

namespace Testing_Support
{
    public static class TestingSupport
    {
        public const string intermediateFolder = "C:\\Users\\Admin\\source\\repos\\Encryption Tool Kit\\Testing Support\\PngFiles\\";

        public static ResultObject GetHashOfBytes(byte[] bytes)  
        {
            ResultObject result = new ResultObject();
            if (bytes.Length > 0)
            {
                result = new ResultObject(BitConverter.ToString(System.Security.Cryptography.MD5.HashData(bytes)));
            }
            return result;
        }

        public static ResultObject GetHashOfFile(string fileName)
        {
            ResultObject result = new ResultObject();
            FileInfo fi = new FileInfo(fileName);
            if (fi.Exists && fi.Length > 0)
            {
                result = new ResultObject(BitConverter.ToString(System.Security.Cryptography.MD5.HashData(File.ReadAllBytes(fileName))));
            }
            return result;
        }

        public static Byte[] GetRandomNumberOfRandomBytes(Int64 AtLeastThisMany, Int64 NotMoreThan)
        {
            byte[] result = Array.Empty<byte>();
            if (AtLeastThisMany > 0 && NotMoreThan > AtLeastThisMany)
            {
                Random rnd = new Random((Int32)(DateTime.UtcNow.Ticks % Int32.MaxValue));
                Int64 desiredLength = rnd.NextInt64(AtLeastThisMany, NotMoreThan);
                result = new byte[desiredLength];
                rnd.NextBytes(result);
            }
            return result;

        }

        public static Byte[] GetRandomBytes(Int64 ThisMany)
        {
            byte[] result = Array.Empty<byte>();
            if (ThisMany > 0 )
            {
                Random rnd = new Random((Int32)(DateTime.UtcNow.Ticks % Int32.MaxValue));
                result = new byte[ThisMany];
                rnd.NextBytes(result);
            }
            return result;

        }

        public static Int32 GetRandomInt(int AtLeast, int NotBiggerThan)
        {            
            Random rnd = new Random((Int32)(DateTime.UtcNow.Ticks % Int32.MaxValue));
            return rnd.Next(AtLeast, NotBiggerThan);
        }


        public static string GetRandomString(int AtLeast, int NotBiggerThan)
        {
            string result = string.Empty;

            StringBuilder sb = new StringBuilder();
            {
                Random rand = new Random((Int32)(DateTime.UtcNow.Ticks % Int32.MaxValue));

                int stringlen = rand.Next(AtLeast, NotBiggerThan);
                for (int i = 0; i < stringlen; i++)
                {
                    sb.Append(Convert.ToChar(rand.Next(32, 126)));
                }
            }

            result = sb.ToString();

            return result;
        }


    }
}
