using Common_Support;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Time_Based_Encryption
{    


    public static class TimeBasedCryptionLimits
    {
        public const int MinimumPassPhraseLength = 42;
        public const int MinimumArgon2MemorySize = 24000;
        public const int MinimumArgon2NumberOfPasses = 4;

        public const int MaximumPassPhraseLength = 2048;
        public const int MaximumArgon2MemorySize = 96000;
        public const int MaximumArgon2NumberOfPasses = 2048;

        public const int MaximumPlusMinusSeconds = 10;
        public const int MaximumGoBackSeconds = 2048;
        public const int MaximumGuaranteedLagSeconds = 120;

        public static DateTime EarliestSecretDate
        {
            get { return new DateTime(410, 8, 24, 0, 0, 0); }
        }
        public static DateTime LatestSecretDate
        {
            get { return new DateTime(2100, 1, 1, 0, 0, 0); }
        }
        public static DateTime EarliestEncryptionDate
        {
            get { return new DateTime(2025, 1, 1, 0, 0, 0); }
        }
        public static DateTime LatestEncryptionDate
        {
            get { return DateTime.UtcNow.AddMinutes(10); }
        }
        public static Int32 MaximumCyphertextBytes
        {
            get { return Int32.MaxValue; }
        }
        public static Int32 MaximumPlaintextBytes
        {
            get { return (Int32)(Int32.MaxValue / 4); }
        }


        public static Int32 MinimumCyphertextBytes
        {
            get { return CommonSupport.PracticalInt32Min; }
        }

        public static Int32 MinimumPlaintextBytes
        {
            get { return CommonSupport.PracticalInt32Min; }
        }

        public static Int32 MinimumPlusMinusSeconds
        {
            get { return CommonSupport.PracticalInt32Min; }
        }

        public static Int32 MinimumGoBackSeconds
        {
            get { return CommonSupport.PracticalInt32Min; }
        }
        public static Int32 MinimumGuaranteedLagSeconds
        {
            get { return 0; }
        }

        public static string ShowLimits()
        {
            List<string> limits = new List<string>();

            limits.Add("** Time Based Cryption Limits **");

            limits.Add("PassPhraseMinimumLength:" + MinimumPassPhraseLength.ToString());
            limits.Add("PassPhraseMaximumLength:" + MaximumPassPhraseLength.ToString());
            limits.Add("MinimumArgon2MemorySize: " + MinimumArgon2MemorySize.ToString());
            limits.Add("MaximumArgon2MemorySize: " + MaximumArgon2MemorySize.ToString());
            limits.Add("MinimumArgon2NumberOfPasses: " + MinimumArgon2NumberOfPasses.ToString());
            limits.Add("MaximumArgon2NumberOfPasses: " + MaximumArgon2NumberOfPasses.ToString());
            limits.Add("EarlistSecretDate: " + EarliestSecretDate.ToString());
            limits.Add("LatestSecretDate: " + LatestSecretDate.ToString());
            limits.Add("EarlistEncryptionDate: " + EarliestEncryptionDate.ToString());
            limits.Add("LatestEncryptionDate: " + LatestEncryptionDate.ToString());
            limits.Add("MinimumCyphertextBytes: " + MinimumCyphertextBytes.ToString());
            limits.Add("MaximumCyphertextBytes: " + MaximumCyphertextBytes.ToString());
            limits.Add("MinimumPlaintextBytes: " + MinimumPlaintextBytes.ToString());
            limits.Add("MaximumPlaintextBytes: " + MaximumPlaintextBytes.ToString());
            limits.Add("MinimumPlusMinusSeconds:" + MinimumPlusMinusSeconds.ToString());
            limits.Add("MaximumPlusMinusSeconds:" + MaximumPlusMinusSeconds.ToString());
            limits.Add("MinimumGoBackSeconds:" + MinimumGoBackSeconds.ToString());
            limits.Add("MaximumGoBackSeconds:" + MaximumGoBackSeconds.ToString());
            limits.Add("MinimumGuaranteedLagSeconds: " + MinimumGuaranteedLagSeconds.ToString());
            limits.Add("MaximumGuaranteedLagSeconds: " + MaximumGuaranteedLagSeconds.ToString());

            return string.Join(Environment.NewLine, limits.ToArray());
        }
    }

   
}
