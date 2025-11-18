using Common_Support;
using Time_Based_Encryption;
using Testing_Support;

namespace Time_Based_Encryption_Tests
{
    [TestClass]
    public sealed class MaximumTimeBasedTests
    {
        [TestMethod]
        public void BytesToTimeEncryptedBytesToBytes()
        {
            Console.WriteLine(TimeBasedCryptionLimits.ShowLimits());

            //// arrange
            ResultObject result = new ResultObject();
            TimeStampObject secretDate = new TimeStampObject("2009-06-15T13:45:30");
            byte[] plaintext = TestingSupport.GetRandomBytes(TimeBasedCryptionLimits.MaximumPlaintextBytes);
            byte[] passphrase = TestingSupport.GetRandomBytes(TimeBasedCryptionLimits.MaximumPassPhraseLength);
            Int32 memSize = TimeBasedCryptionLimits.MaximumArgon2MemorySize;
            Int32 passes =  TimeBasedCryptionLimits.MaximumArgon2NumberOfPasses;

            //memSize = TimeBasedCryptionLimits.MinimumArgon2MemorySize;
            //passes = TimeBasedCryptionLimits.MinimumArgon2NumberOfPasses;

            //memSize = 31337;
            //passes = 1337;

            //// act
            if (secretDate.Valid)
            {            
                ResultObject intermediate = UseTimeToStatically.Encrypt(plaintext, passphrase, secretDate.TimeStampValue, memSize, passes , new ValidationSummary());
                if (intermediate.Worked && intermediate.WroteBytes)
                {
                    result = UseTimeToStatically.Decrypt(intermediate.Bytes, passphrase, secretDate.TimeStampValue, DateTime.UtcNow,
                        2, 120, 0, memSize, passes, new ValidationSummary());
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
            Assert.AreEqual<int>(plaintext.Length,result.Bytes.Length);
            Assert.AreEqual<string>(TestingSupport.GetHashOfBytes(plaintext).Base64String, TestingSupport.GetHashOfBytes(result.Bytes).Base64String);


        }
    }
}
