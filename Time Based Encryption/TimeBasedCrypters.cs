// Ignore Spelling: Crypter

using Common_Support;
using NSec.Cryptography;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Time_Based_Encryption
{
    public class UseTimeToStatically
    {
        public static ResultObject Encrypt(byte[] plainText, byte[] passPhrase, DateTime secretDateTime, Int32 argon2MemorySize, Int32 argon2NumberOfPasses, ValidationSummary validation)
        {
            return Encrypt( plainText,  passPhrase,  secretDateTime, DateTime.UtcNow, argon2MemorySize, argon2NumberOfPasses, validation);
        }
        public static ResultObject Encrypt(byte[] plainText, byte[] passPhrase, DateTime secretDateTime, DateTime encryptionTime, Int32 argon2MemorySize, Int32 argon2NumberOfPasses, ValidationSummary validation)
        {

            ResultObject result = new ResultObject();

            Stopwatch sw = new Stopwatch();

            sw.Start();

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

            try
            {
                TimeBasedParamValidation.Validate(TimeBasedCrypterParam.Passphrase, passPhrase, ref validation);
                TimeBasedParamValidation.Validate(TimeBasedCrypterParam.SecretDateTime, secretDateTime, ref validation);
                TimeBasedParamValidation.Validate(TimeBasedCrypterParam.ArgonMemorySize, argon2MemorySize, ref validation);
                TimeBasedParamValidation.Validate(TimeBasedCrypterParam.ArgonNumberOfPasses, argon2NumberOfPasses, ref validation);

                TimeBasedParamValidation.Validate(TimeBasedCrypterParam.Plaintext, plainText, ref validation);

                if (validation.Valid)
                {
                    using (TimeBasedCrypter Crypter = new TimeBasedCrypter(passPhrase, encryptionTime, secretDateTime, argon2MemorySize, argon2NumberOfPasses, validation))
                    {
                        result = Crypter.Encrypt(plainText, validation);
                    }
                }
                else
                {
                    result = new ResultObject(validation);
                }

            }
            catch (Exception ex)
            {
                result = new ResultObject(ex, activity);
            }

            sw.Stop();

            Console.WriteLine("Encryption took " + sw.ElapsedMilliseconds.ToString() + " milliseconds");

            return result;

        }

        public static ResultObject Decrypt(byte[] cypherText, byte[] passPhrase, DateTime secretDateTime, DateTime encryptionDateTime, Int32 PlusMinusSeconds, Int32 GoBackSeconds, Int32 GuaranteedLagSeconds, Int32 argon2MemorySize, Int32 argon2NumberOfPasses, ValidationSummary validation)
        {

            ResultObject result = new ResultObject();

            Stopwatch sw = new Stopwatch();

            sw.Start();

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

            try
            {
                TimeBasedParamValidation.Validate(TimeBasedCrypterParam.Cyphertext, cypherText, ref validation);
                TimeBasedParamValidation.Validate(TimeBasedCrypterParam.Passphrase, passPhrase, ref validation);
                TimeBasedParamValidation.Validate(TimeBasedCrypterParam.EncryptionDateTime, encryptionDateTime, ref validation);
                TimeBasedParamValidation.Validate(TimeBasedCrypterParam.SecretDateTime, secretDateTime, ref validation);
                TimeBasedParamValidation.Validate(TimeBasedCrypterParam.ArgonMemorySize, argon2MemorySize, ref validation);
                TimeBasedParamValidation.Validate(TimeBasedCrypterParam.ArgonNumberOfPasses, argon2NumberOfPasses, ref validation);

                TimeBasedParamValidation.Validate(TimeBasedCrypterParam.PlusMinusSeconds, PlusMinusSeconds, ref validation);
                TimeBasedParamValidation.Validate(TimeBasedCrypterParam.GoBackSeconds, GoBackSeconds, ref validation);
                TimeBasedParamValidation.Validate(TimeBasedCrypterParam.GuaranteedLagSeconds, GuaranteedLagSeconds, ref validation);

                if (validation.Valid)
                {

                    for (int i = -1 * PlusMinusSeconds + GuaranteedLagSeconds; i <= PlusMinusSeconds + GoBackSeconds + GuaranteedLagSeconds; i++)
                    {
                        using (TimeBasedCrypter worker = new TimeBasedCrypter(passPhrase, encryptionDateTime.AddSeconds(-1 * i), secretDateTime, argon2MemorySize, argon2NumberOfPasses, validation))
                        {
                            result = new ResultObject(worker.Decrypt(cypherText, validation).Bytes, encryptionDateTime.AddSeconds(-1 * i));
                        }

                        if (result.Worked)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    result = new ResultObject(validation);
                }

            }
            catch (Exception ex)
            {
                result = new ResultObject(ex, activity);
            }

            sw.Stop();

            Console.WriteLine("All decryption attempts collectively took " + sw.ElapsedMilliseconds.ToString() + " milliseconds");

            return result;

        }

    }

    public class TimeBasedCrypter : IDisposable
    {
        // normal properties

        private Aegis256 crypter = new Aegis256();
        private AegisSecrets crypterKeys;
        public DateTime? timestamp = null;

        private bool disposedValue;

        public TimeBasedCrypter(byte[] passPhrase, DateTime encryptionDateTime, DateTime secretDateTime, Int32 memorySize, Int32 numberOfPasses, ValidationSummary validation)
        {
            TimeBasedParamValidation.Validate(TimeBasedCrypterParam.Passphrase, passPhrase,ref validation);
            TimeBasedParamValidation.Validate(TimeBasedCrypterParam.EncryptionDateTime, encryptionDateTime, ref validation);
            TimeBasedParamValidation.Validate(TimeBasedCrypterParam.SecretDateTime, secretDateTime, ref validation);
            TimeBasedParamValidation.Validate(TimeBasedCrypterParam.ArgonMemorySize, memorySize, ref validation);
            TimeBasedParamValidation.Validate(TimeBasedCrypterParam.ArgonNumberOfPasses, numberOfPasses, ref validation);
            
            if (validation.Valid)
            {
                using (DeriveAegisSecrets crypterParam = new DeriveAegisSecrets(passPhrase, encryptionDateTime, secretDateTime, crypter.KeySize, crypter.NonceSize, crypter.TagSize, memorySize, numberOfPasses))
                {
                    timestamp = encryptionDateTime;
                    crypterKeys = new AegisSecrets(Key.Import(crypter, (ReadOnlySpan<byte>)crypterParam.key, KeyBlobFormat.RawSymmetricKey), crypterParam.nonce, crypterParam.authTag);
                }
            }
            else
            {
                throw validation.GetException();
            }
            
        }

        public ResultObject Encrypt(byte[] plainText, ValidationSummary validation )
        {
            ResultObject result = new ResultObject();

            TimeBasedParamValidation.Validate(TimeBasedCrypterParam.Plaintext, plainText, ref validation);
            
            if (validation.Valid)
            {
                result = new ResultObject(crypter.Encrypt(crypterKeys.safeKey, crypterKeys.nonce, crypterKeys.authTag, plainText), timestamp);
            }
            else
            {
                result = new ResultObject(validation);
            }
            
            return result;
        }

        public ResultObject Decrypt(byte[] cypherText, ValidationSummary validation)
        {
            ResultObject result = new ResultObject();

            TimeBasedParamValidation.Validate(TimeBasedCrypterParam.Cyphertext, cypherText, ref validation);
            
            if (validation.Valid)
            {
                result = new ResultObject(crypter.Decrypt(crypterKeys.safeKey, crypterKeys.nonce, crypterKeys.authTag, cypherText), timestamp);
            }
            else
            {
                result = new ResultObject(validation);
            }

            return result;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Redact();
                }

                disposedValue = true;
            }
        }

        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public void Redact()
        {
            //TODO Pin? Fix?
            crypter = new Aegis256();

            //TODO Pin and fix
            timestamp = null;

            crypterKeys.Redact();
        }

    }
}
