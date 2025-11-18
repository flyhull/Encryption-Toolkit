using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Console_Support
{
    public static class ConsoleSupport
    {

        #region "For Encryption and Decryption"
        public static Byte[] GetPassphrase()
        {
            string? passPhrase = string.Empty;

            while (string.IsNullOrEmpty(passPhrase))
            {
                Console.Write("Please enter passphrase ");
                passPhrase = Console.ReadLine();
            }

            return Encoding.UTF8.GetBytes(passPhrase);
        }

        public static DateTime GetSecretDateTime()
        {
            DateTime secretDateTime = DateTime.UtcNow;
            bool haveSecretDateTime = false;
            string? dateTimeString = string.Empty;
            CultureInfo culture = CultureInfo.InvariantCulture;
            DateTimeStyles style = DateTimeStyles.AssumeUniversal;

            while (!haveSecretDateTime)
            {
                Console.Write("Please enter secret date and optional time using a standard .NET date and time format ");
                dateTimeString = Console.ReadLine();
                //Console.WriteLine("");
                haveSecretDateTime = DateTime.TryParse(dateTimeString, culture, style, out secretDateTime);
            }

            return secretDateTime;
        }

        public static string GetPadFileName(string? directoryToLookInFirst = null)
        {
            NativeFileDialogSharp.DialogResult getKeyFileResult;
            string padFileName = string.Empty;

            if (!String.IsNullOrEmpty(directoryToLookInFirst))
            {
                DirectoryInfo di = new DirectoryInfo(directoryToLookInFirst);
                if (!di.Exists)
                {
                    directoryToLookInFirst = null;
                }
            } else
            {
                directoryToLookInFirst = null;
            }

            while (string.IsNullOrEmpty(padFileName))
            {
                Console.WriteLine("Please select the file to be used as an encryption pad");
                getKeyFileResult = NativeFileDialogSharp.Dialog.FileOpen("png", directoryToLookInFirst);
                if (getKeyFileResult.IsOk)
                {
                    padFileName = getKeyFileResult.Path;
                }
            }

            return padFileName;
        }

        #endregion

        #region "For Encryption"

        public static byte[] GetTextMessage()
        {
            
            //get text and put in array
            string? plaintext = string.Empty;
            while (string.IsNullOrEmpty(plaintext))
            {
                Console.Write("Please enter text to encrypt ");
                plaintext = Console.ReadLine();
                Console.WriteLine("");
            }

            return Encoding.UTF8.GetBytes(plaintext);
        }

        public static string GetNameOfFileToEncrypt(string folderWithFilesToEncrypt)
        {
            NativeFileDialogSharp.DialogResult getInputFileResult;
            string inputFileName = string.Empty;

            while (string.IsNullOrEmpty(inputFileName))
            {
                Console.WriteLine("Please select a file to encrypt");
                getInputFileResult = NativeFileDialogSharp.Dialog.FileOpen(null, folderWithFilesToEncrypt);
                if (getInputFileResult.IsOk)
                {
                    inputFileName = getInputFileResult.Path;
                }
            }
            
            return inputFileName;
        }

        public static string GetModelFileName(string? directoryToLookInFirst = null)
        {
            NativeFileDialogSharp.DialogResult getKeyFileResult;
            string ModelFileName = string.Empty;

            if (!String.IsNullOrEmpty(directoryToLookInFirst))
            {
                DirectoryInfo di = new DirectoryInfo(directoryToLookInFirst);
                if (!di.Exists)
                {
                    directoryToLookInFirst = null;
                }
            }
            else
            {
                directoryToLookInFirst = null;
            }

            while (string.IsNullOrEmpty(ModelFileName))
            {
                Console.WriteLine("Please select the file already used or to be used to obfuscate the cyphertext");
                getKeyFileResult = NativeFileDialogSharp.Dialog.FileOpen("png", directoryToLookInFirst);
                if (getKeyFileResult.IsOk)
                {
                    ModelFileName = getKeyFileResult.Path;
                }
            }

            return ModelFileName;
        }

        public static string GetFileNameForEncryptedFile()
        {
            string? outputFileName = string.Empty;

            while (string.IsNullOrEmpty(outputFileName))
            {
                Console.Write("Please enter Name (without extension) for the encrypted file ");
                outputFileName = Console.ReadLine();
            }

            return outputFileName;
        }




        #endregion

        #region "For Decryption"

        public static DateTime GetEncryptionDateTime(DateTime GuessedDateTime)
        {
            CultureInfo culture = CultureInfo.InvariantCulture;
            DateTimeStyles style = DateTimeStyles.AssumeUniversal;

            Console.WriteLine(string.Concat("The data seems to have been encrypted at ", GuessedDateTime.ToLongTimeString(), " on ", GuessedDateTime.ToLongDateString(), " UTC"));

            bool haveFileDateTime = false;
            DateTime cypherTextCreation = DateTime.UtcNow;
            string? fileDateTimeString = string.Empty;

            while (!haveFileDateTime)
            {
                Console.Write("Hit 'Enter' to confirm. If incorrect, please enter correct UTC date and time ) ");
                fileDateTimeString = Console.ReadLine();
                if (string.IsNullOrEmpty(fileDateTimeString))
                {
                    haveFileDateTime = true;
                    cypherTextCreation = GuessedDateTime;
                }
                else
                {
                    haveFileDateTime = DateTime.TryParse(fileDateTimeString, culture, style, out cypherTextCreation);
                }
            }

            return cypherTextCreation;

        }
        public static string GetEncryptedDataFileName(string encryptedFileExtension, string? directoryToLookIn = null)
        {
            NativeFileDialogSharp.DialogResult getInputFileResult;
            string inputFileName = string.Empty;

            if (!String.IsNullOrEmpty(directoryToLookIn))
            {
                DirectoryInfo di = new DirectoryInfo(directoryToLookIn);
                if (!di.Exists)
                {
                    directoryToLookIn = null;
                }
            }
            else
            {
                directoryToLookIn = null;
            }

            while (string.IsNullOrEmpty(inputFileName))
            {
                Console.WriteLine("Please select a file to decrypt");
                getInputFileResult = NativeFileDialogSharp.Dialog.FileOpen(encryptedFileExtension, directoryToLookIn);
                if (getInputFileResult.IsOk)
                {
                    inputFileName = getInputFileResult.Path;
                }
            }

            return inputFileName;
        }

        public static string GetRecoveredFileName()
        {
            string? outputFileName = string.Empty;

            while (string.IsNullOrEmpty(outputFileName))
            {
                Console.Write("Please enter Name (without extension) for the decrypted file ");
                outputFileName = Console.ReadLine();
            }

            return outputFileName;
        } 

        public static void OpenRecoveredFile(string fileName)
        {
            ProcessStartInfo openFile = new ProcessStartInfo(fileName);
            openFile.UseShellExecute = true;
            Process.Start(openFile);
        }

        #endregion

    }
}
