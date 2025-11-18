// Ignore Spelling: Png Rgb Rgba

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using Common_Support;
using System.Reflection;
using System.Text;

// https://docs.sixlabors.com/articles/imagesharp/gettingstarted.html

namespace Image_Support
{
    
    public enum ImageOutputFormat
    {
        bytes,
        base64,
        file
    }

    public static class ImageSupport
    {
        public const string pngExtension = ".png";

        public static List<string> GetRemovableVolumes()
        {
            List<string> result = new List<string>();

            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.DriveType == DriveType.Removable)
                {
                    result.Add(drive.RootDirectory.Name);
                }
            }

            return result;
        }

        public static bool IsPng(string padFileFullName)
        {
            return (Image.DetectFormat(padFileFullName).Name == "PNG");
        }

        public static bool IsPng(byte[] padFileBytes)
        {
            return (Image.DetectFormat(padFileBytes).Name == "PNG");
        }

        public static FileInfo GetNewPngFileName(string ExistingDirectoryName, string OptionalOutputFileName = "")
        {
            if (string.IsNullOrEmpty(OptionalOutputFileName))
            {
                OptionalOutputFileName = CommonSupport.GetRandomString(24,8);
            }

            if (!OptionalOutputFileName.EndsWith(".png"))
            {
                OptionalOutputFileName = OptionalOutputFileName + ImageSupport.pngExtension;
            }

            string fullFileName = Path.Combine(ExistingDirectoryName, OptionalOutputFileName);

            return new FileInfo(fullFileName);
        }

        public static Rgba32 FirstRgba32PixelInPngFile(string fileName)
        {
            Image<Rgba32> input = (Image<Rgba32>)Image<Rgba32>.Load(fileName);
            Rgba32[] temp = new Rgba32[input.Width * input.Height];
            input.CopyPixelDataTo(temp);
            return temp[0];
        }
        public static Rgba32 FirstRgba32PixelInPngBytes(byte[] pngBytes)
        {
            Image<Rgba32> input = (Image<Rgba32>)Image<Rgba32>.Load(pngBytes);
            Rgba32[] temp = new Rgba32[input.Width * input.Height];
            input.CopyPixelDataTo(temp);
            return temp[0];
        }

        #region "these function read the bytes which should be available from every png"

        public static ResultObject GetRgbBytesFromPngBytes(byte[] padFileBytes, bool ForPadFile,  ValidationSummary validation)
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

            Int32 bytesPerPixel = 1;

            if (ForPadFile)
            {
                ImageParamValidation.Validate(ImageParam.CryptionPadPngBytes, padFileBytes, bytesPerPixel, ref validation);
            }
            else
            {
                ImageParamValidation.Validate(ImageParam.ModelPngBytes, padFileBytes, bytesPerPixel, ref validation);
            }

            if (validation.Valid)
            {

                byte[] intermediateResult = Array.Empty<byte>();

                using (Image<Rgb24> image = Image.Load<Rgb24>(padFileBytes))
                {
                    intermediateResult = new byte[image.Width * image.Height * 3];

                    image.CopyPixelDataTo(intermediateResult);
                    result = new ResultObject(intermediateResult);
                    if (result.Failed)
                    {
                        result.RecordImageIssue(ImageIssue.could_not_retrieve_bytes_from_png, activity);
                    }
                }
            }
            else
            {
                result = new ResultObject(validation);
            }

            sw.Stop();

            if (result.Worked)
            {
                Console.WriteLine("It took " + sw.ElapsedMilliseconds.ToString() + " milliseconds to extract " + result.Bytes.Length.ToString() + " bytes");
            }
            else
            {
                Console.WriteLine("Extracting bytes from png failed after " + sw.ElapsedMilliseconds.ToString() + " milliseconds");
            }

            return result;

        }

        public static ResultObject GetRgbBytesFromPngFile(string fileName, bool ForPadFile, ValidationSummary validation)
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

            Int32 bytesPerPixel = 1;

            if (ForPadFile)
            {
                ImageParamValidation.Validate(ImageParam.CryptionPadPngFile, fileName, bytesPerPixel, ref validation);
            }
            else
            {
                ImageParamValidation.Validate(ImageParam.ModelPngFile, fileName, bytesPerPixel, ref validation);
            }

            if (validation.Valid)
            {

                byte[] intermediateResult = Array.Empty<byte>();

                using (Image<Rgb24> image = Image.Load<Rgb24>(fileName))
                {
                    intermediateResult = new byte[image.Width * image.Height * 3];

                    image.CopyPixelDataTo(intermediateResult);
                    result = new ResultObject(intermediateResult);
                    if (result.Failed)
                    {
                        result.RecordImageIssue(ImageIssue.could_not_retrieve_bytes_from_png, activity);
                    }
                }
            }
            else
            {
                result = new ResultObject(validation);
            }

            sw.Stop();

            if (result.Worked)
            {
                Console.WriteLine("It took " + sw.ElapsedMilliseconds.ToString() + " milliseconds to extract " + result.Bytes.Length.ToString() + " bytes");
            }
            else
            {
                Console.WriteLine("Extracting bytes from png failed after " + sw.ElapsedMilliseconds.ToString() + " milliseconds");
            }

            return result;

        }

        #endregion

        #region "These functions are used internally"

        public static Int32 DetermineEnlargementFactor(Int32 h, Int32 w, Int32 bytesNeeded, Int32 bytesPerPixel)
        {
            Int32 actualBytesNeeded = ImageLimits.MinimumTransportPaddingSize + bytesNeeded;
            Int32 bytesStoredBeforeEnlargement = h * w * bytesPerPixel;

            decimal raw = actualBytesNeeded / bytesStoredBeforeEnlargement;

            Int32 result = 1 + (Int32)Math.Floor(raw);

            //Console.WriteLine("Enlargement Factor is " + result.ToString());

            return result;

        }

        public static ImageSize DetermineNewImageSize(Int32 bytesNeeded, Int32 bytesPerPixel, bool varyPadding)
        {
            Int32 actualBytesNeeded = ImageLimits.MinimumTransportPaddingSize + bytesNeeded;

            Int32 extraBytes = 0;

            if (varyPadding)
            {

                Random rand = new Random((Int32)(DateTime.UtcNow.Ticks % Int32.MaxValue));

                Int32 maxToAdd = (Int32)(Math.Floor((Decimal)ImageLimits.MaximumBytesToStoreInOtherPng - actualBytesNeeded) / 2) % Int32.MaxValue;
                
                if (maxToAdd < 1)
                {
                    extraBytes = 0;
                }
                else
                {
                    extraBytes = (Int32)rand.Next(Math.Min(maxToAdd, bytesNeeded));
                }

            }

            Int32 actualPixelsNeeded = (actualBytesNeeded + extraBytes) / bytesPerPixel;

            Int32 height = (Int32)Math.Floor(Math.Sqrt(actualPixelsNeeded));
            decimal raw = actualPixelsNeeded / height;
            Int32 width = 1 + (Int32)Math.Floor(raw);

            //Console.WriteLine("Image will be " + height.ToString() + " pixels high and " + width.ToString() + " pixels wide");

            return new ImageSize(height, width);

        }

        

        #endregion



    }
}
