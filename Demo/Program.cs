using Microsoft.Extensions.Logging;
using Facade_Support;
using Time_Based_Encryption;
using Pad_Based_Encryption;
using Image_Support;
using Common_Support;
using Console_Support;
using Obfuscation_as_New_PNG;
using Obfuscation_in_Existing_PNG;
using Obfuscation_in_Generated_PNG;

byte[] passPhraseBytes = Array.Empty<byte>();
//string imageFileExtention = ".png";
string encryptedFileExtention = ".enc";
string? dateTimeString = string.Empty;
string? inputFileName = string.Empty;
string? decryptedFileExtension = string.Empty;
string? padFileName = string.Empty;
string? passPhrase = string.Empty;
DateTime secretDateTime = DateTime.UtcNow;
Int32 encryptionSecondsIncludingStorage = 360;

ResultObject retrievePad = new ResultObject();
ValidationSummary credentialsValidation = new ValidationSummary();
ValidationSummary timestampValidation = new ValidationSummary();
ResultObject retrieveCyphertext = new ResultObject();
DateTime cypherTextCreation = DateTime.MaxValue.AddDays(-2);
ResultObject createFileFromModel = new ResultObject();
NativeFileDialogSharp.DialogResult showFileResult;

bool tutorialMode = false;

bool MyUseIsNonCommercial = true;

string folderWithFilesToEncrypt = "C:\\Users\\Admin\\OneDrive\\Documents";
string folderWithPossibleModelFiles = "C:\\Users\\Admin\\source\\repos\\Encryption Tool Kit\\Demo\\AssortedPngFiles";
string folderWithEncryptedFiles = "C:\\Users\\Admin\\source\\repos\\Encryption Tool Kit\\Demo\\FilesContainingCyphertext";
string folderWithDecryptedFiles = "C:\\Users\\Admin\\source\\repos\\Encryption Tool Kit\\Demo\\DecryptedFiles";

using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder
        .AddFilter("Microsoft", LogLevel.Warning)
        .AddFilter("System", LogLevel.Warning)
        .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug)
        .AddConsole();
});

ILogger logger = loggerFactory.CreateLogger<Program>();

Console.Title = "Encryption Tool Kit Demo";
Console.WindowWidth = 220;
Console.BackgroundColor = ConsoleColor.DarkMagenta;
Console.WriteLine("");
Console.WriteLine("************************************************************************************************************");
Console.WriteLine("* DO NOT USE THIS CODE TO RISK LIFE OR FREEDOM (OR EVEN MONEY), IT IS ONLY A POC OF THE CRYPTION ALGORITHM *");
Console.WriteLine("************************************************************************************************************");
Console.WriteLine("");
Console.WriteLine("Why? Read this");
Console.WriteLine(" https://crypto.stackexchange.com/questions/43272/why-is-writing-your-own-encryption-discouraged");
Console.WriteLine("and this");
Console.WriteLine(" https://soatok.blog/2025/01/31/hell-is-overconfident-developers-writing-encryption-code");
Console.WriteLine("and find the picture of the cake here");
Console.WriteLine(" https://crypto.stackexchange.com/questions/58897/writing-your-own-encryption-algorithm");
Console.WriteLine("");
Console.WriteLine("**********************************************************************************************************************");
Console.WriteLine("* Even if no other mistakes were made, this program is not protected against local attacks on real or virtual memory *");
Console.WriteLine("**********************************************************************************************************************");
Console.WriteLine("");
Console.BackgroundColor = ConsoleColor.Black;
Console.WriteLine("That said, it implements symmetric encryption using a one-time key and uses that to implement symmetric multi-factor encryption using a physical (USB Stick) pad");
Console.WriteLine("");
Console.WriteLine("An explanation of the algorithm is here: C:\\Users\\Admin\\source\\repos\\Encryption Tool Kit\\Demo\\Documentation");
Console.WriteLine("");
Console.Write("Type T for Tutorial Mode or X for Expert Mode ");
ConsoleKeyInfo tutorial = Console.ReadKey();
Console.WriteLine("");

if (tutorial.KeyChar == 'T' || tutorial.KeyChar == 't') 
{
    tutorialMode = true;
    encryptionSecondsIncludingStorage = 600;
}

Console.WriteLine("First Enter Cryption Keys");

while (retrievePad.Failed)
{
    if (tutorialMode)
    {
        Console.BackgroundColor = ConsoleColor.DarkBlue;
        Console.WriteLine("Encryption is based on two things you know, a pass phrase and a point in time, and one thing you have, an image file in png format on the root of a removable drive");
        Console.WriteLine("First you will be asked for the pass phrase which must be at least " + TimeBasedCryptionLimits.MinimumPassPhraseLength.ToString() + " characters long");
        //Console.WriteLine("Please press any key to continue");
        Console.BackgroundColor = ConsoleColor.Black;
        //Console.ReadKey();
    }

    passPhraseBytes = ConsoleSupport.GetPassphrase();

    if (tutorialMode)
    {
        Console.BackgroundColor = ConsoleColor.DarkBlue;
        Console.WriteLine("Now you will be asked for a point in time using a standard Microsoft .Net format, YYYY-MM-DD works");
        Console.WriteLine("This will be used in conjunction with the current time to generate a third time value which will be used once");
        Console.WriteLine("The third time value will used along with the passphrase to generate a symmetric encryption key");
        //Console.WriteLine("Please press any key to continue");
        Console.BackgroundColor = ConsoleColor.Black;
        //Console.ReadKey();    
    }

    secretDateTime = ConsoleSupport.GetSecretDateTime();

    if (tutorialMode)
    {
        Console.BackgroundColor = ConsoleColor.DarkBlue;
        Console.WriteLine("Now you will be asked for an encryption pad in the form of a png image file stored on the root directory of a a removable drive");
        Console.WriteLine("Please make sure that a removable drive is attached and that it holds a png image file suitable for use as a encryption pad on it's root");
        Console.WriteLine("Please press any key to continue");
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ReadKey();
    }

    List<string> RemovableDrives = ImageSupport.GetRemovableVolumes();
    if (RemovableDrives.Count > 0)
    {
        Console.WriteLine("The image selected must contain between " + ImageLimits.MinimumCryptionPadPngPixels.ToString() + " and " + ImageLimits.MaximumCryptionPadPngPixels.ToString() + " pixels");
        padFileName = ConsoleSupport.GetPadFileName(RemovableDrives.First<string>());

        Console.WriteLine("Pad file " + padFileName + " selected");
    }
    else
    {
        Console.BackgroundColor = ConsoleColor.DarkRed;
        Console.WriteLine("The pad file needs to be at on the root directory of a removable drive (USB stick or such) but none is attached");
        Console.WriteLine("Please press any key to try again");
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ReadKey();
        break;
    }

    TimeBasedParamValidation.Validate(TimeBasedCrypterParam.Passphrase, passPhraseBytes, ref credentialsValidation);
    TimeBasedParamValidation.Validate(TimeBasedCrypterParam.SecretDateTime, secretDateTime, ref credentialsValidation);

    //Console.WriteLine("Reading contents of " + padFileName + " for use as encryption pad");

    retrievePad = ImageSupport.GetRgbBytesFromPngFile(padFileName, true, credentialsValidation);

    if (retrievePad.Failed)
    {
        Console.BackgroundColor = ConsoleColor.DarkRed;
        Console.WriteLine("Cryption Keys are Invalid");
        Console.WriteLine(retrievePad.Snapshot);
        credentialsValidation = new ValidationSummary();
        Console.Write("Please press any key to try again");
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ReadKey();
        Console.WriteLine("");
    }
    else
    {
        Console.WriteLine("Cryption Keys Accepted");

        if (tutorialMode)
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("The color bytes in each pixel of the png file image selected for the pad will be extracted and used to create an array of bytes");
            Console.WriteLine("The bytes will be encrypted using the symmetric encryption key to create a random encryption pad for a book cypher");
            Console.WriteLine("Since the symmetric encryption key should be unique the encrypted pad should be a one time pad.  To be sure, just use each image once.");
            Console.WriteLine("Next you will be asked whether you want to use the credentials to encrypt or decrypt");
            //Console.WriteLine("Please press any key to continue");
            Console.BackgroundColor = ConsoleColor.Black;
            //Console.ReadKey();
        }

    }

} // while entering credentials and retrieving pad

Console.Write("Type E to encrypt or D to decrypt ");
ConsoleKeyInfo input = Console.ReadKey();
Console.WriteLine("");

if (input.KeyChar == 'D' || input.KeyChar == 'd')
{
    Console.WriteLine("Decrypting selected");

    while (retrieveCyphertext.Failed)
    {
        if (tutorialMode)
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("Now you will be asked for the file to decrypt. It will be read, converted to a byte array and decrypted using the symmetric encryption key to recover the array of numbers converted to bytes during encryption");
            Console.WriteLine("The numbers will be extracted from the byte array and each will then be looked up on the similarly randomized encryption pad to find out which byte value it represents");
            Console.WriteLine("If the next number is less that the prior, it is an indicator that the encrypted pad should be encrypteed again using the same symmetric key before continuing");
            Console.WriteLine("The recovered byte values become the cyphertext encrypting the plaintext from the first stage of encryption which is decrypted using the symmetric encryption key to recover the original plaintext");
            //Console.WriteLine("Please press any key to continue");
            Console.BackgroundColor = ConsoleColor.Black;
            //Console.ReadKey();
        }

        inputFileName = ConsoleSupport.GetEncryptedDataFileName("png,enc", folderWithEncryptedFiles);

        Console.WriteLine("File " + inputFileName + " selected for decryption");

        if (inputFileName.EndsWith(".enc"))
        {
            if (tutorialMode)
            {
                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.WriteLine("The file contains the bytes of cyphertext which will be decrypted directly");
                //Console.WriteLine("Please press any key to continue");
                Console.BackgroundColor = ConsoleColor.Black;
                //Console.ReadKey();
            }

            Console.WriteLine("Reading contents of " + inputFileName + " for use as cyphertext");
            retrieveCyphertext = FacadeSupport.GetBytesFromFile(inputFileName, new ValidationSummary());
        }
        else
        {
            if (tutorialMode)
            {
                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.WriteLine("The file is a png image and contains the bytes of cyphertext"); 
                Console.WriteLine("They were either interlaced into an existing png image or a png image created to hold them OR");
                Console.WriteLine("They were used to construct the pixels of a new image created from them");
                Console.WriteLine("The program cannot tell which, but if the image is sharp it is likely interlaced and ");
                Console.WriteLine("if the image is a fuzzy pastel it is likely the cyphertext bytes created the image's pixels");
                //Console.WriteLine("Please press any key to continue");
                Console.BackgroundColor = ConsoleColor.Black;
                //Console.ReadKey();
            }


            Console.Write("Type Y if the cyphertext was obfuscated inside an image");
            ConsoleKeyInfo interlaced = Console.ReadKey();
            Console.WriteLine("");

            if (interlaced.KeyChar == 'Y' || interlaced.KeyChar == 'y')
            {
                Console.WriteLine("Interlacing selected");

                if (tutorialMode)
                {
                    Console.BackgroundColor = ConsoleColor.DarkBlue;
                    Console.WriteLine("The cyphertext was interlaced into the image and will be extracted before being decrypted");
                    //Console.WriteLine("Please press any key to continue");
                    Console.BackgroundColor = ConsoleColor.Black;
                    //Console.ReadKey();
                }

                Console.WriteLine("Extracting bits interlaced into pixels in " + inputFileName + " for use as cyphertext");
                retrieveCyphertext = InterlacingSupport.GetInterlacedEncryptedBytesFromRgba32PngFile(inputFileName, new ValidationSummary());
            }
            else
            {
                Console.WriteLine("Whole image selected");

                if (tutorialMode)
                {
                    Console.BackgroundColor = ConsoleColor.DarkBlue;
                    Console.WriteLine("The cyphertext bytes were used to create the pixels a new png format image file and will now be extracted from them."); 
                    //Console.WriteLine("Please press any key to continue");
                    Console.BackgroundColor = ConsoleColor.Black;
                    //Console.ReadKey();
                }

                Console.WriteLine("Extracting the pixels from " + inputFileName + " for use as cyphertext");
                retrieveCyphertext = ImageCreator.GetEncryptedBytesFromRgba32PngFile(inputFileName, new ValidationSummary());
            }
        }

        if (retrieveCyphertext.Failed)
        {
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Cyphertext was not Retrieved");
            Console.WriteLine(retrievePad.Snapshot);
            Console.Write("Please press any key to try again");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ReadKey();
            Console.WriteLine("");
        }
        else
        {
            Console.WriteLine("Cyphertext Retrieved");
        }

    }  // while retrieving cyphertext

    if (tutorialMode)
    {
        Console.BackgroundColor = ConsoleColor.DarkBlue;
        Console.WriteLine("The cyphertext is now retrieved and next we need to figure out when it was created do that we use the Passphrase and Secret Timestamp to create a symmetric encryption keys");
        Console.WriteLine("More than one symmetric encryption key and decryption attempt may be needed since we may not know the exact encryption time.  This is okay, the retry process is automated");
        //Console.WriteLine("Please press any key to continue");
        Console.BackgroundColor = ConsoleColor.Black;
        //Console.ReadKey();
    }

    while (!timestampValidation.Valid)
    {
        cypherTextCreation = ConsoleSupport.GetEncryptionDateTime(File.GetCreationTimeUtc(inputFileName));

        TimeBasedParamValidation.Validate(TimeBasedCrypterParam.EncryptionDateTime, cypherTextCreation, ref timestampValidation);

        if (timestampValidation.Valid)
        {
            Console.WriteLine("Timestamp Accepted");
        }
        else
        {
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Timestamp is Invalid");
            timestampValidation = new ValidationSummary();
            Console.Write("Please press any key to try again");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ReadKey();
            Console.WriteLine("");
        }

    } // while checking timestamp  

    if (tutorialMode)
    {
        Console.BackgroundColor = ConsoleColor.DarkBlue;
        Console.WriteLine("Now the program will try to decrypt the recovered cyphertext using a series of symmetric encryption keys based on:");
        Console.WriteLine("1) Timestamps near the encryption time");
        Console.WriteLine("2) The Secret Timestamp");
        Console.WriteLine("3) The Passphrase");
        Console.WriteLine("If successful, the symmetric encryption keys will have recovered the bytes storing the numeric look ups which point to byte values in the encryption pad");
        Console.WriteLine("If/when successful, the program will encrypt the pad bytes obtained from the pad file using the proven symmetric encryption key to create the same initial pad which was used for encryption");
        Console.WriteLine("Then it will reverse the pad encryption process by looking up the byte value corresponding to each numeric look up to recover the plaintext as it was initially encrypted for randomization");
        Console.WriteLine("If the ascending sequence of offsets in the pad resets to a lower value, the encrypted pad is re-encrypted using the same symmetric key to refresh the pad before the process continues");
        Console.WriteLine("The encrypted plaintext will then be decrypted using the same symmetric encryption key to recover the original plaintext completing the decryption");
        //Console.WriteLine("Please press any key to continue");
        Console.BackgroundColor = ConsoleColor.Black;
        //Console.ReadKey();
    }

    ResultObject decryption = UseTimeAndPadToStatically.Decrypt(retrieveCyphertext.Bytes, retrievePad.Bytes,
        passPhraseBytes, secretDateTime, cypherTextCreation, 1, encryptionSecondsIncludingStorage, 0, TimeBasedCryptionLimits.MinimumArgon2MemorySize,
        TimeBasedCryptionLimits.MinimumArgon2NumberOfPasses, new ValidationSummary());

    if (decryption.Failed)
    {
        Console.BackgroundColor = ConsoleColor.Red;
        Console.ForegroundColor = ConsoleColor.Black;
        Console.WriteLine("Decryption Failed");
        Console.WriteLine(decryption.Snapshot);
        // final
    }
    else
    {
        Console.WriteLine("Decryption Succeeded");

        decryptedFileExtension = FacadeSupport.GetFileExtensionFromBytes(ref decryption, MyUseIsNonCommercial);

        ResultObject writingOutput = FacadeSupport.WriteFileFromBytes(ref decryption, 
            folderWithDecryptedFiles, "", decryptedFileExtension, new ValidationSummary(), MyUseIsNonCommercial);

        if (writingOutput.Worked)
        {
            Console.BackgroundColor = ConsoleColor.Green;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("Please see decrypted file '" + writingOutput.FileName + "' here");
            showFileResult = NativeFileDialogSharp.Dialog.FileOpen(decryptedFileExtension.TrimStart('.'), folderWithDecryptedFiles);
            if (showFileResult.IsOk)
            {
                ConsoleSupport.OpenRecoveredFile(showFileResult.Path);
            }
            // final
        }
        else
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("Could not write Decrypted Data to file");
            Console.WriteLine(writingOutput.Snapshot);
            // final
        }


    } // end decryption

} // end decryption choice
else
{
    Console.WriteLine("Encrypting selected");

    byte[] plainTextBytes = [];

    if (tutorialMode)
    {
        Console.BackgroundColor = ConsoleColor.DarkBlue;
        Console.WriteLine("Now you will be asked for the text or file to encrypt. Whichever you provide will be converted to a byte array for encryption");
        Console.WriteLine("The bytes will be encrypted using the symmetric encryption key and each byte will then be looked up on the similarly randomized encryption pad");
        Console.WriteLine("The numeric offset indicating where each byte was found in the pad is converted to bytes which become the cyphertext encrypting the plaintext");
        Console.WriteLine("If the pad is not long enough, the encrypted pad is encrypted again using the same symmetric key to generate more pad so the process can continue");
        Console.WriteLine("Since the result is predictably an array of numbers converted to bytes, it is encrypted using the symmetric encryption key to create the final more random-looking cyphertext");
        //Console.WriteLine("Please press any key to continue");
        Console.BackgroundColor = ConsoleColor.Black;
        //Console.ReadKey();
    }

    Console.Write("Type T to encrypt text or F to encrypt a file ");
    input = Console.ReadKey();
    Console.WriteLine("");
    if (input.KeyChar == 'T' || input.KeyChar == 't')
    {
        Console.WriteLine("Text selected");

        if (tutorialMode)
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("Now you will be asked to enter the text to encrypt.");
            Console.WriteLine("Since it did not come from a file with a recognizable format it will be decrypted to a '.dat' (data) file");
            //Console.WriteLine("Please press any key to continue");
            Console.BackgroundColor = ConsoleColor.Black;
            //Console.ReadKey();
        }

        plainTextBytes = ConsoleSupport.GetTextMessage();

        Console.BackgroundColor = ConsoleColor.DarkMagenta;
        Console.WriteLine("When decrypted the message will be written into a '.dat' file since there is no actual file type to recover it into");
        Console.BackgroundColor = ConsoleColor.Black;

    }
    else
    {
        Console.WriteLine("File selected");

        if (tutorialMode)
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("Now you will be asked to select the file to encrypt.");
            Console.WriteLine("Its name and extension are not preserved during the encryption process");
            Console.WriteLine("The program can usually figure out its extension after it is decrypted and use it along with a random name");
            Console.WriteLine("If not, the program will warn you beforehand and it will be decrypted to a randomly named '.dat' (data) file");
            Console.WriteLine("Please press any key to continue");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ReadKey();
        }

        Console.WriteLine("The file must contain between " + PadBasedCryptionLimits.MinimumPlaintextBytes.ToString() + " and " + PadBasedCryptionLimits.MaximumPlaintextBytes.ToString() + " bytes");
        
        PlainTextInputFileObject fileObject = new PlainTextInputFileObject();

        while (!fileObject.Valid)
        {
            inputFileName = ConsoleSupport.GetNameOfFileToEncrypt(folderWithFilesToEncrypt);

            fileObject = new PlainTextInputFileObject(inputFileName, MyUseIsNonCommercial);

            if (fileObject.fileContents.Worked)
            {
                plainTextBytes = fileObject.bytes;
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Could not retrieve file" );
                Console.WriteLine(fileObject.error);
                Console.Write("Please press any key to try again");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ReadKey();
                Console.WriteLine("");
            }

                            
        } // while getting plaintext from file

        Console.WriteLine("Plaintext file " + inputFileName + " selected for encryption");

        if (!fileObject.SupportedFileType)
        {
            Console.BackgroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("File extension '" + inputFileName.Split('.').Last<string>() + "' not understood, will be recovered as a '.dat' file when decrypted");
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }

    ResultObject encryption = UseTimeAndPadToStatically.Encrypt(plainTextBytes, retrievePad.Bytes,
            passPhraseBytes, secretDateTime, TimeBasedCryptionLimits.MinimumArgon2MemorySize,
            TimeBasedCryptionLimits.MinimumArgon2NumberOfPasses, 3, new ValidationSummary());

    if (encryption.Failed)
    {
        Console.BackgroundColor = ConsoleColor.Red;
        Console.ForegroundColor = ConsoleColor.Black;
        Console.WriteLine("Encryption Failed");
        Console.WriteLine(encryption.Snapshot);
        // final
    }
    else
    {
        DateTime done = DateTime.UtcNow;
        Console.WriteLine("Encryption Succeeded at " + done.ToLongTimeString() + " on " + done.ToLongDateString() + " UTC");
        Console.WriteLine("Now we need to figure out how to store the result");

        if (tutorialMode)
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("The cyphertext might be hard to attach to an email as a binary file");
            Console.WriteLine("An alternative is to store the cyphertext in a png image file");
            Console.WriteLine("The cyphertext can either be used to create a new png image or can be interlaced into a new or existing png image");
            //Console.WriteLine("Please press any key to continue");
            Console.BackgroundColor = ConsoleColor.Black;
            //Console.ReadKey();
        }

        Console.WriteLine("Do you want the cyphertext to be obfuscated inside an image?");
        Console.Write("Type Y for yes or N for no ");
        input = Console.ReadKey();
        Console.WriteLine("");

        if (input.KeyChar == 'Y' || input.KeyChar == 'y')
        {
            Console.WriteLine("Obfuscation inside an image selected");

            if (tutorialMode)
            {
                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.WriteLine("Now you will be asked whether or not you want to provide the image inside of which the cyphertext will interlaced");
                Console.WriteLine("If you select the image it can be of something realistic BUT you should use it just once and destroy the image you select it is used");
                Console.WriteLine("If you allow the image to be generated for you, the artwork will be crude but colorful until we hook up an AI image generator to this");
                Console.WriteLine("When queried while decrypting, remember that the cyphertext bytes ARE INTERLACED in this png image");
                //Console.WriteLine("Please press any key to continue");
                Console.BackgroundColor = ConsoleColor.Black;
                //Console.ReadKey();
            }

            Console.WriteLine("Do you want to select the image?");
            Console.Write("Type Y for yes or N for no ");
            input = Console.ReadKey();
            Console.WriteLine("");

            if (input.KeyChar == 'Y' || input.KeyChar == 'y')
            {
                Console.WriteLine("Model file selection chosen");

                if (tutorialMode)
                {
                    Console.BackgroundColor = ConsoleColor.DarkBlue;
                    Console.WriteLine("Now you will be asked to select a png format image file of a certain size range.  It will enlarged as required to hold the cyphertext");
                    Console.WriteLine("The image selected must contain between " + ImageLimits.MinimumModelPngPixels.ToString() + " and " + ImageLimits.MaximumCryptionPadPngPixels.ToString() + " pixels");
                    Console.WriteLine("The number of pixels in an image can be predetermined by multiplying the image's height and width");
                    Console.WriteLine("When queried while decrypting, remember that the cyphertext bytes ARE INTERLACED in this png image");
                    //Console.WriteLine("Please press any key to continue");
                    Console.BackgroundColor = ConsoleColor.Black;
                    //Console.ReadKey();
                }

                while (createFileFromModel.Failed)
                {
                    Console.WriteLine("Please select an image in png format");
                    Console.WriteLine("The image selected must contain between " + ImageLimits.MinimumModelPngPixels.ToString() + " and " + ImageLimits.MaximumCryptionPadPngPixels.ToString() + " pixels");
                    string modelFileName = ConsoleSupport.GetModelFileName(folderWithPossibleModelFiles);

                    Console.WriteLine("Model file " + modelFileName + " selected");

                    createFileFromModel = ImageProcessor.CreateRgba32PngByInterlacingEncryptedBytesIntoModelFile(modelFileName,
                        encryption.Bytes, ImageOutputFormat.file, folderWithEncryptedFiles, new ValidationSummary());
                                       
                    if (createFileFromModel.Worked)
                    {
                        Console.BackgroundColor = ConsoleColor.Green;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.WriteLine("Please see successfully encrypted and obfuscated file '" + createFileFromModel.FileName + "' here");
                        Console.WriteLine("When queried while decrypting, remember that the cyphertext bytes ARE INTERLACED in this png image");
                        showFileResult = NativeFileDialogSharp.Dialog.FileOpen(decryptedFileExtension, folderWithEncryptedFiles);
                        if (showFileResult.IsOk)
                        {
                            ConsoleSupport.OpenRecoveredFile(showFileResult.Path);
                        }
                        // final

                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine("Could not obfuscate successfully encrypted file");
                        Console.WriteLine(createFileFromModel.Snapshot);
                        Console.Write("Please press any key to try again");
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ReadKey();
                        Console.WriteLine("");
                    }
                }                      
            }
            else
            {
                Console.WriteLine("Model file generation chosen");

                if (tutorialMode)
                {
                    Console.BackgroundColor = ConsoleColor.DarkBlue;

                    Console.WriteLine("The program will generate a crude but colorful and properly sized image inside of which the cyphertext will interlaced");
                    Console.WriteLine("When queried while decrypting, remember that the cyphertext bytes ARE INTERLACED in this png image");
                    Console.WriteLine("Please press any key to continue");
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ReadKey();
                }

                Console.WriteLine("An image will be generated to accept bits of cyphertext interlaced into its pixels");
                                
                ResultObject createGeneratedFile = ImageGenerator.CreateRgba32PngByInterlacingEncryptedBytes(encryption.Bytes,
                "scritch scratch", ImageOutputFormat.file, folderWithEncryptedFiles, new ValidationSummary());

                if (createGeneratedFile.Worked)
                {
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine("Please see successfully encrypted and obfuscated file '" + createGeneratedFile.FileName + "' here");
                    Console.WriteLine("When queried while decrypting, remember that the cyphertext bytes ARE INTERLACED in this png image");
                    showFileResult = NativeFileDialogSharp.Dialog.FileOpen(decryptedFileExtension, folderWithEncryptedFiles);
                    if (showFileResult.IsOk)
                    {
                        ConsoleSupport.OpenRecoveredFile(showFileResult.Path);
                    }
                    // final
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine("Could not obfuscate successfully encrypted file");
                    Console.WriteLine(createGeneratedFile.Snapshot);
                    // final
                }
            }
        }
        else
        {
            Console.WriteLine("Write only cyphertext selected");

            if (tutorialMode)
            {
                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.WriteLine("Now you will be asked whether the cyphertext should be stored in a new binary '.enc' encrypted file or be used to make the pixels in a new png image file");
                Console.WriteLine("If you select the png image format the cyphertext will be stored as pixels in a png image file which will typically appear to be a weird pastel");
                Console.WriteLine("To attachment format filters is is just another png, to a human or AI which is filtering images it might look strange");
                Console.WriteLine("If you select the binary format the cyphertext will be stored as bytes in a '.enc' file which can be easily identified as cyphertext");
                //Console.WriteLine("Please press any key to continue");
                Console.BackgroundColor = ConsoleColor.Black;
                //Console.ReadKey();
            }

            Console.WriteLine("Do you want the cyphertext converted to an image?");
            Console.Write("Type Y for yes or N for no ");
            input = Console.ReadKey();
            Console.WriteLine("");
            if (input.KeyChar == 'Y' || input.KeyChar == 'y')
            {
                Console.WriteLine("Cyphertext as image chosen");

                if (tutorialMode)
                {
                    Console.BackgroundColor = ConsoleColor.DarkBlue;
                    Console.WriteLine("In this case the cyphertext will be stored as pixels in a png image file which will typically appear to be a weird pastel");
                    Console.WriteLine("To attachment format filters is is just another png, to a human or AI which is filtering images it might look strange");
                    Console.WriteLine("When queried while decrypting, remember that the cyphertext bytes ARE NOT INTERLACED in this png image");
                    //Console.WriteLine("Please press any key to continue");
                    Console.BackgroundColor = ConsoleColor.Black;
                    //Console.ReadKey();
                }

                Console.WriteLine("A new image will be created using the cyphertext as its pixels");

                ResultObject createNewPngFile = ImageCreator.CreateRgba32PngFromEncryptedBytes(encryption.Bytes, 
                    ImageOutputFormat.file, folderWithEncryptedFiles, new ValidationSummary());
                                    
                if (createNewPngFile.Worked)
                {
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine("Please see successfully encrypted and obfuscated file '" + createNewPngFile.FileName + "' here");
                    Console.WriteLine("When queried while decrypting, remember that the cyphertext bytes ARE NOT INTERLACED in this png image");
                    showFileResult = NativeFileDialogSharp.Dialog.FileOpen(decryptedFileExtension, folderWithEncryptedFiles);
                    if (showFileResult.IsOk)
                    {
                        ConsoleSupport.OpenRecoveredFile(showFileResult.Path);
                    }
                    // final
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine("Could not obfuscate successfully encrypted file");
                    Console.WriteLine(createNewPngFile.Snapshot);
                    // final
                }
            }
            else
            {
                Console.WriteLine("Cyphertext as binary chosen");

                if (tutorialMode)
                {
                    Console.BackgroundColor = ConsoleColor.DarkBlue;
                    Console.WriteLine("In this case the cyphertext will be stored as bytes in a '.enc' file which can be easily identified as cyphertext");
                    //Console.WriteLine("Please press any key to continue");
                    Console.BackgroundColor = ConsoleColor.Black;
                    //Console.ReadKey();
                }

                Console.WriteLine("The cyphertext will remain cyphertext");

                ResultObject writeEncryptedFile = FacadeSupport.WriteFileFromBytes(ref encryption, 
                    folderWithEncryptedFiles, "", encryptedFileExtention, new ValidationSummary(), MyUseIsNonCommercial);

                if (writeEncryptedFile.Worked)
                {
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine("Please see successfully encrypted file '" + writeEncryptedFile.FileName + "' here");
                    showFileResult = NativeFileDialogSharp.Dialog.FileOpen(decryptedFileExtension, folderWithEncryptedFiles);
                    if (showFileResult.IsOk)
                    {
                        ConsoleSupport.OpenRecoveredFile(showFileResult.Path);
                    }
                    // final
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine("Could not write successfully encrypted file");
                    Console.WriteLine(writeEncryptedFile.Snapshot);
                    // final
                }
            }
        }

    } // end encryption succeeded

} // end encryption choice

// in either case...
Console.Write("Please press any key to exit");
Console.ReadKey();
