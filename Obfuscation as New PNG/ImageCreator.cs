using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System.Diagnostics;
using Common_Support;
using Image_Support;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Net.NetworkInformation;

namespace Obfuscation_as_New_PNG
{
    public static class ImageCreator
    {

        const Int32 bytesPerPixel = 4;

        #region "these functions support encrypted data which stuffed into a pastel-looking png"

        public static ResultObject CreateRgba32PngFromEncryptedBytes(byte[] cypherText, ImageOutputFormat how, string ExistingDirectoryName, ValidationSummary validation,  string OptionalOutputFileName = "", bool varyPadding = true)
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
                ImageParamValidation.Validate(ImageParam.BytesToStoreInPng, cypherText, bytesPerPixel, ref validation);
                if (how == ImageOutputFormat.file)
                {
                    ImageParamValidation.Validate(ImageParam.RequiredFullDirectoryName, ExistingDirectoryName, bytesPerPixel, ref validation);
                }
                else
                {
                    ImageParamValidation.Validate(ImageParam.OptionalFullDirectoryName, ExistingDirectoryName, bytesPerPixel, ref validation);
                }

                ImageParamValidation.Validate(ImageParam.OptionalOutputPngFileName, OptionalOutputFileName, bytesPerPixel, ref validation);
                // not validating varyPadding because it is a boolean with a default, there is no point

                if (validation.Valid)
                {                                
                    ImageSize size = ImageSupport.DetermineNewImageSize(cypherText.Length , bytesPerPixel, varyPadding);

                    ImageParamValidation.Validate(ImageParam.CreatedImageSize, size, bytesPerPixel, ref validation);

                    if (validation.Valid)
                    {                   
                        ResultObject paddedBytes = PaddingSupport.PadBytes(cypherText, size.PixelCount , bytesPerPixel);

                        if (paddedBytes.Worked)
                        {
                            Rgba32[] rgba32Data = new Rgba32[size.PixelCount];

                            Int32 i = 0;
                            Int32 j = 0;

                            while (i < size.PixelCount)
                            {
                                j = i * 4;

                                rgba32Data[i] = new Rgba32(paddedBytes.Bytes[j], paddedBytes.Bytes[1 + j], paddedBytes.Bytes[2 + j], paddedBytes.Bytes[3 + j]);

                                i++;
                            }

                            using (Image<Rgba32> image = Image.LoadPixelData<Rgba32>(rgba32Data, size.width, size.height))
                            {
                                image.Metadata.ExifProfile = null;
                                image.Metadata.XmpProfile = null;

                                switch(how)
                                {
                                    case ImageOutputFormat.file:
                                        FileInfo outputFile = ImageSupport.GetNewPngFileName(ExistingDirectoryName, OptionalOutputFileName);
                                        if (outputFile.Exists)
                                        {
                                            Console.WriteLine("Overwriting " + outputFile.FullName);
                                        }
                                        image.SaveAsPng(outputFile.FullName);
                                        Thread.Sleep((image.Height * image.Width) / 1024);
                                        outputFile.Refresh();
                                        if (outputFile.Exists)
                                        {
                                            result = new ResultObject(outputFile, false);
                                        } 
                                        else
                                        {
                                            result.RecordImageIssue(ImageIssue.could_not_write_output, activity);
                                        }                                            
                                        break;
                                    case ImageOutputFormat.base64:
                                        result = new ResultObject(image.ToBase64String(PngFormat.Instance).Split(',')[1]);
                                        break;
                                    default:
                                        using (MemoryStream ms = new MemoryStream())
                                        {
                                            image.Save(ms, new PngEncoder());
                                            result = new ResultObject(ms.ToArray());
                                        }
                                        break;

                                }

                                if (result.Failed)
                                {
                                    result.RecordImageIssue(ImageIssue.failed_to_create_new_image, activity + " to " + how.ToString());
                                }

                            } // Dispose - releasing memory into a memory pool ready for the next image you wish to process.

                        }
                        else
                        {
                            result.RecordImageIssue(ImageIssue.could_not_create_padding_for_created_image, activity);
                        }

                    }
                    else
                    {
                        result = new ResultObject(validation);
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
                Console.WriteLine("It took " + sw.ElapsedMilliseconds.ToString() + " milliseconds to create the png from " + cypherText.Length.ToString() + " bytes");
            }
            else
            {
                Console.WriteLine("Creating the png from " + cypherText.Length.ToString() + " bytes failed after " + sw.ElapsedMilliseconds.ToString() + " milliseconds");
            }

            return result;
        }

        public static ResultObject GetEncryptedBytesFromRgba32PngBytes(byte[] imageBytes, ValidationSummary validation)
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
                ImageParamValidation.Validate(ImageParam.Rgba32PngBytes, imageBytes, bytesPerPixel, ref validation);

                if (validation.Valid)
                {                    
                    using (Image<Rgba32> image = Image.Load<Rgba32>(imageBytes))
                    {
                        byte[] intermediateResult = new byte[image.Height * image.Width * Unsafe.SizeOf<Rgba32>()]; 
                        image.CopyPixelDataTo(intermediateResult);
                        result = new ResultObject(PaddingSupport.UnPadBytes(intermediateResult) );
                        if (result.Failed)
                        {
                            result.RecordImageIssue(ImageIssue.could_not_recover_cyphertext_from_created_image, activity);
                        }
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
                Console.WriteLine("It took " + sw.ElapsedMilliseconds.ToString() + " milliseconds to recover " + result.Bytes.Length.ToString() + " bytes from png");
            }
            else
            {
                Console.WriteLine("Recovering all pixel bytes from png failed after " + sw.ElapsedMilliseconds.ToString() + " milliseconds");
            }

            return result;

        }

        public static ResultObject GetEncryptedBytesFromRgba32PngFile(string fileName, ValidationSummary validation)
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
                ImageParamValidation.Validate(ImageParam.Rgba32PngFile, fileName, bytesPerPixel, ref validation);

                if (validation.Valid)
                {
                    using (Image<Rgba32> image = Image.Load<Rgba32>(fileName))
                    {
                        byte[] intermediateResult = new byte[image.Width * image.Height * Unsafe.SizeOf<Rgba32>()];
                        image.CopyPixelDataTo(intermediateResult);
                        result = new ResultObject(PaddingSupport.UnPadBytes(intermediateResult));
                        if (result.Failed)
                        {
                            result.RecordImageIssue(ImageIssue.could_not_recover_cyphertext_from_created_image, activity);
                        }
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
                Console.WriteLine("It took " + sw.ElapsedMilliseconds.ToString() + " milliseconds to recover " + result.Bytes.Length.ToString() + " bytes from png");
            }
            else
            {
                Console.WriteLine("Recovering all pixel bytes from png failed after " + sw.ElapsedMilliseconds.ToString() + " milliseconds");
            }

            return result;

        }


        #endregion

    }


}
