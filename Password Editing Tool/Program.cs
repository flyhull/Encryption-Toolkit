using Microsoft.Extensions.Logging;
using Facade_Support;
using Time_Based_Encryption;
using Pad_Based_Encryption;
using Image_Support;
using Common_Support;
using Console_Support;
using Obfuscation_in_Existing_PNG;
using SixLabors.ImageSharp;
using Password_Editing_Tool;

byte[] passPhraseBytes = Array.Empty<byte>();
//string imageFileExtention = ".png";
//string encryptedFileExtention = ".enc";
//string? dateTimeString = string.Empty;
string? logoFileName = string.Empty;
bool logoFileIsEmpty = true;
bool haveLogoFile = false;
ImageProfile logoInfo;
string payload = string.Empty;
//string? decryptedFileExtension = string.Empty;
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


//The System.Environment class has two GetFolderPath overloads:
//public static string GetFolderPath (SpecialFolder folder);
//public static string GetFolderPath (SpecialFolder folder, SpecialFolderOption option);
//SpecialFolder is an enum with values like ApplicationData, MyDocuments, and ProgramFiles.
//The SpecialFolderOption enum has three //values: None, Create, and DoNotVerify. These control the return value when the folder does not exist. Specifying None causes an //empty string to be returned. Specifying Create causes the folder to be created. And DoNotVerify causes the path to be returned //even when the folder does not exist.

string folderWithPossibleLogoFiles = System.Environment.SpecialFolder.MyPictures.ToString();

//using var loggerFactory = LoggerFactory.Create(builder =>
//{
//    builder
//        .AddFilter("Microsoft", LogLevel.Warning)
//        .AddFilter("System", LogLevel.Warning)
//        .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug)
//        .AddConsole();
//});

//ILogger logger = loggerFactory.CreateLogger<Program>();

Console.Title = "Password Editing Tool";
Console.WindowWidth = 220;
Console.WriteLine("Welcome to the Password Editing Tool");
Console.WriteLine("First Enter Cryption Keys");

while (retrievePad.Failed)
{    
    passPhraseBytes = ConsoleSupport.GetPassphrase();

    secretDateTime = ConsoleSupport.GetSecretDateTime();

    List<string> RemovableDrives = ImageSupport.GetRemovableVolumes();
    if (RemovableDrives.Count > 0)
    {
        Console.WriteLine("Please select a Pad image in png format to encrypt the password information");
        Console.WriteLine("The image selected must contain between " + ImageLimits.MinimumCryptionPadPngPixels.ToString() + " and " + ImageLimits.MaximumCryptionPadPngPixels.ToString() + " pixels and reside at the root of a removable drive");
        Console.WriteLine("Please press any key when ready");
        Console.ReadKey();

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
    }

} // while entering credentials and retrieving pad

while (!haveLogoFile)
{
    Console.WriteLine("Please select a Logo image in png format to hold the password information");
    Console.WriteLine("The image selected must contain between " + ImageLimits.MinimumModelPngPixels.ToString() + " and " + ImageLimits.MaximumCryptionPadPngPixels.ToString() + " pixels");
    Console.WriteLine("Please press any key when ready");
    Console.ReadKey();

    logoFileName = ConsoleSupport.GetModelFileName(folderWithPossibleLogoFiles);

    if (!string.IsNullOrEmpty(logoFileName))
    {
        if (ImageSupport.IsPng(logoFileName))
        {
            haveLogoFile = true;
            break;
        }
    }
    
    Console.BackgroundColor = ConsoleColor.DarkRed;
    Console.WriteLine(logoFileName + " is in the wrong format");
    Console.Write("Please press any key to try again");
    Console.BackgroundColor = ConsoleColor.Black;
    Console.ReadKey();
    Console.WriteLine("");
}

Console.WriteLine("Logo file " + logoFileName + " selected");

logoInfo = new ImageProfile(logoFileName);

if (logoInfo.IsPng32)
{
    // might not be new
    retrieveCyphertext = InterlacingSupport.GetInterlacedEncryptedBytesFromRgba32PngFile(logoFileName, new ValidationSummary());
    if (retrieveCyphertext.Worked)
    {
        cypherTextCreation = File.GetLastWriteTimeUtc(logoFileName);

        ResultObject decryption = UseTimeAndPadToStatically.Decrypt(retrieveCyphertext.Bytes, retrievePad.Bytes,
            passPhraseBytes, secretDateTime, cypherTextCreation, 1, encryptionSecondsIncludingStorage, 0, TimeBasedCryptionLimits.MinimumArgon2MemorySize,
            TimeBasedCryptionLimits.MinimumArgon2NumberOfPasses, new ValidationSummary());

        if (decryption.Worked)
        {
            ResultObject getPayload = FacadeSupport.GetStringFromBytes(decryption.Bytes, new ValidationSummary());
            if (getPayload.Worked)
            {
                logoFileIsEmpty = false;
                payload = getPayload.Base64String;
            }

        }
        else
        {
            Console.WriteLine("Could not decrypt the data retrieved from the image using the credentials provided");
            Console.WriteLine("Either this logo has never been used OR it has been used before but with different credentials");
        }
    }
    else
    {
        Console.WriteLine("Could not retrieve data from image");
    }
}
else
{
    Console.WriteLine("Logo has never been used before");
}

ConsoleKeyInfo input = new ConsoleKeyInfo();
PasswordData storedData = new PasswordData();

if (logoFileIsEmpty)
{
    Console.WriteLine("These Is No Data Available");
    Console.Write("Type A to add or X to exit ");
    input = Console.ReadKey();
    Console.WriteLine("");
}
else
{
    Console.WriteLine("Previously Stored Data Retrieved");
        
    storedData = new PasswordData(payload);
    Console.WriteLine(storedData.Stringified);

    Console.Write("Type R to replace or X to exit ");
    input = Console.ReadKey();
    Console.WriteLine("");
}

string? scratch = string.Empty;

switch (input.KeyChar)
{
    case 'A':
    case 'a':
    case 'R':
    case 'r':
        foreach (Topic item in Enum.GetValues<Topic>())
        {
            Console.WriteLine(string.Concat(item.ToString(), ": ", storedData.getValue(item) ));
            Console.Write("New value? ");
            scratch = Console.ReadLine();
            if (!string.IsNullOrEmpty(scratch))
            {
                storedData.setValue(item, scratch);
            } 
        }
        payload = storedData.Stringified;
        break;

    default:
        break;
}

if (storedData.NoSave)
{
    Console.WriteLine("Exiting without making changes");
}
else
{
    Console.WriteLine("Saving changes");

    ResultObject getPlainText = FacadeSupport.GetBytesFromString(payload, new ValidationSummary());

    if (getPlainText.Failed)
    {
        Console.BackgroundColor = ConsoleColor.Red;
        Console.ForegroundColor = ConsoleColor.Black;
        Console.WriteLine("Getting Plaintext Failed");
        Console.WriteLine(getPlainText.Snapshot);
        // final
    }
    else
    {
        ResultObject encryption = UseTimeAndPadToStatically.Encrypt(getPlainText.Bytes, retrievePad.Bytes,
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
        
            FileInfo fi = new FileInfo(logoFileName);

            createFileFromModel = ImageProcessor.CreateRgba32PngByInterlacingEncryptedBytesIntoModelFile(logoFileName,
                encryption.Bytes, ImageOutputFormat.file, fi.DirectoryName ?? "", new ValidationSummary(), fi.Name);

            if (createFileFromModel.Worked)
            {
                Console.BackgroundColor = ConsoleColor.Green;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine("Please updated Logo File '" + createFileFromModel.FileName + "' here");
                showFileResult = NativeFileDialogSharp.Dialog.FileOpen(fi.Extension.TrimStart('.'), fi.DirectoryName ?? "");
                if (showFileResult.IsOk)
                {
                    ConsoleSupport.OpenRecoveredFile(showFileResult.Path);
                }
                // final
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Could not store successfully encrypted data");
                Console.WriteLine(createFileFromModel.Snapshot);
                // final       
            }

        } // end encryption succeeded

    } // end get plain text succeeded

} // end changes were made

// in any case...
Console.Write("Please press any key to exit");
Console.ReadKey();
