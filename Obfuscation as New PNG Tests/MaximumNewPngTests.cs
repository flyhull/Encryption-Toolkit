using Common_Support;
using Image_Support;
using Testing_Support;
using Obfuscation_as_New_PNG;

namespace Obfuscation_as_New_PNG_Tests
{
    [TestClass]
    public sealed class MaximumNewPngTests
    {
        [TestMethod]
        public void BytesToNewPngFileToBytes()
        {
            Int32 BytesPerPixel = 4;
            Console.WriteLine(ImageLimits.ShowLimits(BytesPerPixel));

            //// arrange
            ResultObject result = new ResultObject();
            byte[] payload = TestingSupport.GetRandomBytes(ImageLimits.MaximumBytesToStoreInOtherPng);
            bool useVariablePadding = true;

            //// act

            //Use ImageCreator.CreateRgba32PngFromEncryptedBytes to create a png file

            ResultObject intermediate = ImageCreator.CreateRgba32PngFromEncryptedBytes(payload, ImageOutputFormat.file, TestingSupport.intermediateFolder, 
                new ValidationSummary(),  "CreateRgba32PngFromEncryptedBytes" + ImageSupport.pngExtension, useVariablePadding);

            //Use ImageCreator.GetEncryptedBytesFromRgba32PngFile to recover the bytes

            if (intermediate.Worked && intermediate.WroteFile)
            {
                result = ImageCreator.GetEncryptedBytesFromRgba32PngFile(intermediate.FileName, new ValidationSummary());
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

        [TestMethod]
        public void BytesToNewPngBytesToBytes()
        {

            Int32 BytesPerPixel = 4;
            Console.WriteLine(ImageLimits.ShowLimits(BytesPerPixel));

            //// arrange
            ResultObject result = new ResultObject();
            byte[] payload = TestingSupport.GetRandomBytes(ImageLimits.MaximumBytesToStoreInOtherPng);
            bool useVariablePadding = false;

            //// act
            //Use ImageCreator.CreateRgba32PngFromEncryptedBytes to create png bytes

            ResultObject intermediate = ImageCreator.CreateRgba32PngFromEncryptedBytes(payload, ImageOutputFormat.bytes, "", new ValidationSummary(), "", useVariablePadding);

            //Use ImageCreator.GetEncryptedBytesFromRgba32PngBytes to recover the bytes

            if (intermediate.Worked && intermediate.WroteBytes)
            {
                result = ImageCreator.GetEncryptedBytesFromRgba32PngBytes(intermediate.Bytes, new ValidationSummary());
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
