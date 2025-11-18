using Common_Support;
using Facade_Support;
using Image_Support;
using MimeDetective.Storage;
using System.Text;
using Testing_Support;
using Time_Based_Encryption;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace Facade_Tests
{
    [TestClass]
    public sealed class PositiveFacadeTests
    {
        //Bytes => GetBase64FromBytes => GetBytesFromBase64 => Bytes
        [TestMethod]
        public void BytesToBase64ToBytes()
        {
            //// arrange
            ResultObject result = new ResultObject();
            byte[] payload = TestingSupport.GetRandomNumberOfRandomBytes(1, TimeBasedCryptionLimits.MaximumPlaintextBytes);

            //// act
            ResultObject intermediate = FacadeSupport.GetBase64FromBytes(payload, new ValidationSummary());

            if (intermediate.Worked && intermediate.WroteString)
            {
                result = FacadeSupport.GetBytesFromBase64(intermediate.Base64String, new ValidationSummary());
                Console.WriteLine(result.Snapshot);
            }
            else
            {
                Console.WriteLine(intermediate.Snapshot);
            }

            //// assert
            Assert.IsTrue(result.Worked);
            Assert.IsTrue(result.WroteBytes);
            Assert.AreEqual<int>(payload.Length, result.Bytes.Length);
            Assert.AreEqual<string>(TestingSupport.GetHashOfBytes(payload).Base64String, TestingSupport.GetHashOfBytes(result.Bytes).Base64String);
        }

        //Bytes => GetStringFromBytes => GetBytesFromString => Bytes
        [TestMethod]
        public void ByteToStringToBytes()
        {
            //// arrange
            ResultObject result = new ResultObject();
            string gibberish = TestingSupport.GetRandomString(1, TimeBasedCryptionLimits.MaximumPlaintextBytes);
            byte[] payload = Encoding.UTF8.GetBytes(gibberish);

            //// act
            ResultObject intermediate = FacadeSupport.GetStringFromBytes(payload, new ValidationSummary());

            if (intermediate.Worked && intermediate.WroteString)
            {
                result = FacadeSupport.GetBytesFromString(intermediate.Base64String, new ValidationSummary());
                Console.WriteLine(result.Snapshot);
            }
            else
            {
                Console.WriteLine(intermediate.Snapshot);
            }

            //// assert
            Assert.IsTrue(result.Worked);
            Assert.IsTrue(result.WroteBytes);
            Assert.AreEqual<int>(payload.Length, result.Bytes.Length);
            Assert.AreEqual<string>(TestingSupport.GetHashOfBytes(payload).Base64String, TestingSupport.GetHashOfBytes(result.Bytes).Base64String);

        }

        //Bytes => WriteFileFromBytes => GetBytesFromFile => Bytes
        [TestMethod]
        public void BytesToFileToBytes()
        {

            //// arrange
            ResultObject result = new ResultObject();
            byte[] payload = TestingSupport.GetRandomNumberOfRandomBytes(1, TimeBasedCryptionLimits.MaximumPlaintextBytes);
            ResultObject byteContainer = new ResultObject(payload);
            string dropFolderName = "C:\\Users\\Admin\\source\\repos\\Encryption Tool Kit\\Facade Tests\\IntermediateFiles";

            //// act
            ResultObject intermediate = FacadeSupport.WriteFileFromBytes(ref byteContainer, dropFolderName, "", "", new ValidationSummary(),true);

            if (intermediate.Worked && intermediate.WroteFile)
            {
                result = FacadeSupport.GetBytesFromFile(intermediate.FileName, new ValidationSummary());
                Console.WriteLine(result.Snapshot);
            }
            else
            {
                Console.WriteLine(intermediate.Snapshot);
            }

            //// assert
            Assert.IsTrue(result.Worked);
            Assert.IsTrue(result.WroteBytes);
            Assert.AreEqual<int>(payload.Length, result.Bytes.Length);
            Assert.AreEqual<string>(TestingSupport.GetHashOfBytes(payload).Base64String, TestingSupport.GetHashOfBytes(result.Bytes).Base64String);
        }
    }
}
