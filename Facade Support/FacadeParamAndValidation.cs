using Common_Support;
using Image_Support;
using System.Buffers.Text;
using System.ComponentModel.Design;
using Time_Based_Encryption;

namespace Facade_Support
{

    public enum FacadeParam
    {          
        Base64String,
        NonEmptyString,
        NonEmptyBytes,
        ExistingFullFileName,
        //ExistingFullDirectoryName,
        FileNameWithoutExt,
        DotAndExtension
    }

    internal class FacadeParamValidation
    {
        public static void Validate(FacadeParam param, object input, ref ValidationSummary summary)
        {
            if (summary.StillNeeds(param.ToString()))
            {
                switch (param)
                {
                    case FacadeParam.Base64String:
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
                                    if (StringIsBase64(Value)) 
                                    {
                                        summary.RecordValidationResult(param.ToString(), ValidationResult.perfect);
                                    }
                                    else
                                    {
                                        summary.RecordValidationResult(param.ToString(), ValidationResult.not_base64);
                                    }
                                }

                            }
                            else
                            {
                                summary.RecordValidationResult(param.ToString(), ValidationResult.wrong_type);
                            }
                        }
                        break;

                    case FacadeParam.NonEmptyString:
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
                                    summary.RecordValidationResult(param.ToString(), ValidationResult.perfect);
                                }
                            }
                            else
                            {
                                summary.RecordValidationResult(param.ToString(), ValidationResult.wrong_type);
                            }
                        }
                        break;

                    case FacadeParam.NonEmptyBytes:
                        if (input == null)
                        {
                            summary.RecordValidationResult(param.ToString(), ValidationResult.@null);
                        }
                        else
                        {
                            if (input is byte[] Value)
                            {
                                if (Value.Length < 1)
                                {
                                    summary.RecordValidationResult(param.ToString(), ValidationResult.empty);
                                }
                                else 
                                {
                                    if (Value.Length > TimeBasedCryptionLimits.MaximumPlaintextBytes)
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
                                summary.RecordValidationResult(param.ToString(), ValidationResult.wrong_type);
                            }
                        }
                        break;

                    case FacadeParam.ExistingFullFileName:
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
                                    if (FullOrRelativeFileNameSeemsValid(Value))
                                    {
                                        if (Path.IsPathRooted(Value))
                                        {
                                            if (File.Exists(Value))
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

                    //case FacadeParam.ExistingFullDirectoryName:
                    //    if (input == null)
                    //    {
                    //        summary.RecordVaidationResult(param.ToString(), ValidationResult.@null);
                    //    }
                    //    else
                    //    {
                    //        if (input is string Value)
                    //        {
                    //            if (string.IsNullOrEmpty(Value))
                    //            {
                    //                summary.RecordVaidationResult(param.ToString(), ValidationResult.empty);
                    //            }
                    //            else
                    //            {
                    //                if (FullOrRelativeDirNameSeemsValid(Value))
                    //                {
                    //                    if (Path.IsPathRooted(Value))
                    //                    {
                    //                        if (Directory.Exists(Value))
                    //                        {
                    //                            summary.RecordVaidationResult(param.ToString(), ValidationResult.perfect);
                    //                        }
                    //                        else
                    //                        {
                    //                            summary.RecordVaidationResult(param.ToString(), ValidationResult.not_present);
                    //                        }
                    //                    }
                    //                    else
                    //                    {
                    //                        summary.RecordVaidationResult(param.ToString(), ValidationResult.is_relative);
                    //                    }
                    //                }
                    //                else
                    //                {
                    //                    summary.RecordVaidationResult(param.ToString(), ValidationResult.is_invalid);
                    //                }
                    //            }
                    //        }
                    //        else
                    //        {
                    //            summary.RecordVaidationResult(param.ToString(), ValidationResult.wrong_type);
                    //        }
                    //    }
                    //    break;


                    case FacadeParam.FileNameWithoutExt:
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
                                    if (Value.Length > 200) 
                                    {
                                        summary.RecordValidationResult(param.ToString(), ValidationResult.too_long);
                                    } 
                                    else 
                                    {
                                        if (FullOrRelativeFileNameSeemsValid(Value)) 
                                        {
                                            summary.RecordValidationResult(param.ToString(), ValidationResult.perfect);
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

                    case FacadeParam.DotAndExtension:
                        if (input == null)
                        {
                            summary.RecordValidationResult(param.ToString(), ValidationResult.@null);
                        }
                        else
                        {
                            if (input is string Value)
                            {
                                if (Value.Length < 4)
                                {
                                    summary.RecordValidationResult(param.ToString(), ValidationResult.too_short);
                                }
                                else
                                {
                                    if (Value.Length < 4)
                                    {
                                        summary.RecordValidationResult(param.ToString(), ValidationResult.too_short);
                                    }
                                    else
                                    {
                                        if (Value.StartsWith('.'))
                                        {
                                            summary.RecordValidationResult(param.ToString(), ValidationResult.perfect);
                                        }
                                        else
                                        {
                                            summary.RecordValidationResult(param.ToString(), ValidationResult.missing_period);
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

                    default:
                        summary.RecordValidationResult(param.ToString(), ValidationResult.out_of_scope);
                        break;
                }
            }
        }

        #region "functions used for facade validations"

        public static bool StringIsBase64(string base64)
        {
            int bufferSize = (base64.Length * 3 + 3) / 4 - (base64.Length > 0 && base64[^1] == '=' ? base64.Length > 1 && base64[^2] == '=' ? 2 : 1 : 0);
            Span<byte> buffer = new byte[bufferSize]; 
            return Convert.TryFromBase64String(base64, buffer, out _);
        }

        public static bool NameOfFileSeemsValid(string proposedFileName)
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

        public static bool FullOrRelativeFileNameSeemsValid(string proposedFullOrRelativeFileName, string extension = "")
        {
            if (string.IsNullOrEmpty(extension) || proposedFullOrRelativeFileName.ToLower().EndsWith("." + extension))
            {
                if (Environment.OSVersion.Platform.ToString().StartsWith('W'))
                {
                    if (proposedFullOrRelativeFileName.All(x => char.IsLetterOrDigit(x) || x == Path.VolumeSeparatorChar || x == Path.DirectorySeparatorChar || x == '-' || x == '_' || x == '.' || x == ' '))
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

        public static bool FullOrRelativeDirNameSeemsValid(string proposedFullOrRelativeDirName)
        {
            if (Environment.OSVersion.Platform.ToString().StartsWith('W'))
            {
                if (proposedFullOrRelativeDirName.All(x => char.IsLetterOrDigit(x) || x == Path.DirectorySeparatorChar || x == '-' || x == '_' || x == ':'))
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
                if (proposedFullOrRelativeDirName.All(x => char.IsLetterOrDigit(x) || x == Path.DirectorySeparatorChar || x == '-' || x == '_'))
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