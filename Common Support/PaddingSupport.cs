using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common_Support
{

    public static class PaddingSupport
    {
        #region "functions used to add and remove a random pad to or from a byte array"
        internal static Byte[] BuildPadding(Int32 padLength)
        {

            byte[] result = new byte[padLength];

            result.Initialize();

            Int32 seed = (Int32)(DateTime.Now.Ticks % Int32.MaxValue);

            Random rng = new Random(seed);

            while (result[0].Equals(Byte.MinValue))
            {
                rng.NextBytes(result);
            }

            result[-1 + result.Length] = result[0];


            Int64 i = 1;

            while (i < (-1 + result.Length))
            {
                if (result[i].Equals(result[0]))
                {
                    result[i] = byte.MinValue;
                }
                i++;
            }

            return result;

        }
        public static ResultObject PadBytes(byte[] bytes, Int64 pixels, Int32 bytesPerPixel)
        {
            // TODO - PROTECT FROM SIZE OVERFLOW
            ResultObject result = new ResultObject();

            //Console.WriteLine("Payload is " + bytes.Length.ToString() + " bytes long and starts with " + BitConverter.ToString(bytes.Take<byte>(10).ToArray<byte>()) + " and ends with " + BitConverter.ToString(bytes.TakeLast<byte>(10).ToArray<byte>()));

            Int32 totalBytesNeeded = (Int32)pixels * bytesPerPixel;

            Int32 bytesNeeded = totalBytesNeeded - bytes.Length;

            //Console.WriteLine(totalBytesNeeded.ToString() + " bytes are needed which will be a " + bytesNeeded.ToString() + " byte pad followed by a " + bytes.Length.ToString() + " byte payload");

            byte[] pad = BuildPadding(bytesNeeded);

            //Console.WriteLine("Pad is " + pad.Length.ToString() + " bytes long and starts with " + BitConverter.ToString(pad.Take<byte>(10).ToArray<byte>()) + " and ends with " + BitConverter.ToString(pad.TakeLast<byte>(10).ToArray<byte>()));

            byte[] intermediateResult = new byte[totalBytesNeeded];

            Buffer.BlockCopy(pad, 0, intermediateResult, 0, (int)bytesNeeded);

            Buffer.BlockCopy(bytes, 0, intermediateResult, (int)bytesNeeded, bytes.Length);

            //Console.WriteLine("Padded payload is " + intermediateResult.Length.ToString() + " bytes long and starts with " + BitConverter.ToString(intermediateResult.Take<byte>(10).ToArray<byte>()) + " and ends with " + BitConverter.ToString(intermediateResult.TakeLast<byte>(10).ToArray<byte>()));

            result = new ResultObject(intermediateResult);

            return result;
        }
        public static Byte[] UnPadBytes(byte[] paddedBytes)
        {
            byte[] result = Array.Empty<byte>();

            byte firstAndLast = paddedBytes[0];

            Int32 padLength = 1 + Array.IndexOf<byte>(paddedBytes, firstAndLast, 1);

            Int32 payloadLength = paddedBytes.Length - padLength;

            result = paddedBytes.TakeLast<byte>(payloadLength).ToArray<byte>();

            //Console.WriteLine("Recovered and discarded pad is " + padLength.ToString() + " long and starts with " + BitConverter.ToString(paddedBytes.Take<byte>(10).ToArray<byte>()) + " and ends with " + BitConverter.ToString(paddedBytes.Skip(-10 + padLength).Take<byte>(10).ToArray<byte>()));

            //Console.WriteLine("Recovered payload is " + result.Length.ToString() + " long and starts with " + BitConverter.ToString(result.Take<byte>(10).ToArray<byte>()) + " and ends with " + BitConverter.ToString(result.TakeLast<byte>(10).ToArray<byte>()));

            return result;

        }

        #endregion
    }
}
