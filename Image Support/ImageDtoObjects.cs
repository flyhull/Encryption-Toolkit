// Ignore Spelling: Png Rgb Rgba

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using Common_Support;
using System.Reflection;

// https://docs.sixlabors.com/articles/imagesharp/gettingstarted.html

namespace Image_Support
{



    // this is a data transfer object used to hold an image size
    public class ImageSize
    {

        public readonly Int32 height = 1;
        public readonly Int32 width = 1;

        public Int64 PixelCount
        {
            get { return width * height; }
        }

        public ImageSize(Int32 h, Int32 w)
        {
            height = h;
            width = w;
        }

    }

    // this is a data transfer object used for image validation
        public class ImageProfile
    {
        private readonly ImageInfo info;
        public Exception ex = new Exception("");
        public bool Valid
        {
            get { return !(ex.Message.Length > 0); }
        }
        public bool IsPng32
        {
            get { return (Valid && info.PixelType.BitsPerPixel == 32); }
        }
        public Int64 PixelCount
        {
            get
            {
                if (Valid)
                {
                    return info.Height * info.Width;
                }
                else
                {
                    return 0;
                };
            }
        }

        public ImageProfile(string fileName)
        {
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
                info = Image.Identify(fileName);
            }
            catch (Exception x)
            {
                info = new ImageInfo(new SixLabors.ImageSharp.Formats.PixelTypeInfo(1), new SixLabors.ImageSharp.Size(1, 1), null);
                ex = x;
            }

        }

        public ImageProfile(byte[] fileContent)
        {
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
                info = Image.Identify(fileContent);
            }
            catch (Exception x)
            {
                info = new ImageInfo(new SixLabors.ImageSharp.Formats.PixelTypeInfo(1), new SixLabors.ImageSharp.Size(1, 1), null);
                ex = x;
            }
        }
    }    
}
