using Common_Support;
using Pad_Based_Encryption;
using Time_Based_Encryption;
using Testing_Support;

namespace Pad_Based_Encryption_Tests
{
    [TestClass]
    public sealed class MaximumPadBasedTests
    {
        [TestMethod]
        public void BytesToPadEncryptedBytesToBytes()
        {
            Console.WriteLine(TimeBasedCryptionLimits.ShowLimits());
            Console.WriteLine(PadBasedCryptionLimits.ShowLimits());

            //// arrange
            ResultObject result = new ResultObject();
            TimeStampObject secretDate = new TimeStampObject("2009-06-15T13:45:30");
            byte[] plaintext = TestingSupport.GetRandomBytes(PadBasedCryptionLimits.MaximumPlaintextBytes);
            byte[] cryptionPad = TestingSupport.GetRandomBytes(PadBasedCryptionLimits.MaximumCryptionPadBytes);
            byte[] passphrase = TestingSupport.GetRandomBytes(TimeBasedCryptionLimits.MaximumPassPhraseLength);
            Int32 memSize =  TimeBasedCryptionLimits.MaximumArgon2MemorySize;
            Int32 passes =  TimeBasedCryptionLimits.MaximumArgon2NumberOfPasses;

            memSize = TimeBasedCryptionLimits.MinimumArgon2MemorySize;
            passes = TimeBasedCryptionLimits.MinimumArgon2NumberOfPasses;

            //memSize = 31337;
            //passes = 1337;

            //// act
            if (secretDate.Valid)
            {
                ResultObject intermediate = Pad_Based_Encryption.UseTimeAndPadToStatically.Encrypt(plaintext, cryptionPad, passphrase, secretDate.TimeStampValue, memSize, passes, 3, new ValidationSummary());
                
                if (intermediate.Worked && intermediate.WroteBytes)
                {
                    result = Pad_Based_Encryption.UseTimeAndPadToStatically.Decrypt(intermediate.Bytes, cryptionPad, passphrase, secretDate.TimeStampValue, DateTime.UtcNow,
                        2, 1024, 0, memSize, passes, new ValidationSummary());
                    Console.WriteLine(result.Snapshot);
                }
                else
                {
                    Console.WriteLine(intermediate.Snapshot);
                }
            }

            //// assert
            Assert.IsTrue(result.Worked);
            Assert.IsTrue(result.WroteBytes);
            Assert.AreEqual<int>(plaintext.Length, result.Bytes.Length);
            Assert.AreEqual<string>(TestingSupport.GetHashOfBytes(plaintext).Base64String, TestingSupport.GetHashOfBytes(result.Bytes).Base64String);

        }
    }
}
