using Common_Support;
using Image_Support;
using Testing_Support;
using Obfuscation_in_Existing_PNG;

namespace Obfuscation_in_Existing_PNG_Tests
{
    [TestClass]
    public sealed class MaximumExistingPngTests
    {
        //model is file, intermediate is bytes

        [TestMethod]
        public void BytesIntoExistingPngFileToPngBytesToBytes()
        {

            Int32 BytesPerPixel = 1;
            Console.WriteLine(ImageLimits.ShowLimits(BytesPerPixel));

            //// arrange
            ResultObject result = new ResultObject();
            string modelFileName = "C:\\Users\\Admin\\source\\repos\\Encryption Tool Kit\\Testing Support\\PngFiles\\book.png";
            byte[] payload = TestingSupport.GetRandomBytes(ImageLimits.MaximumBytesToStoreInModelPng);
            //bool useVariablePadding = true;

            //// act
            //Use ImageProcessor.CreateRgba32PngByInterlacingEncryptedBytesIntoModelFile to create png bytes inside a model png file
            
            ResultObject intermediate = ImageProcessor.CreateRgba32PngByInterlacingEncryptedBytesIntoModelFile(modelFileName, payload, ImageOutputFormat.bytes, "", new ValidationSummary());
                        
            if (intermediate.Worked && intermediate.WroteBytes)
            {
                //Use ImageProcessor.GetInterlacedEncryptedBytesFromRgba32PngBytes to recover the payload from the png bytes

                result = ImageProcessor.GetInterlacedEncryptedBytesFromRgba32PngBytes(intermediate.Bytes, new ValidationSummary());
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

        //model is bytes, intermediate is bytes

        [TestMethod]
            public void BytesIntoExistingPngBytesToPngBytesToBytes()
            {
                Int32 BytesPerPixel = 1;
                Console.WriteLine(ImageLimits.ShowLimits(BytesPerPixel));

                //// arrange
                ResultObject result = new ResultObject();
                string modelFileName = "C:\\Users\\Admin\\source\\repos\\Encryption Tool Kit\\Testing Support\\PngFiles\\cat.png";
                byte[] modelBytes = File.ReadAllBytes(modelFileName);
                byte[] payload = TestingSupport.GetRandomBytes(ImageLimits.MaximumBytesToStoreInModelPng);
                //bool useVariablePadding = true;

                //// act
                //Use ImageProcessor.CreateRgba32PngByInterlacingEncryptedBytesIntoModelBytes to create png bytes inside model png bytes
                
                ResultObject intermediate = ImageProcessor.CreateRgba32PngByInterlacingEncryptedBytesIntoModelBytes(modelBytes, payload, ImageOutputFormat.bytes, "", new ValidationSummary());
                           
                if (intermediate.Worked && intermediate.WroteBytes)
                {
                    //Use ImageProcessor.GetInterlacedEncryptedBytesFromRgba32PngBytes to recover the payload from the png bytes

                    result = ImageProcessor.GetInterlacedEncryptedBytesFromRgba32PngBytes(intermediate.Bytes, new ValidationSummary());
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

        //model is bytes, intermediate is file
  
        [TestMethod]
            public void BytesIntoExistingPngBytesToPngFileToBytes()
            {
                Int32 BytesPerPixel = 1;
                Console.WriteLine(ImageLimits.ShowLimits(BytesPerPixel));

                //// arrange
                ResultObject result = new ResultObject();
                string modelFileName = "C:\\Users\\Admin\\source\\repos\\Encryption Tool Kit\\Testing Support\\PngFiles\\smiley.png";
                byte[] modelBytes = File.ReadAllBytes(modelFileName);
                byte[] payload = TestingSupport.GetRandomBytes(ImageLimits.MaximumBytesToStoreInModelPng);
                //bool useVariablePadding = true;

                //// act
                //Use ImageProcessor.CreateRgba32PngByInterlacingEncryptedBytesIntoModelBytes to create a png file inside model png bytes
                
                ResultObject intermediate = ImageProcessor.CreateRgba32PngByInterlacingEncryptedBytesIntoModelBytes(modelBytes,payload, ImageOutputFormat.file, TestingSupport.intermediateFolder, 
                    new ValidationSummary(), "CreateRgba32PngByInterlacingEncryptedBytesIntoModelBytes" + ImageSupport.pngExtension);
                           
                if (intermediate.Worked && intermediate.WroteFile)
                {
                    //Use ImageProcessor.GetInterlacedEncryptedBytesFromRgba32PngBytes to recover the payload from the png file

                    result = ImageProcessor.GetInterlacedEncryptedBytesFromRgba32PngFile(intermediate.FileName, new ValidationSummary());
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
        
        //model is file, intermediate is file

        [TestMethod]
            public void BytesIntoExistingPngFileToPngFileToBytes()
            {
                Int32 BytesPerPixel = 1;
                Console.WriteLine(ImageLimits.ShowLimits(BytesPerPixel));

                //// arrange
                ResultObject result = new ResultObject();
                string modelFileName = "C:\\Users\\Admin\\source\\repos\\Encryption Tool Kit\\Testing Support\\PngFiles\\boat.png";
                byte[] payload = TestingSupport.GetRandomBytes(ImageLimits.MaximumBytesToStoreInModelPng);
                //bool useVariablePadding = true;

                //// act
                //Use ImageProcessor.CreateRgba32PngByInterlacingEncryptedBytesIntoModelFile to create a png file inside a model png file
               
                ResultObject intermediate = ImageProcessor.CreateRgba32PngByInterlacingEncryptedBytesIntoModelFile(modelFileName, payload, ImageOutputFormat.file, TestingSupport.intermediateFolder, 
                    new ValidationSummary(), "CreateRgba32PngByInterlacingEncryptedBytesIntoModelFile" + ImageSupport.pngExtension);
                            
                if (intermediate.Worked && intermediate.WroteFile)
                {
                    //Use ImageProcessor.GetInterlacedEncryptedBytesFromRgba32PngBytes to recover the payload from the png file

                    result = ImageProcessor.GetInterlacedEncryptedBytesFromRgba32PngFile(intermediate.FileName, new ValidationSummary());
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
