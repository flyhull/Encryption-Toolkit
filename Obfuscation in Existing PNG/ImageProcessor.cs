// Ignore Spelling: Rgba

using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp;
using System.Diagnostics;
using Image_Support;
using Common_Support;
using System.Xml.Schema;
using System.Reflection;

namespace Obfuscation_in_Existing_PNG
{
    public static class ImageProcessor
    {
        const Int32 bytesPerPixel = 1;
        
        

        #region "these functions support encrypted data which is interlaced into a copy of a model png" 


        public static ResultObject CreateRgba32PngByInterlacingEncryptedBytesIntoModelFile(string modelFileName, byte[] bytes, ImageOutputFormat how, string ExistingDirectoryName, ValidationSummary validation, string OptionalOutputFileName = "")
        {
            ResultObject result = new ResultObject();

            Stopwatch sw = new Stopwatch();

            sw.Start();
            

            string activity = "";
            MethodBase? m = MethodBase.GetCurrentMethod();
            if (m == null)
            {
                activity = "unknown";
            }
            else
            {
                if (m.ReflectedType == null)
                {
                    activity = m.Name;
                }
                else
                {
                    activity = m.ReflectedType.Name + "." + m.Name;
                }
            }

            try
            {
                ImageParamValidation.Validate(ImageParam.ModelPngFile, modelFileName, bytesPerPixel, ref validation);
                ImageParamValidation.Validate(ImageParam.BytesToStoreInModelPng, bytes, bytesPerPixel, ref validation);
                if (how == ImageOutputFormat.file)
                {
                    ImageParamValidation.Validate(ImageParam.RequiredFullDirectoryName, ExistingDirectoryName, bytesPerPixel, ref validation);
                }
                else
                {
                    ImageParamValidation.Validate(ImageParam.OptionalFullDirectoryName, ExistingDirectoryName, bytesPerPixel, ref validation);
                }
                ImageParamValidation.Validate(ImageParam.OptionalOutputPngFileName, OptionalOutputFileName, bytesPerPixel, ref validation);

                if (validation.Valid)
                {
                    Rgba32[] pixelArray = Array.Empty<Rgba32>();
                    ImageSize desiredSize = new ImageSize(1, 1);


                    ImageProfile modelProfile = new ImageProfile(modelFileName);

                    // Open the file automatically detecting the file type to decode it.
                    // Our image is now in an uncompressed, file format agnostic, structure in-memory as
                    // a series of pixels.
                    // You can also specify the pixel format using a type parameter (e.g. Image<Rgba32> image = Image.Load<Rgba32>("foo.jpg"))

                    if (modelProfile.IsPng32)
                    {

                        using (Image<Rgba32> modelImage = Image.Load<Rgba32>(modelFileName))
                        {

                            // get pixel array from Png32 model

                            Int32 enlargementFactor = ImageSupport.DetermineEnlargementFactor(modelImage.Height, modelImage.Width, bytes.Length, bytesPerPixel);

                            // Resize the image in place and return it for chaining.
                            // 'x' signifies the current image processing context.

                            if (enlargementFactor > 1)
                            {

                                modelImage.Mutate(x => x.Resize(modelImage.Width * enlargementFactor, modelImage.Height * enlargementFactor));
                                //Console.WriteLine("Image was enlarged.");

                            }
                            else
                            {
                                //Console.WriteLine("Did not need to enlarge image");
                            }

                            desiredSize = new ImageSize(modelImage.Height, modelImage.Width);

                            ImageParamValidation.Validate(ImageParam.EnlargedImageSize, desiredSize, bytesPerPixel, ref validation);

                            if (validation.Valid)
                            {
                                pixelArray = new Rgba32[modelImage.Width * modelImage.Height];
                                modelImage.CopyPixelDataTo(pixelArray);
                            }
                            else
                            {
                                result = new ResultObject(validation);
                            }

                        } // Dispose - releasing memory into a memory pool ready for the next image you wish to process.

                    }
                    else
                    {

                        // alternative logic if the model image does not have transparency

                        //Console.WriteLine("XXXXXXXXXXX Model is on wrong format XXXXXXXXXX");

                        using (Image<Rgb24> modelImage = Image.Load<Rgb24>(modelFileName))
                        {

                            Rgb24[] testPixelArray = new Rgb24[modelImage.Width * modelImage.Height];

                            modelImage.CopyPixelDataTo(testPixelArray);

                            Int32 enlargementFactor = ImageSupport.DetermineEnlargementFactor(modelImage.Height, modelImage.Width, bytes.Length, bytesPerPixel);

                            // Resize the image in place and return it for chaining.
                            // 'x' signifies the current image processing context.

                            if (enlargementFactor > 1)
                            {

                                modelImage.Mutate(x => x.Resize(modelImage.Width * enlargementFactor, modelImage.Height * enlargementFactor));
                                //Console.WriteLine("Image was enlarged.");

                            }
                            else
                            {
                                //Console.WriteLine("Did not need to enlarge image");
                            }

                            using (Image<Rgba32> transparentImage = modelImage.CloneAs<Rgba32>())
                            {

                                desiredSize = new ImageSize(transparentImage.Height, transparentImage.Width);

                                ImageParamValidation.Validate(ImageParam.EnlargedImageSize, desiredSize, bytesPerPixel, ref validation);

                                if (validation.Valid)
                                {
                                    pixelArray = new Rgba32[transparentImage.Width * transparentImage.Height];
                                    transparentImage.CopyPixelDataTo(pixelArray);
                                }
                                else
                                {
                                    result = new ResultObject(validation);
                                }

                            }

                        } // Dispose - releasing memory into a memory pool ready for the next image you wish to process.

                    }

                    //Console.WriteLine("Pixel Array " + pixelArray.Length.ToString() + " size matches " + desiredSize.PixelCount.ToString() + " pixels");

                    if (result.ImageBasedIssue == ImageIssue.None)
                    {
                        result = InterlacingSupport.ReassembleImage(bytes, pixelArray, desiredSize, how, ExistingDirectoryName, validation, OptionalOutputFileName);
                    }

                }
                else
                {
                    result = new ResultObject(validation);
                }

            }
            catch (Exception ex)
            {
                result = new ResultObject(ex, activity);
            }

            sw.Stop();

            if (result.Worked)
            {
                Console.WriteLine("It took " + sw.ElapsedMilliseconds.ToString() + " milliseconds to interlace " + bytes.Length.ToString() + " bytes into the model png");
            }
            else
            {
                Console.WriteLine("Interlacing " + bytes.Length.ToString() + " bytes into the model png failed after " + sw.ElapsedMilliseconds.ToString() + " milliseconds");
            }

            return result;

        }

        public static ResultObject CreateRgba32PngByInterlacingEncryptedBytesIntoModelBytes(byte[] modelFileBytes, byte[] bytes, ImageOutputFormat how, string ExistingDirectoryName, ValidationSummary validation, string OptionalOutputFileName = "")
        {
            ResultObject result = new ResultObject();

            Stopwatch sw = new Stopwatch();

            sw.Start();

            string activity = "";
            MethodBase? m = MethodBase.GetCurrentMethod();
            if (m == null)
            {
                activity = "unknown";
            }
            else
            {
                if (m.ReflectedType == null)
                {
                    activity = m.Name;
                }
                else
                {
                    activity = m.ReflectedType.Name + "." + m.Name;
                }
            }

            try
            {
                ImageParamValidation.Validate(ImageParam.ModelPngBytes, modelFileBytes, bytesPerPixel, ref validation);
                ImageParamValidation.Validate(ImageParam.BytesToStoreInModelPng, bytes, bytesPerPixel, ref validation);
                if (how == ImageOutputFormat.file)
                {
                    ImageParamValidation.Validate(ImageParam.RequiredFullDirectoryName, ExistingDirectoryName, bytesPerPixel, ref validation);
                }
                else
                {
                    ImageParamValidation.Validate(ImageParam.OptionalFullDirectoryName, ExistingDirectoryName, bytesPerPixel, ref validation);
                }
                ImageParamValidation.Validate(ImageParam.OptionalOutputPngFileName, OptionalOutputFileName, bytesPerPixel, ref validation);

                if (validation.Valid)
                {
                    Rgba32[] pixelArray = Array.Empty<Rgba32>();
                    ImageSize desiredSize = new ImageSize(1, 1);


                    ImageProfile modelProfile = new ImageProfile(modelFileBytes);

                    // Open the file automatically detecting the file type to decode it.
                    // Our image is now in an uncompressed, file format agnostic, structure in-memory as
                    // a series of pixels.
                    // You can also specify the pixel format using a type parameter (e.g. Image<Rgba32> image = Image.Load<Rgba32>("foo.jpg"))

                    if (modelProfile.IsPng32)
                    {

                        using (Image<Rgba32> modelImage = Image.Load<Rgba32>(modelFileBytes))
                        {

                            // get pixel array from Png32 model

                            Int32 enlargementFactor = ImageSupport.DetermineEnlargementFactor(modelImage.Height, modelImage.Width, bytes.Length, bytesPerPixel);

                            // Resize the image in place and return it for chaining.
                            // 'x' signifies the current image processing context.

                            if (enlargementFactor > 1)
                            {

                                modelImage.Mutate(x => x.Resize(modelImage.Width * enlargementFactor, modelImage.Height * enlargementFactor));
                                //Console.WriteLine("Image was enlarged.");

                            }
                            else
                            {
                                //Console.WriteLine("Did not need to enlarge image");
                            }

                            desiredSize = new ImageSize(modelImage.Height, modelImage.Width);

                            ImageParamValidation.Validate(ImageParam.EnlargedImageSize, desiredSize, bytesPerPixel, ref validation);

                            if (validation.Valid)
                            {
                                pixelArray = new Rgba32[modelImage.Width * modelImage.Height];
                                modelImage.CopyPixelDataTo(pixelArray);
                            }
                            else
                            {
                                result = new ResultObject(validation);
                            }

                        } // Dispose - releasing memory into a memory pool ready for the next image you wish to process.

                    }
                    else
                    {

                        // alternative logic if the model image does not have transparency

                        //Console.WriteLine("XXXXXXXXXXX Model is on wrong format XXXXXXXXXX");

                        using (Image<Rgb24> modelImage = Image.Load<Rgb24>(modelFileBytes))
                        {

                            Rgb24[] testPixelArray = new Rgb24[modelImage.Width * modelImage.Height];

                            modelImage.CopyPixelDataTo(testPixelArray);

                            Int32 enlargementFactor = ImageSupport.DetermineEnlargementFactor(modelImage.Height, modelImage.Width, bytes.Length, bytesPerPixel);

                            // Resize the image in place and return it for chaining.
                            // 'x' signifies the current image processing context.

                            if (enlargementFactor > 1)
                            {

                                modelImage.Mutate(x => x.Resize(modelImage.Width * enlargementFactor, modelImage.Height * enlargementFactor));
                                //Console.WriteLine("Image was enlarged.");

                            }
                            else
                            {
                                //Console.WriteLine("Did not need to enlarge image");
                            }

                            using (Image<Rgba32> transparentImage = modelImage.CloneAs<Rgba32>())
                            {

                                desiredSize = new ImageSize(transparentImage.Height, transparentImage.Width);

                                ImageParamValidation.Validate(ImageParam.EnlargedImageSize, desiredSize, bytesPerPixel, ref validation);

                                if (validation.Valid)
                                {
                                    pixelArray = new Rgba32[transparentImage.Width * transparentImage.Height];
                                    transparentImage.CopyPixelDataTo(pixelArray);
                                }
                                else
                                {
                                    result = new ResultObject(validation);
                                }

                            }

                        } // Dispose - releasing memory into a memory pool ready for the next image you wish to process.

                    }

                    //Console.WriteLine("Pixel Array " + pixelArray.Length.ToString() + " size matches " + desiredSize.PixelCount.ToString() + " pixels");

                    if (result.ImageBasedIssue == ImageIssue.None)
                    {
                        result = InterlacingSupport.ReassembleImage(bytes, pixelArray, desiredSize, how, ExistingDirectoryName, validation, OptionalOutputFileName);
                    }

                }
                else
                {
                    result = new ResultObject(validation);
                }

            }
            catch (Exception ex)
            {
                result = new ResultObject(ex, activity);
            }

            sw.Stop();

            if (result.Worked)
            {
                Console.WriteLine("It took " + sw.ElapsedMilliseconds.ToString() + " milliseconds to interlace " + bytes.Length.ToString() + " bytes into the model png");
            }
            else
            {
                Console.WriteLine("Interlacing " + bytes.Length.ToString() + " bytes into the model png failed after " + sw.ElapsedMilliseconds.ToString() + " milliseconds");
            }

            return result;
        }

        public static ResultObject GetInterlacedEncryptedBytesFromRgba32PngBytes(byte[] bytes, ValidationSummary validation)
        {
            return InterlacingSupport.GetInterlacedEncryptedBytesFromRgba32PngBytes(bytes, validation);
        }

        public static ResultObject GetInterlacedEncryptedBytesFromRgba32PngFile(string fileName, ValidationSummary validation)
        {
            return InterlacingSupport.GetInterlacedEncryptedBytesFromRgba32PngFile(fileName, validation);
        }

        #endregion




    }

}
