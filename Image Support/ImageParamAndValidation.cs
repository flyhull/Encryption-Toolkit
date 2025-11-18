using Common_Support;
using SixLabors.ImageSharp.PixelFormats;

namespace Image_Support
{

    public enum ImageParam
    {
        InterlacedImageSize,
        EnlargedImageSize,
        Rgba32PngPixels,
        Rgba32PngBytes,
        Rgba32PngFile,
        BytesToStoreInPng,
        OptionalFullDirectoryName,
        RequiredFullDirectoryName,
        OptionalOutputPngFileName,
        ModelPngBytes,
        ModelPngFile,
        GenerationDirectives,
        CryptionPadPngFile,
        CryptionPadPngBytes,
        GeneratedImageSize,
        CreatedImageSize,
        BytesToStoreInModelPng
    }

    public class ImageParamValidation
    {       
        public static void Validate(ImageParam param, object input, Int32 bytesPerPixel, ref ValidationSummary summary)
        {
            if (summary.StillNeeds(param.ToString()))
            {
                switch (param)
                {                   
                    case ImageParam.InterlacedImageSize:
                    case ImageParam.CreatedImageSize:
                    case ImageParam.EnlargedImageSize:
                    case ImageParam.GeneratedImageSize:
                        if (input == null)
                        {
                            summary.RecordValidationResult(param.ToString(), ValidationResult.@null);
                        }
                        else
                        {
                            if (input is ImageSize Value)
                            {
                                if (Value.PixelCount > ImageLimits.MinimumRgba32PngPixels(bytesPerPixel))
                                {

                                    if (Value.PixelCount > ImageLimits.MaximumRgba32PngPixels(bytesPerPixel))
                                    {
                                        summary.RecordValidationResult(param.ToString(), ValidationResult.too_large);
                                    }
                                    else
                                    {
                                        summary.RecordValidationResult(param.ToString(), ValidationResult.perfect);
                                    }

                                }
                                else
                                {
                                    summary.RecordValidationResult(param.ToString(), ValidationResult.too_small);
                                }
                            }
                            else
                            {
                                summary.RecordValidationResult(param.ToString(), ValidationResult.wrong_type);
                            }
                        }
                        break;

                    case ImageParam.RequiredFullDirectoryName:
                        if (input == null)
                        {
                            summary.RecordValidationResult(param.ToString(), ValidationResult.@null);
                        }
                        else
                        {
                            if (input is string Value)
                            {
                                if (string.IsNullOrEmpty(Value))
                                {
                                    summary.RecordValidationResult(param.ToString(), ValidationResult.empty);
                                }
                                else
                                {
                                    if (FullOrRelativeDirNameSeemsValid(Value))
                                    {
                                        if (Path.IsPathRooted(Value))
                                        {
                                            if (Directory.Exists(Value))
                                            {
                                                summary.RecordValidationResult(param.ToString(), ValidationResult.perfect);
                                            }
                                            else
                                            {
                                                summary.RecordValidationResult(param.ToString(), ValidationResult.not_present);
                                            }
                                        }
                                        else
                                        {
                                            summary.RecordValidationResult(param.ToString(), ValidationResult.is_relative);
                                        }
                                    }
                                    else
                                    {
                                        summary.RecordValidationResult(param.ToString(), ValidationResult.is_invalid);
                                    }
                                }
                            }
                            else
                            {
                                summary.RecordValidationResult(param.ToString(), ValidationResult.wrong_type);
                            }
                        }
                        break;

                    case ImageParam.OptionalFullDirectoryName:
                        if (input == null)
                        {
                            summary.RecordValidationResult(param.ToString(), ValidationResult.@null);
                        }
                        else
                        {
                            if (input is string Value)
                            {
                                if (string.IsNullOrEmpty(Value))
                                {
                                    summary.RecordValidationResult(param.ToString(), ValidationResult.perfect);
                                }
                                else
                                {
                                    if (FullOrRelativeDirNameSeemsValid(Value))
                                    {
                                        if (Path.IsPathRooted(Value))
                                        {
                                            if (Directory.Exists(Value))
                                            {
                                                summary.RecordValidationResult(param.ToString(), ValidationResult.perfect);
                                            }
                                            else
                                            {
                                                summary.RecordValidationResult(param.ToString(), ValidationResult.not_present);
                                            }
                                        }
                                        else
                                        {
                                            summary.RecordValidationResult(param.ToString(), ValidationResult.is_relative);
                                        }
                                    }
                                    else
                                    {
                                        summary.RecordValidationResult(param.ToString(), ValidationResult.is_invalid);
                                    }
                                }
                            }
                            else
                            {
                                summary.RecordValidationResult(param.ToString(), ValidationResult.wrong_type);
                            }
                        }
                        break;
                    
                    case ImageParam.Rgba32PngPixels:
                        if (input == null)
                        {
                            summary.RecordValidationResult(param.ToString(), ValidationResult.@null);
                        }
                        else
                        {
                            if (input is Rgba32[] Value)
                            {
                                if (Value.Length > ImageLimits.MinimumRgba32PngPixels(bytesPerPixel))
                                {                                    
                                    if (Value.Length > ImageLimits.MaximumRgba32PngPixels(bytesPerPixel))
                                    {
                                        summary.RecordValidationResult(param.ToString(), ValidationResult.too_many);
                                    }
                                    else
                                    {
                                        summary.RecordValidationResult(param.ToString(), ValidationResult.perfect);
                                    }                                    
                                }
                                else
                                {
                                    summary.RecordValidationResult(param.ToString(), ValidationResult.too_small);
                                }
                            }
                            else
                            {
                                summary.RecordValidationResult(param.ToString(), ValidationResult.wrong_type);
                            }
                        }
                        break;

                    case ImageParam.Rgba32PngBytes:

                        summary.RecordValidationResult(param.ToString(), ValidatePngBytes(input, false, ImageLimits.MinimumRgba32PngPixels(bytesPerPixel), ImageLimits.MaximumRgba32PngPixels(bytesPerPixel)));
                        break;

                    case ImageParam.Rgba32PngFile:

                        summary.RecordValidationResult(param.ToString(), ValidatePngFile(input,false, ImageLimits.MinimumRgba32PngPixels(bytesPerPixel), ImageLimits.MaximumRgba32PngPixels(bytesPerPixel)));                      
                        break;

                    case ImageParam.BytesToStoreInPng:
                        if (input == null)
                        {
                            summary.RecordValidationResult(param.ToString(), ValidationResult.@null);
                        }
                        else
                        {
                            if (input is byte[] Value)
                            {
                                if (Value.Length > 0)
                                {                                
                                    if (Value.Length < ImageLimits.MinimumBytesToStoreInAnyPng)
                                    {
                                        summary.RecordValidationResult(param.ToString(), ValidationResult.too_small);
                                    }
                                    else
                                    {
                                    
                                            if (Value.Length > ImageLimits.MaximumBytesToStoreInOtherPng)
                                            {
                                                summary.RecordValidationResult(param.ToString(), ValidationResult.too_large);
                                            }
                                            else
                                            {
                                                summary.RecordValidationResult(param.ToString(), ValidationResult.perfect);
                                            }

                                    }
                                }
                                else
                                {
                                    summary.RecordValidationResult(param.ToString(), ValidationResult.empty);
                                }
                            }
                            else
                            {
                                summary.RecordValidationResult(param.ToString(), ValidationResult.wrong_type);
                            }
                        }
                        break;

                    case ImageParam.BytesToStoreInModelPng:
                        if (input == null)
                        {
                            summary.RecordValidationResult(param.ToString(), ValidationResult.@null);
                        }
                        else
                        {
                            if (input is byte[] Value)
                            {
                                if (Value.Length > 0)
                                {
                                    if (Value.Length < ImageLimits.MinimumBytesToStoreInAnyPng)
                                    {
                                        summary.RecordValidationResult(param.ToString(), ValidationResult.too_small);
                                    }
                                    else
                                    {

                                        if (Value.Length > ImageLimits.MaximumBytesToStoreInModelPng)
                                        {
                                            summary.RecordValidationResult(param.ToString(), ValidationResult.too_large);
                                        }
                                        else
                                        {
                                            summary.RecordValidationResult(param.ToString(), ValidationResult.perfect);
                                        }

                                    }
                                }
                                else
                                {
                                    summary.RecordValidationResult(param.ToString(), ValidationResult.empty);
                                }
                            }
                            else
                            {
                                summary.RecordValidationResult(param.ToString(), ValidationResult.wrong_type);
                            }
                        }
                        break;

                    case ImageParam.OptionalOutputPngFileName:

                        if (input == null)
                        {
                            summary.RecordValidationResult(param.ToString(), ValidationResult.@null);
                        }
                        else
                        {
                            if (input is String Value)
                            {
                                if (Value.Length < 1)
                                {
                                    summary.RecordValidationResult(param.ToString(), ValidationResult.perfect);
                                }
                                else
                                {
                                    if (Value.Length > 256)
                                    {
                                        summary.RecordValidationResult(param.ToString(), ValidationResult.too_long);
                                    }
                                    else
                                    {
                                        if (FullOrRelativeFileNameSeemsValid(Value))
                                        {
                                            if (Value.ToLower().EndsWith(".png"))
                                            {
                                                if (Path.IsPathRooted(Value))
                                                {
                                                    summary.RecordValidationResult(param.ToString(), ValidationResult.has_path);
                                                }
                                                else
                                                {
                                                    summary.RecordValidationResult(param.ToString(), ValidationResult.perfect);
                                                }                                                    
                                            }
                                            else
                                            {
                                                summary.RecordValidationResult(param.ToString(), ValidationResult.has_wrong_extension);
                                            }
                                        }
                                        else
                                        {
                                            summary.RecordValidationResult(param.ToString(), ValidationResult.is_invalid);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                summary.RecordValidationResult(param.ToString(), ValidationResult.wrong_type);
                            }
                        }
                        break;

                    //case ImageParam.RequiredOutputPngFileName:

                    //    if (input == null)
                    //    {
                    //        summary.RecordVaidationResult(param.ToString(), ValidationResult.@null);
                    //    }
                    //    else
                    //    {
                    //        if (input is String Value)
                    //        {
                    //            if (Value.Length < 5)
                    //            {
                    //                summary.RecordVaidationResult(param.ToString(), ValidationResult.too_short);
                    //            }
                    //            else
                    //            {
                    //                if (Value.Length > 256)
                    //                {
                    //                    summary.RecordVaidationResult(param.ToString(), ValidationResult.too_long);

                    //                }
                    //                else
                    //                {
                    //                    if (FullOrRelativeFileNameSeemsValid(Value))
                    //                    {
                    //                        if (Value.ToLower().EndsWith(".png"))
                    //                        {
                    //                            summary.RecordVaidationResult(param.ToString(), ValidationResult.perfect);
                    //                        }
                    //                        else
                    //                        {
                    //                            summary.RecordVaidationResult(param.ToString(), ValidationResult.not_a_png);
                    //                        }

                    //                    }
                    //                    else
                    //                    {
                    //                        summary.RecordVaidationResult(param.ToString(), ValidationResult.is_invalid);
                    //                    }
                    //                }
                    //            }
                    //        }
                    //        else
                    //        {
                    //            summary.RecordVaidationResult(param.ToString(), ValidationResult.wrong_type);
                    //        }
                    //    }
                    //    break;

                    case ImageParam.ModelPngBytes:
                        summary.RecordValidationResult(param.ToString(), ValidatePngBytes(input, true, ImageLimits.MinimumModelPngPixels, ImageLimits.MaximumModelPngPixels));
                        break;

                    case ImageParam.ModelPngFile:
                        summary.RecordValidationResult(param.ToString(), ValidatePngFile(input, true, ImageLimits.MinimumModelPngPixels, ImageLimits.MaximumModelPngPixels));
                        break;

                    case ImageParam.GenerationDirectives:

                        if (input == null)
                        {
                            summary.RecordValidationResult(param.ToString(), ValidationResult.@null);
                        }
                        else
                        {
                            if (input is String Value)
                            {
                                if (Value.Length < 1)
                                {
                                    summary.RecordValidationResult(param.ToString(), ValidationResult.empty);
                                }
                                else
                                {

                                    if (Value.Length > 256)
                                    {
                                        summary.RecordValidationResult(param.ToString(), ValidationResult.too_long);

                                    }
                                    else
                                    {
                                        if (string.Concat(Value.Where(c => (!char.IsWhiteSpace(c) && !char.IsLetter(c)))).Length > 0)
                                        {
                                            summary.RecordValidationResult(param.ToString(), ValidationResult.is_invalid);
                                        }
                                        else
                                        {
                                            summary.RecordValidationResult(param.ToString(), ValidationResult.perfect);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                summary.RecordValidationResult(param.ToString(), ValidationResult.wrong_type);
                            }
                        }
                        break;
                    
                    case ImageParam.CryptionPadPngFile:
                        ValidationResult result = ValidatePngFile(input, true, ImageLimits.MinimumCryptionPadPngPixels, ImageLimits.MaximumCryptionPadPngPixels);
                        //ideally the pad file is on a removable drive but that cannot be tested
                        if (result == ValidationResult.perfect)
                        {
                            FileInfo fi = new FileInfo((string)input);
                            if (fi.Directory == null)
                            {
                                summary.RecordValidationResult(param.ToString(), ValidationResult.orphaned_file);
                            } 
                            else
                            {
                                if (fi.Directory.Root.Name.Equals(fi.Directory.Name))
                                {
                                    if (ImageSupport.GetRemovableVolumes().Contains(fi.Directory.Root.Name))
                                    {
                                        summary.RecordValidationResult(param.ToString(), result);
                                    } 
                                    else
                                    {
                                        summary.RecordValidationResult(param.ToString(), ValidationResult.not_on_removable_drive);
                                    }                                        
                                }
                                else
                                {
                                    summary.RecordValidationResult(param.ToString(), ValidationResult.not_on_drive_root);
                                }
                            }
                        }
                        else
                        {
                            summary.RecordValidationResult(param.ToString(),result);
                        }
                        break;

                    case ImageParam.CryptionPadPngBytes:
                        summary.RecordValidationResult(param.ToString(), ValidatePngBytes(input, true, ImageLimits.MinimumCryptionPadPngPixels, ImageLimits.MaximumCryptionPadPngPixels));
                        break;

                    default:
                        summary.RecordValidationResult(param.ToString(), ValidationResult.out_of_scope);
                        break;

                }
            }
        }

        #region "private generic png validators"
        private static ValidationResult ValidatePngFile(Object input, bool DoesNotNeedToBeRgba32, Int64 LowerLimit, Int64 UpperLimit )
        {
            if (input == null)
            {
                return  ValidationResult.@null;
            }
            else
            {
                if (input is string Value)
                {
                    if (Value.Length > 0)
                    {
                        if (File.Exists(Value))
                        {
                            if (ImageSupport.IsPng(Value))
                            {
                                ImageProfile info = new ImageProfile(Value);

                                if (info.Valid)
                                {
                                    if (info.IsPng32 || DoesNotNeedToBeRgba32)
                                    {
                                        if (info.PixelCount < LowerLimit)
                                        {
                                            return ValidationResult.too_small;
                                        }
                                        else
                                        {
                                            if (info.PixelCount > UpperLimit)
                                            {
                                                return  ValidationResult.too_large;
                                            }
                                            else
                                            {
                                                return  ValidationResult.perfect;
                                            }
                                        }                                        
                                    }
                                    else
                                    {
                                        return  ValidationResult.not_a_transparent_png;
                                    }
                                }
                                else
                                {
                                    return  ValidationResult.unreadable;
                                }
                            }
                            else
                            {
                                return  ValidationResult.not_a_png;
                            }
                        }
                        else
                        {
                            return  ValidationResult.not_present;
                        }

                    }
                    else
                    {
                        return  ValidationResult.missing;
                    }
                }
                else
                {
                    return  ValidationResult.wrong_type;
                }
            }
        }

        private static ValidationResult ValidatePngBytes(Object input, bool DoesNotNeedToBeRgba32, Int64 LowerLimit, Int64 UpperLimit)
        {
            if (input == null)
            {
                return ValidationResult.@null;
            }
            else
            {
                if (input is byte[] Value)
                {
                    if (Value.Length > 0)
                    {
                        if (ImageSupport.IsPng(Value))
                        {
                            ImageProfile info = new ImageProfile(Value);

                            if (info.Valid)
                            {
                                if (info.IsPng32 || DoesNotNeedToBeRgba32)
                                {
                                    if (info.PixelCount < LowerLimit)
                                    {
                                        return ValidationResult.too_small;
                                    }
                                    else
                                    {
                                        if (info.PixelCount > UpperLimit)
                                        {
                                            return ValidationResult.too_large;
                                        }
                                        else
                                        {
                                            return ValidationResult.perfect;
                                        }
                                    }                                    
                                }
                                else
                                {
                                    return ValidationResult.not_a_transparent_png;
                                }
                            }
                            else
                            {
                                return ValidationResult.unreadable;
                            }
                        }
                        else
                        {
                            return ValidationResult.not_a_png;
                        }
                    }
                    else
                    {
                        return ValidationResult.empty;
                    }
                }
                else
                {
                    return ValidationResult.wrong_type;
                }
            }
        }

        #endregion

        #region "private file and path name validators used above"

        
        private static bool NameOfFileSeemsValid(string proposedFileName)
        {

            if (proposedFileName.All(x => char.IsLetterOrDigit(x) || x == '-' || x == '_'))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        internal static bool FullOrRelativeFileNameSeemsValid(string proposedFullOrRelativeFileName, string extension = "")
        {
            if (string.IsNullOrEmpty(extension) || proposedFullOrRelativeFileName.ToLower().EndsWith("." + extension))
            {
                if (Environment.OSVersion.Platform.ToString().StartsWith('W'))
                {
                    if (proposedFullOrRelativeFileName.All(x => char.IsLetterOrDigit(x) || x == Path.DirectorySeparatorChar || x == '-' || x == '_' || x == '.' || x == ' ' || x == Path.VolumeSeparatorChar))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (proposedFullOrRelativeFileName.All(x => char.IsLetterOrDigit(x) || x == Path.DirectorySeparatorChar || x == '-' || x == '_' || x == '.'))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }

        private static bool FullOrRelativeDirNameSeemsValid(string proposedFullOrRelativeDirName)
        {
            if (Environment.OSVersion.Platform.ToString().StartsWith('W'))
            {
                if (proposedFullOrRelativeDirName.All(x => char.IsLetterOrDigit(x) || x == Path.DirectorySeparatorChar || x == '-' || x == '.' || x == ' ' || x == Path.VolumeSeparatorChar))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else 
            { 
                if (proposedFullOrRelativeDirName.All(x => char.IsLetterOrDigit(x) || x == Path.DirectorySeparatorChar || x == '-' || x == '.' ))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        #endregion

    }
}
