using MimeDetective;
using MimeDetective.Engine;
using System.Collections.Immutable;
using System.Globalization;
using System.Text;
using Common_Support;
using Image_Support;
using Pad_Based_Encryption;
using Time_Based_Encryption;
using System.Reflection;
using Microsoft.Extensions.Options;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;
using System.IO;

namespace Facade_Support
{

    public static class FacadeSupport
    {

        #region "From input format to bytes"

        // Base64 String in

        public static ResultObject GetBytesFromBase64(string input, ValidationSummary validation)
        {
            FacadeParamValidation.Validate(FacadeParam.Base64String, input, ref validation);

            if (validation.Valid)
            {
                return new ResultObject(System.Convert.FromBase64String(input));
            }
            else
            {
                return new ResultObject(validation);
            }

        }

        // String in

        public static ResultObject GetBytesFromString(string input, ValidationSummary validation)
        {
            //Validate that string not empty
            FacadeParamValidation.Validate(FacadeParam.NonEmptyString, input, ref validation);

            if (validation.Valid)
            {
                return new ResultObject(Encoding.UTF8.GetBytes(input));
            }
            else
            {
                return new ResultObject(validation);
            }
        }


        // File in

        public static ResultObject GetBytesFromFile(string fileName, ValidationSummary validation)
        {
            //Validate fileName
            FacadeParamValidation.Validate(FacadeParam.ExistingFullFileName,fileName, ref validation);

            if (validation.Valid)
            {
                return new ResultObject(File.ReadAllBytes(fileName));
            }
            else
            {
                return new ResultObject(validation);
            }
        }

        #endregion

        #region "From bytes to useful output format"

        // Base64 String out

        public static ResultObject GetBase64FromBytes(byte[] input, ValidationSummary validation)
        {
            //Validate that bytes are not empty
            FacadeParamValidation.Validate(FacadeParam.NonEmptyBytes, input, ref validation);

            if (validation.Valid)
            {
                string result = Convert.ToBase64String(input);
                
                return new ResultObject(result);
            }
            else
            {
                return new ResultObject(validation);
            }
        }

        // String out

        public static ResultObject GetStringFromBytes(byte[] input, ValidationSummary validation)
        {
            //Validate that bytes are not empty
            FacadeParamValidation.Validate(FacadeParam.NonEmptyBytes, input, ref validation);

            if (validation.Valid)
            {
                return new ResultObject(Encoding.UTF8.GetString(input));
            }
            else
            {
                return new ResultObject(validation);
            }
        }

        // File out

        public static ResultObject WriteFileFromBytes(ref ResultObject input, string existingOutputDirectoryName,  string OptionalOutputFileNameWithoutExt, string dotAndExtension, ValidationSummary validation, bool MyUseIsNonCommercial = false )
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

            if (string.IsNullOrEmpty(dotAndExtension) || dotAndExtension.Length < 4 || !dotAndExtension.StartsWith('.'))
            {
                dotAndExtension = GetFileExtensionFromBytes(ref input,MyUseIsNonCommercial);
            }

            if (string.IsNullOrEmpty(OptionalOutputFileNameWithoutExt))
            {
                OptionalOutputFileNameWithoutExt = CommonSupport.GetRandomString(24, 8);
            }

            //Validate that bytes are not empty
            //Verify directory and filename
            //See
            FacadeParamValidation.Validate(FacadeParam.NonEmptyBytes, input.Bytes, ref validation);
            ImageParamValidation.Validate(ImageParam.RequiredFullDirectoryName, existingOutputDirectoryName, 1, ref validation);
            FacadeParamValidation.Validate(FacadeParam.FileNameWithoutExt, OptionalOutputFileNameWithoutExt, ref validation);
            FacadeParamValidation.Validate(FacadeParam.DotAndExtension, dotAndExtension, ref validation);


            if (validation.Valid)
            {
                result = CreateFileName(dotAndExtension, validation, OptionalOutputFileNameWithoutExt);

                if (result.Worked)
                {
                    FileInfo outputFile = new FileInfo(Path.Combine(existingOutputDirectoryName, result.Base64String));
                    if (outputFile.Exists)
                    {
                        Console.WriteLine("Overwriting " + outputFile.FullName);
                    }
                   
                    File.WriteAllBytes(outputFile.FullName, input.Bytes);
                    Thread.Sleep(input.Bytes.Length / 1024);
                    outputFile.Refresh();
                    if (outputFile.Exists)
                    {
                        result = new ResultObject(outputFile, false);
                    }
                    else
                    {
                        result.RecordImageIssue(ImageIssue.could_not_write_output, activity);
                    }
                }
                else
                {
                    // the result gets returned
                }
            }
            else 
            {
                result = new ResultObject(validation);
            }

            return result;
        }

        #endregion

        #region "File Helpers"
        public static ResultObject CreateFileName(string dotAndExtension, ValidationSummary validation, string OptionalOutputFileNameWithoutExt = "")
        {
            ResultObject result = new ResultObject();
            
            if (string.IsNullOrEmpty(OptionalOutputFileNameWithoutExt))
            {
                OptionalOutputFileNameWithoutExt = CommonSupport.GetRandomString(24,8);
            }
            
            FacadeParamValidation.Validate(FacadeParam.FileNameWithoutExt, OptionalOutputFileNameWithoutExt, ref validation);
            FacadeParamValidation.Validate(FacadeParam.DotAndExtension, dotAndExtension, ref validation);

            if (validation.Valid)
            { 
                result = new ResultObject(string.Concat( OptionalOutputFileNameWithoutExt.Split('.')[0], dotAndExtension));
            }
            else 
            {
                result = new ResultObject(validation);
            }

            return result;
        }

        public static string GetFileExtensionFromBytes(ref ResultObject input, bool MyUseIsNonCommercial)
        {
            ContentInspector Inspector;

            if (MyUseIsNonCommercial)
            {
                Inspector = new ContentInspectorBuilder()
                {
                    Definitions = new MimeDetective.Definitions.CondensedBuilder()
                    {
                        UsageType = MimeDetective.Definitions.Licensing.UsageType.PersonalNonCommercial
                    }.Build()
                }.Build();
            }
            else
            {
                Inspector = new ContentInspectorBuilder()
                {
                    Definitions = MimeDetective.Definitions.Default.All()
                }.Build();
            }

            ImmutableArray<DefinitionMatch> inspectionResult = Inspector.Inspect(input.Bytes);

            if (inspectionResult.Length > 0)
            {
                //it is identified

                string decryptedFileExtension = inspectionResult[0].Definition.File.Extensions[0];

                if (inspectionResult[0].Definition.File.Extensions.Length > 0)
                {
                    return "." + decryptedFileExtension;
                }
                else
                {
                    return ".xxx";
                }

            }
            else
            {
                return ".dat";
            }

        }



        #endregion

        #region "Single Line messaging support"

        public static bool StringIsMultiLine(string input)
        {
            return (input.ToCharArray().Contains<char>('\n'));
        }
               

        #endregion


    }

}
