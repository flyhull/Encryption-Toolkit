using Common_Support;
using NSec.Cryptography;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Time_Based_Encryption
{
    internal class AegisSecrets : IDisposable
    {

        internal byte[] nonce = Array.Empty<byte>();
        internal byte[] authTag = Array.Empty<byte>();
        internal Key safeKey;

        private bool disposedValue;

        internal AegisSecrets(Key safeKeyIn, byte[] nonceIn, byte[] authTagIn)
        {
            // TODO PIN THESE
            nonce = nonceIn;
            authTag = authTagIn;
            safeKey = safeKeyIn;
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
            // TODO Pin these (and rewrite)
            CommonSupport.RedactByteArray(ref nonce);
            CommonSupport.RedactByteArray(ref authTag);
            safeKey.Dispose();
        }

    }

    internal class DeriveAegisSecrets : IDisposable
    {
        // normal properties
        public byte[] key = Array.Empty<byte>();
        public byte[] nonce = Array.Empty<byte>();
        public byte[] authTag = Array.Empty<byte>();

        //used by Constructor
        Int32 derivedSize = 0;
        byte[] salt = Array.Empty<byte>();
        Argon2Parameters argon2Parameters = new Argon2Parameters();
        CultureInfo defaultCulture = Thread.CurrentThread.CurrentCulture;
        string hashInput = string.Empty;
        Argon2id argon2Id;
        byte[] keyAndIvAndTag = Array.Empty<byte>();
        DateTime dateTimeToHash = DateTime.UtcNow;

        // used to dispose
        private bool disposedValue;

        public bool worked
        {
            get { return key.Length * nonce.Length * authTag.Length > 0; }
        }
        internal DeriveAegisSecrets(byte[] passPhrase, DateTime encryptionDateTime, DateTime secretDateTime, Int32 keySize, Int32 nonceSize, Int32 tagSize, Int32 memorySize, Int32 numberOfPasses)
        {
            derivedSize = keySize + nonceSize + tagSize;

            //switch culture to invariant for date formatting

            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            //Console.WriteLine("Passphrase is '" + BitConverter.ToString(passPhrase) + "'");
            //Console.WriteLine("Secret Datetime is '" + secretDateTime.ToLongTimeString() + " on " + secretDateTime.ToLongDateString() + "'");
            //Console.WriteLine("Encryption Datetime is '" + encryptionDateTime.ToLongTimeString() + " on " + encryptionDateTime.ToLongDateString() + "'");

            dateTimeToHash = encryptionDateTime.AddTicks(-2 * (encryptionDateTime.Ticks - secretDateTime.Ticks));

            hashInput = dateTimeToHash.ToLongTimeString() + " on " + dateTimeToHash.ToLongDateString();

            //switch culture back

            Thread.CurrentThread.CurrentCulture = defaultCulture;

            //Console.WriteLine("Salt used to generate the key and nonce from the passphrase is the hash of '" + hashInput + "'");

            using (MD5 hasher = MD5.Create())
            {
                salt = hasher.ComputeHash(Encoding.UTF8.GetBytes(hashInput));
            }

            argon2Parameters.DegreeOfParallelism = 1;
            argon2Parameters.MemorySize = memorySize;
            argon2Parameters.NumberOfPasses = numberOfPasses;

            argon2Id = new Argon2id(argon2Parameters);

            // TODO PIN THIS

            keyAndIvAndTag = argon2Id.DeriveBytes(passPhrase, salt, derivedSize);

            //Console.WriteLine("DERIVED KEY, NONCE AND TAG IS " + BitConverter.ToString(keyAndIvAndTag));

            // TODO PIN THESE

            key = new byte[keySize];
            nonce = new byte[nonceSize];
            authTag = new byte[tagSize];

            Buffer.BlockCopy(keyAndIvAndTag, 0, key, 0, keySize);
            Buffer.BlockCopy(keyAndIvAndTag, keySize, nonce, 0, nonceSize);
            Buffer.BlockCopy(keyAndIvAndTag, keySize + nonceSize, authTag, 0, tagSize);

            //Console.WriteLine("DERIVED KEY IS " + BitConverter.ToString(key));
            //Console.WriteLine("DERIVED NONCE IS " + BitConverter.ToString(nonce));
            //Console.WriteLine("DERIVED TAG IS " + BitConverter.ToString(authTag));

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

            // normal properties
            //TODO Pin and rewrite
            CommonSupport.RedactByteArray(ref key);
            CommonSupport.RedactByteArray(ref nonce);
            CommonSupport.RedactByteArray(ref authTag);

            ////used by Constructor
            derivedSize = 0;
            //TODO Pin and rewrite
            CommonSupport.RedactByteArray(ref salt);
            //TODO Fix
            Argon2Parameters argon2Parameters = new Argon2Parameters();
            argon2Parameters.DegreeOfParallelism = 1;
            argon2Parameters.MemorySize = TimeBasedCryptionLimits.MaximumArgon2MemorySize;
            argon2Parameters.NumberOfPasses = TimeBasedCryptionLimits.MaximumArgon2NumberOfPasses;
            //TODO Pin and rewrite
            defaultCulture = Thread.CurrentThread.CurrentCulture;
            //TODO Pin and rewrite
            string hashInput = string.Empty;
            //TODO Pin and rewrite
            argon2Id = new Argon2id(argon2Parameters);
            //TODO Pin and rewrite
            CommonSupport.RedactByteArray(ref keyAndIvAndTag);
            //TODO Pin and fix
            dateTimeToHash = DateTime.UtcNow;
        }

    }

}
