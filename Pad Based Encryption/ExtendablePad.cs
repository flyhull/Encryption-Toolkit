// Ignore Spelling: Extendable

using Common_Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Time_Based_Encryption;

namespace Pad_Based_Encryption
{    internal class ExtendablePad : IDisposable
    {
        // normal properties
        private UInt16 lastIndexFound = 0;
        private byte[] pad = Array.Empty<byte>();
        private TimeBasedCrypter shuffler;
        private ResultObject shuffle = new ResultObject();

        // used by FindIndexOfByteInPad
        Int32 trial = -1;

        // used to dispose
        private bool disposedValue;

        public ExtendablePad(byte[] passPhrase, DateTime encryptionDateTime, DateTime secretDateTime, Int32 memorySize, Int32 numberOfPasses, ValidationSummary validation)
        {
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
                shuffler = new TimeBasedCrypter(passPhrase, encryptionDateTime, secretDateTime, memorySize, numberOfPasses, validation);

            }
            catch (OperationCanceledException ex)
            {
                throw new OperationCanceledException("Extendable pad could not be created. " + ex.Message);
            }

        }

        public bool LoadPad(byte[] raw)
        {
            bool result = false;
            shuffle = shuffler.Encrypt(raw, new ValidationSummary());

            result = shuffle.Worked;

            if (shuffle.Worked)
            {
                //clearing in case reloading
                //pad.Initialize();
                //Array.Clear(pad);
                if (pad.Length > 0)
                {
                    Buffer.BlockCopy(shuffle.Bytes, 32, pad, 0, pad.Length);
                }
                else
                {
                    //TODO DEAL WITH PINNING THIS AND CLEARING IT
                    pad = new byte[shuffle.Bytes.Length];
                    Buffer.BlockCopy(shuffle.Bytes, 0, pad, 0, shuffle.Bytes.Length);
                }                 
            } 
            else
            {
                //Console.WriteLine(shuffle.Outcome);
            }

            //Console.WriteLine(dumpPad(10));

            return result;

        }

        public string DumpPad(Int32 length)
        {
            if (pad.Length > length)
            {
                return "Pad is " + pad.Length.ToString() + " long and starts with " + BitConverter.ToString(pad).Substring(0, -1 + 3 * length);
            }
            else
            {
                return "Pad is " + pad.Length.ToString() + " long and is " + BitConverter.ToString(pad);
            }
        }

        public ResultObject TimeBasedEncrypt(byte[] plaintext, ValidationSummary validation)
        {
            return (ResultObject)shuffler.Encrypt(plaintext, validation);
        }

        public ResultObject TimeBasedDecrypt(byte[] cyphertext, ValidationSummary validation)
        {
            return (ResultObject)shuffler.Decrypt(cyphertext, validation);
        }

        public UInt16 FindIndexOfByteInPad(byte byteToFind, ref PadBasedCryptionIssue encryptStatus)
        {
            // this is global so we do not have to allocate a new one every time
            trial = Array.IndexOf<byte>(pad, byteToFind, lastIndexFound + 1);

            if (trial < 0 || trial > Int16.MaxValue)
            {
                //not found in pad, reshuffle

                ReshufflePad();

                //Console.WriteLine("Reshuffling Pad");

                trial = Array.IndexOf<byte>(pad, byteToFind, 0);

                if (trial < 0)
                {
                    encryptStatus = PadBasedCryptionIssue.value_not_found_in_reshuffled_pad;
                    return 0;
                }

                if (trial > lastIndexFound || trial > Int16.MaxValue)
                {
                    encryptStatus = PadBasedCryptionIssue.value_not_found_soon_enough_in_reshuffled_pad;
                    return 0;
                }

            }

            lastIndexFound = (UInt16)trial;

            return lastIndexFound;

        }

        public byte RetrieveByteUsingIndex(UInt16 index, ref PadBasedCryptionIssue decryptStatus)
        {
            if (!(index >= lastIndexFound))
            {
                ReshufflePad();
            }

            if (index < pad.Length)
            {
                lastIndexFound = index;
                return pad[(Int32)index];

            }
            else
            {
                decryptStatus = PadBasedCryptionIssue.cryption_pad_not_long_enough_for_index;
                return new byte();
            }
        }

        private bool ReshufflePad()
        {
            return LoadPad(pad);

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
            //TODO pin this
            lastIndexFound = 0;
            //TODO pin this
            CommonSupport.RedactByteArray(ref pad);
            shuffler.Redact();
            shuffle.Redact();

            //TODO pin this
            // used by FindIndexOfByteInPad
            trial = -1;

        }


    }
}
