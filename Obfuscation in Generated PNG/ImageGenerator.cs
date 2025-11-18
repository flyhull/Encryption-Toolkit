// Ignore Spelling: Rgba

using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System.Diagnostics;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using Image_Support;
using Common_Support;
using System.Drawing;
using System.Reflection;

namespace Obfuscation_in_Generated_PNG
{
    public static class ImageGenerator
    {
        const Int32 bytesPerPixel = 1;
       
        #region "these functions support encrypted data which is interlaced into a generated model png" 


        public static ResultObject CreateRgba32PngByInterlacingEncryptedBytes( byte[] bytes, string promptText, ImageOutputFormat how, string ExistingDirectoryName, ValidationSummary validation, string OptionalOutputFileName = "")
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
                ImageParamValidation.Validate(ImageParam.BytesToStoreInPng, bytes, bytesPerPixel, ref validation);
                ImageParamValidation.Validate(ImageParam.GenerationDirectives, promptText, bytesPerPixel, ref validation);
                //wtf
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
                    ImageSize desiredSize = ImageSupport.DetermineNewImageSize(bytes.Length, bytesPerPixel, true);

                    ImageParamValidation.Validate(ImageParam.GeneratedImageSize, desiredSize, bytesPerPixel, ref validation);

                    if (validation.Valid)
                    {
                        result = DrawLines(desiredSize, promptText, validation);
                    }
                    else
                    {
                        result = new ResultObject(validation);
                    }
                                        
                    if (result.Worked)
                    {
                        using (Image<Rgba32> modelImage = Image.Load<Rgba32>(result.Bytes))
                        {
                            pixelArray = new Rgba32[modelImage.Width * modelImage.Height];                            
                            modelImage.CopyPixelDataTo(pixelArray);

                        } // Dispose - releasing memory into a memory pool ready for the next image you wish to process.
                                         
                        //Console.WriteLine("Pixel Array " + pixelArray.Length.ToString() + " size matches " + desiredSize.PixelCount.ToString() + " pixels");

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

        public static ResultObject DrawLines(ImageSize size, string promptText, ValidationSummary validation)
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
                ImageParamValidation.Validate(ImageParam.GeneratedImageSize, size, bytesPerPixel, ref validation);
                ImageParamValidation.Validate(ImageParam.GenerationDirectives, promptText, bytesPerPixel, ref validation);

                if (validation.Valid)
                {
                    Random rand = new Random((Int32)(DateTime.UtcNow.Ticks % Int32.MaxValue));

                    Rgba32 point = new Rgba32((byte)rand.Next(255), (byte)rand.Next(255), (byte)rand.Next(255), (byte)rand.Next(255));

                    string trial = promptText;
                    KnownColor[] colors = (KnownColor[])Enum.GetValues(typeof(KnownColor));
                    foreach (KnownColor hue in colors)
                    {
                        if (trial.Contains(hue.ToString()))
                        {
                            System.Drawing.Color temp = System.Drawing.Color.FromName(hue.ToString());
                            point = new Rgba32(temp.R, temp.G, temp.B, temp.A);
                            break;
                        }
                    }

                    bool matched = false;

                    using (var image = new Image<SixLabors.ImageSharp.PixelFormats.Rgba32>(size.width, size.height, point))
                    {
                        image.Mutate(imageContext =>
                        {
                            int lineCount = (Int32)rand.Next(1000);

                            for (int i = 0; i < lineCount; i++)
                            {
                                if (promptText.ToLower().Contains("none"))
                                {
                                    matched = true;
                                    break;
                                }

                                if (promptText.ToLower().Contains("plaid") || promptText.ToLower().Contains("stripe"))
                                {
                                    matched = true;

                                    float vposition = (float)(8 + rand.NextDouble() * (size.height - 10));

                                    // create an array of two points to make the straight line
                                    var points = new SixLabors.ImageSharp.PointF[2];
                                    points[0] = new SixLabors.ImageSharp.PointF(
                                        x: -1,
                                        y: vposition);
                                    points[1] = new SixLabors.ImageSharp.PointF(
                                        x: size.width,
                                        y: vposition);

                                    // create a pen unique to this line
                                    var lineColor = SixLabors.ImageSharp.Color.FromRgba(
                                        r: (byte)rand.Next(255),
                                        g: (byte)rand.Next(255),
                                        b: (byte)rand.Next(255),
                                        a: (byte)rand.Next(255));
                                    float lineWidth = rand.Next(1, 10);
                                    var linePen = new SixLabors.ImageSharp.Drawing.Processing.SolidPen(lineColor, lineWidth);

                                    // draw the line
                                    imageContext.DrawLine(linePen, points);
                                }

                                if (promptText.ToLower().Contains("plaid") || promptText.ToLower().Contains("bar"))
                                {
                                    matched = true;

                                    float hposition = (float)(8 + rand.NextDouble() * (size.width - 10));

                                    // create an array of two points to make the straight line
                                    var points = new SixLabors.ImageSharp.PointF[2];
                                    points[0] = new SixLabors.ImageSharp.PointF(
                                        x: hposition,
                                        y: -1);
                                    points[1] = new SixLabors.ImageSharp.PointF(
                                        x: hposition,
                                        y: size.height);

                                    // create a pen unique to this line
                                    var lineColor = SixLabors.ImageSharp.Color.FromRgba(
                                        r: (byte)rand.Next(255),
                                        g: (byte)rand.Next(255),
                                        b: (byte)rand.Next(255),
                                        a: (byte)rand.Next(255));
                                    float lineWidth = rand.Next(1, 10);
                                    var linePen = new SixLabors.ImageSharp.Drawing.Processing.SolidPen(lineColor, lineWidth);

                                    // draw the line
                                    imageContext.DrawLine(linePen, points);
                                }

                                if (!matched)
                                {

                                    // create an array of two points to make the straight line
                                    var points = new SixLabors.ImageSharp.PointF[2];
                                    points[0] = new SixLabors.ImageSharp.PointF(
                                        x: (float)(10 + rand.NextDouble() * (size.width - 12)),
                                        y: (float)(10 + rand.NextDouble() * (size.height - 12)));
                                    points[1] = new SixLabors.ImageSharp.PointF(
                                        x: (float)(10 + rand.NextDouble() * (size.width - 12)),
                                        y: (float)(10 + rand.NextDouble() * (size.height - 12)));

                                    // create a pen unique to this line
                                    var lineColor = SixLabors.ImageSharp.Color.FromRgba(
                                        r: (byte)rand.Next(255),
                                        g: (byte)rand.Next(255),
                                        b: (byte)rand.Next(255),
                                        a: (byte)rand.Next(255));
                                    float lineWidth = rand.Next(1, 10);
                                    var linePen = new SixLabors.ImageSharp.Drawing.Processing.SolidPen(lineColor, lineWidth);

                                    // draw the line
                                    imageContext.DrawLine(linePen, points);

                                }
                            }
                        });

                        using (MemoryStream ms = new MemoryStream())
                        {
                            image.Save(ms, new PngEncoder());
                            result = new ResultObject(ms.ToArray());
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
                Console.WriteLine("It took " + sw.ElapsedMilliseconds.ToString() + " milliseconds to render " + result.Bytes.Length.ToString() + " byte model image");
            }
            else
            {
                Console.WriteLine("Rendering a model image failed after " + sw.ElapsedMilliseconds.ToString() + " milliseconds" );
            }

            return result;
        }
    }
}
