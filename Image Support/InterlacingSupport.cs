// Ignore Spelling: Rgba

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Diagnostics;
using Common_Support;
using SixLabors.ImageSharp.Formats.Png;
using System.Reflection;

namespace Image_Support
{
    public static class InterlacingSupport
    {       
        const Int32 bytesPerPixel = 1;
        public static ResultObject ReassembleImage(byte[] cypherText, Rgba32[] pixelArray, ImageSize desiredSize, ImageOutputFormat how, string ExistingDirectoryName, ValidationSummary validation, string OptionalOutputFileName = "")
        {
            ResultObject result = new ResultObject();

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
                ImageParamValidation.Validate(ImageParam.Rgba32PngPixels, pixelArray, bytesPerPixel, ref validation);
                ImageParamValidation.Validate(ImageParam.InterlacedImageSize, desiredSize, bytesPerPixel, ref validation);
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
                    ResultObject paddedBytes = PaddingSupport.PadBytes(cypherText, desiredSize.PixelCount, bytesPerPixel);

                    if (paddedBytes.Worked)
                    {
                        Int32 i = 0;

                        //Console.WriteLine("First byte to be interlaced of " + paddedBytes.bytes.Length.ToString() + " bytes is " + BitConverter.ToString(new byte[] { paddedBytes.bytes[0] }));

                        //Console.WriteLine("First pixel before interlaced data is added is " + pixelArray[0].ToHex());


                        while (i < pixelArray.Length)
                        {
                            pixelArray[i] = Interlace(pixelArray[i], paddedBytes.Bytes[i]);

                            i++;
                        }

                        //Console.WriteLine("First pixel after interlaced data is added is " + pixelArray[0].ToHex());

                        using (Image<Rgba32> image = Image.LoadPixelData<Rgba32>(pixelArray, desiredSize.width, desiredSize.height))
                        {

                            Rgba32[] scratch = new Rgba32[image.Width * image.Height];

                            image.CopyPixelDataTo(scratch);

                            //Console.WriteLine("First pixel stored of " + scratch.Length.ToString() + " pixels is " + scratch[0].ToHex());

                            image.Metadata.ExifProfile = null;
                            image.Metadata.XmpProfile = null;

                            switch (how)
                            {
                                case ImageOutputFormat.file:
                                    FileInfo outputFile = ImageSupport.GetNewPngFileName(ExistingDirectoryName, OptionalOutputFileName);
                                    if (outputFile.Exists)
                                    {
                                        //Console.WriteLine("Overwriting " + outputFile.FullName);
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
                        result.RecordImageIssue(ImageIssue.could_not_create_padding_for_interlaced_image, activity);
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

            return result;

        }

        public static ResultObject GetInterlacedEncryptedBytesFromRgba32PngBytes(byte[] imageBytes, ValidationSummary validation)
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

                        Rgba32[] pixelArray = new Rgba32[image.Width * image.Height];

                        image.CopyPixelDataTo(pixelArray);

                        //Console.WriteLine("First recovered pixel with interlaced data of " + pixelArray.Length.ToString() + " pixels is " + pixelArray[0].ToHex());

                        using (MemoryStream ms = new MemoryStream(pixelArray.Length))
                        {
                            foreach (Rgba32 pixel in pixelArray)
                            {
                                ms.WriteByte(UnInterlace(pixel));
                            }

                            byte[] recoveredBytes = ms.ToArray();

                            //Console.WriteLine("First recovered byte is " + BitConverter.ToString(new byte[] { recoveredBytes[0] }));

                            result = new ResultObject(PaddingSupport.UnPadBytes(recoveredBytes));

                            if (result.Failed)
                            {
                                result.RecordImageIssue(ImageIssue.could_not_recover_cyphertext_from_interlaced_image, activity);
                            }

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
                Console.WriteLine("It took " + sw.ElapsedMilliseconds.ToString() + " milliseconds to recover " + result.Bytes.Length.ToString() + " interlaced bytes from png");
            }
            else
            {
                Console.WriteLine("Recovering interlaced bytes from png failed after " + sw.ElapsedMilliseconds.ToString() + " milliseconds");
            }

            return result;

        }

        public static ResultObject GetInterlacedEncryptedBytesFromRgba32PngFile(string fileName, ValidationSummary validation)
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

                        Rgba32[] pixelArray = new Rgba32[image.Width * image.Height];

                        image.CopyPixelDataTo(pixelArray);

                        //Console.WriteLine("First recovered pixel with interlaced data of " + pixelArray.Length.ToString() + " pixels is " + pixelArray[0].ToHex());

                        using (MemoryStream ms = new MemoryStream(pixelArray.Length))
                        {
                            foreach (Rgba32 pixel in pixelArray)
                            {
                                ms.WriteByte(UnInterlace(pixel));
                            }

                            byte[] recoveredBytes = ms.ToArray();

                            //Console.WriteLine("First recovered byte is " + BitConverter.ToString(new byte[] { recoveredBytes[0] }));

                            result = new ResultObject(PaddingSupport.UnPadBytes(recoveredBytes));

                            if (result.Failed)
                            {
                                result.RecordImageIssue(ImageIssue.could_not_recover_cyphertext_from_interlaced_image, activity);
                            }

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
                Console.WriteLine("It took " + sw.ElapsedMilliseconds.ToString() + " milliseconds to recover " + result.Bytes.Length.ToString() + " interlaced bytes from png");
            }
            else
            {
                Console.WriteLine("Recovering interlaced bytes from png failed after " + sw.ElapsedMilliseconds.ToString() + " milliseconds");
            }

            return result;

        }

        #region "These functions are used internally"
        internal static byte UnInterlace(Rgba32 pixel)
        {
            byte mask = 0b_0000_0011;

            byte Nibble3 = (byte)(mask & pixel.A);
            byte Nibble2 = (byte)((mask & pixel.B) << 2);
            byte Nibble1 = (byte)((mask & pixel.G) << 4);
            byte Nibble0 = (byte)((mask & pixel.R) << 6);

            return (byte)(Nibble0 | Nibble1 | Nibble2 | Nibble3);
        }

        internal static Rgba32 Interlace(Rgba32 pixel, byte data)
        {
            byte mask = 0b_1111_1100;

            byte Nibble3 = 0b_0000_0011;
            byte Nibble2 = 0b_0000_1100;
            byte Nibble1 = 0b_0011_0000;
            byte Nibble0 = 0b_1100_0000;

            Nibble3 = (byte)(Nibble3 & data);
            Nibble2 = (byte)(Nibble2 & data);
            Nibble1 = (byte)(Nibble1 & data);
            Nibble0 = (byte)(Nibble0 & data);

            pixel.R = (byte)((pixel.R & mask) | (Nibble0 >> 6));
            pixel.G = (byte)((pixel.G & mask) | (Nibble1 >> 4));
            pixel.B = (byte)((pixel.B & mask) | (Nibble2 >> 2));
            pixel.A = (byte)((pixel.A & mask) | Nibble3);

            return pixel;
        }

        #endregion
    }
}
