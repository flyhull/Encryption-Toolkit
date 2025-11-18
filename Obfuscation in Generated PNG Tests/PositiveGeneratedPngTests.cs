using Common_Support;
using Image_Support;
using Testing_Support;
using Obfuscation_in_Generated_PNG;

namespace Obfuscation_in_Generated_PNG_Tests
{
    [TestClass]
    public sealed class PositiveGeneratedPngTests
    {

            [TestMethod]
            public void BytesToGeneratedPngBytesToBytes()
            {

                Int32 BytesPerPixel = 1;
                Console.WriteLine(ImageLimits.ShowLimits(BytesPerPixel));

                //// arrange
                ResultObject result = new ResultObject();
                byte[] payload = TestingSupport.GetRandomNumberOfRandomBytes(ImageLimits.MinimumBytesToStoreInAnyPng, ImageLimits.MaximumBytesToStoreInOtherPng);
                //bool useVariablePadding = true;

                //// act

                //Use ImageGenerator.CreateRgba32PngByInterlacingEncryptedBytes to create png bytes inside a generated png

                ResultObject intermediate = ImageGenerator.CreateRgba32PngByInterlacingEncryptedBytes(payload, "plaid Cyan", ImageOutputFormat.bytes, "", new ValidationSummary());

                if (intermediate.Worked && intermediate.WroteBytes)
                {
                    //Use ImageGenerator.GetInterlacedEncryptedBytesFromRgba32PngBytes to recover the payload from the png bytes

                    result = ImageGenerator.GetInterlacedEncryptedBytesFromRgba32PngBytes(intermediate.Bytes, new ValidationSummary());
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
            public void BytesToGeneratedPngFileToBytes()
            {
                Int32 BytesPerPixel = 1;
                Console.WriteLine(ImageLimits.ShowLimits(BytesPerPixel));

                //// arrange
                ResultObject result = new ResultObject();
                byte[] payload = TestingSupport.GetRandomNumberOfRandomBytes(ImageLimits.MinimumBytesToStoreInAnyPng, ImageLimits.MaximumBytesToStoreInOtherPng);
                //bool useVariablePadding = true;

                //// act

                //Use ImageGenerator.CreateRgba32PngByInterlacingEncryptedBytes to create a png file  inside a generated png

                ResultObject intermediate = ImageGenerator.CreateRgba32PngByInterlacingEncryptedBytes(payload, "plaid Cyan", ImageOutputFormat.file, TestingSupport.intermediateFolder, 
                    new ValidationSummary(), "CreateRgba32PngByInterlacingEncryptedBytes.png" + ImageSupport.pngExtension);            

                if (intermediate.Worked && intermediate.WroteFile)
                {
                    //Use ImageGenerator.GetInterlacedEncryptedBytesFromRgba32PngFile to recover the payload from the png file
                
                    result = ImageGenerator.GetInterlacedEncryptedBytesFromRgba32PngFile(intermediate.FileName, new ValidationSummary());
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
