using Common_Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Support
{
    public static class ImageLimits
    {
        public const int MinimumTransportPaddingSize = 4;
        
        public static Int64 MinimumRgba32PngPixels(Int32 BytesPerPixel)
        {
            if (BytesPerPixel > 1)
            {
                return 2;
            }
            else
            {
                return 8;
            }

        }
        public static Int64 MaximumRgba32PngPixels(Int32 BytesPerPixel)
        {
            return CommonSupport.PracticalInt32Max / BytesPerPixel;
        }

        public const int MinimumBytesToStoreInAnyPng = 4;

        public static Int64 MaximumBytesToStoreInOtherPng
        {
            get { return MaximumRgba32PngPixels(1) / 64; }
        }

        public static Int64 MaximumBytesToStoreInModelPng
        {
            get { return MaximumRgba32PngPixels(1) / 1024; }
        }

        public static Int64 MinimumModelPngPixels
        {
            get { return MaximumRgba32PngPixels(1) / (1024 * 128); }
        }

        public static Int64 MaximumModelPngPixels
        {
            get { return MaximumRgba32PngPixels(1) / 1024; }
        }

        public static Int64 MinimumCryptionPadPngPixels
        {
            get { return (UInt16.MaxValue / 2) / 3; }
        }

        public static Int64 MaximumCryptionPadPngPixels
        {
            get { return CommonSupport.PracticalInt32Max / 3; }
        }

        public static Int64 MaximumTransportPaddingSize
        {
            get { return CommonSupport.PracticalInt32Max; }
        }

        public static string ShowLimits(Int32 BytesPerPixel)
        {
            List<string> limits = new List<string>();

            limits.Add("** Image Limits **");

            if (BytesPerPixel > 1)
            {
                limits.Add("Based on four bytes per pixel for for Created Images");
            }
            else
            {
                limits.Add("Based on one byte per pixel for for Interlaced Images");
            }

            limits.Add("MinimumTransportPaddingSize: " + MinimumTransportPaddingSize.ToString());
            limits.Add("MaximumTransportPaddingSize: " + MaximumTransportPaddingSize.ToString());
            limits.Add("MinimumRgba32PngPixels: " + MinimumRgba32PngPixels(BytesPerPixel).ToString() + " which will hold " + (MinimumRgba32PngPixels(BytesPerPixel) * BytesPerPixel).ToString() + " bytes");
            limits.Add("MaximumRgba32PngPixels: " + MaximumRgba32PngPixels(BytesPerPixel).ToString() + " which will hold " + (MaximumRgba32PngPixels(BytesPerPixel) * BytesPerPixel).ToString() + " bytes");
            limits.Add("MinimumBytesToStoreInAnyPng: " + MinimumBytesToStoreInAnyPng.ToString());
            limits.Add("MaximumBytesToStoreInModelPng: " + MaximumBytesToStoreInModelPng.ToString());
            limits.Add("MaximumBytesToStoreInOtherPng: " + MaximumBytesToStoreInOtherPng.ToString());
            limits.Add("MinimumModelPngPixels: " + MinimumModelPngPixels.ToString() + " which will store " + (MinimumModelPngPixels * BytesPerPixel).ToString() + " bytes");
            limits.Add("MaximumModelPngPixels: " + MaximumModelPngPixels.ToString() + " which will store " + (MaximumModelPngPixels * BytesPerPixel).ToString() + " bytes");
            limits.Add("MinimumCryptionPadPngPixels: " + MinimumCryptionPadPngPixels.ToString() + " which will yield " + (MinimumCryptionPadPngPixels * 3).ToString() + " bytes");
            limits.Add("MaximumCryptionPadPngPixels: " + MaximumCryptionPadPngPixels.ToString() + " which will yield " + (MaximumCryptionPadPngPixels * 3).ToString() + " bytes");

            return string.Join(Environment.NewLine, limits.ToArray());
        }
    }
}
