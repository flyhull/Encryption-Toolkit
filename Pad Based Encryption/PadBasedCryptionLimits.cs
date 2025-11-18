using Common_Support;
using Time_Based_Encryption;

namespace Pad_Based_Encryption
{
    
    public static class PadBasedCryptionLimits
    {
        public const int MaximumCyphertextBytes = Int32.MaxValue;
        public const int MaximumEncryptionAttempts = 10;

        public static Int64 MaximumPlaintextBytes
        {
            get { return CommonSupport.PracticalInt32Max / 1024; }
        }

        public static Int32 MinimumCryptionPadBytes
        {
            get { return (Int32)(UInt16.MaxValue / 4); }
        }

        public static Int32 MaximumCryptionPadBytes
        {
            get { return CommonSupport.PracticalInt32Max / 64; }
        }

        public static Int32 MinimumPlaintextBytes
        {
            get { return CommonSupport.PracticalInt32Min; }
        }

        public static Int32 MinimumCyphertextBytes
        {
            get { return CommonSupport.PracticalInt32Min; }
        }

        public static Int32 MinimumEncryptionAttempts
        {
            get { return CommonSupport.PracticalInt32Min; }
        }

        public static string ShowLimits()
        {
            List<string> limits = new List<string>();

            limits.Add("** Pad Based Cryption Limits **");

            limits.Add("MinimumPlaintextBytes:" + MinimumPlaintextBytes.ToString());
            limits.Add("MaximumPlaintextBytes:" + MaximumPlaintextBytes.ToString());
            limits.Add("MinimumCyphertextBytes: " + MinimumCyphertextBytes.ToString());
            limits.Add("MaximumCyphertextBytes: " + MaximumCyphertextBytes.ToString());
            limits.Add("MaximumEncryptionAttempts: " + MaximumEncryptionAttempts.ToString());
            limits.Add("MinimumEncryptionAttempts: " + MinimumEncryptionAttempts.ToString());
            limits.Add("MinimumCryptionPadBytes: " + MinimumCryptionPadBytes.ToString());
            limits.Add("MaximumCryptionPadBytes: " + MaximumCryptionPadBytes.ToString());

            return string.Join(Environment.NewLine, limits.ToArray());
        }
    }
}
