using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using Image_Support;
using Time_Based_Encryption;
using Pad_Based_Encryption;


//Pad_Based_Encryption.staticly encrypt
//Pad_Based_Encryption.staticly decrypt


internal class Program
{
    private static void Main(string[] args)
    {
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder
                .AddFilter("Microsoft", LogLevel.Warning)
                .AddFilter("System", LogLevel.Warning)
                .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug)
                .AddConsole();
        });

        ILogger logger = loggerFactory.CreateLogger<Program>();

        Console.Title = "Non Mathematical Encryption Tests";
        Console.BackgroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine("This program executes the test plan");

        Console.WriteLine(TimeBasedCryptionLimits.ShowLimits());
        Console.WriteLine(PadBasedCryptionLimits.showLimits());
        Console.WriteLine("Creating new png out of cyphertext");
        Console.WriteLine(ImageLimits.ShowLimits(4));
        Console.WriteLine("Interlacing cyphertext into png");
        Console.WriteLine(ImageLimits.ShowLimits(1));


    }

}