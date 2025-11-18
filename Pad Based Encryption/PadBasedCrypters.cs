using System.Diagnostics;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Time_Based_Encryption;
using Common_Support;
using System.ComponentModel.Design;
using System.Reflection;

namespace Pad_Based_Encryption
{
    public class UseTimeAndPadToStatically
    {
        public static ResultObject Encrypt(byte[] plainText, byte[] pad, byte[] passPhrase, DateTime secretDateTime, Int32 argon2MemorySize, Int32 argon2NumberOfPasses, Int32 maximumAttempts, ValidationSummary validation)
        {
            ResultObject result = new ResultObject();

            Stopwatch sw = new Stopwatch();

            sw.Start();

            Int32 tries = 0;

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
                PadBasedParamValidation.Validate(PadBasedCrypterParam.Plaintext, plainText, ref validation);
                PadBasedParamValidation.Validate(PadBasedCrypterParam.CryptionPadBytes, pad, ref validation);
                TimeBasedParamValidation.Validate(TimeBasedCrypterParam.Passphrase, passPhrase, ref validation);
                TimeBasedParamValidation.Validate(TimeBasedCrypterParam.SecretDateTime, secretDateTime, ref validation);
                TimeBasedParamValidation.Validate(TimeBasedCrypterParam.ArgonMemorySize, argon2MemorySize, ref validation);
                TimeBasedParamValidation.Validate(TimeBasedCrypterParam.ArgonNumberOfPasses, argon2NumberOfPasses, ref validation);
                PadBasedParamValidation.Validate(PadBasedCrypterParam.EncryptionAttempts, maximumAttempts, ref validation);

                if (validation.Valid)
                {
                    //Console.WriteLine("ENCRYPTING");

                    while (tries < maximumAttempts)
                    {
                        tries++;

                        using (PadBasedCrypter worker = new PadBasedCrypter(passPhrase, DateTime.UtcNow, secretDateTime, argon2MemorySize, argon2NumberOfPasses, validation))
                        {
                            result = worker.Encrypt(plainText, pad, validation);
                        }

                        if (result.Worked)
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(1000);
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

            Console.WriteLine();
            Console.WriteLine("All " + tries.ToString() + " encryption attempts collectively took " + sw.ElapsedMilliseconds.ToString() + " milliseconds");
            
            return result;

        }

        public static ResultObject Decrypt(byte[] cypherText, byte[] pad, byte[] passPhrase, DateTime secretDateTime, DateTime encryptionDateTime, Int32 PlusMinusSeconds, Int32 GoBackSeconds, Int32 GuaranteedLagSeconds, Int32 argon2MemorySize, Int32 argon2NumberOfPasses, ValidationSummary validation)
        {
            ResultObject result = new ResultObject();

            Stopwatch sw = new Stopwatch();

            sw.Start();

            Int32 attempts = 0;
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
                

                PadBasedParamValidation.Validate(PadBasedCrypterParam.Cyphertext, cypherText, ref validation);
                PadBasedParamValidation.Validate(PadBasedCrypterParam.CryptionPadBytes, pad, ref validation);
                TimeBasedParamValidation.Validate(TimeBasedCrypterParam.Passphrase, passPhrase, ref validation);
                TimeBasedParamValidation.Validate(TimeBasedCrypterParam.SecretDateTime, secretDateTime, ref validation);
                TimeBasedParamValidation.Validate(TimeBasedCrypterParam.EncryptionDateTime, encryptionDateTime, ref validation);
                TimeBasedParamValidation.Validate(TimeBasedCrypterParam.ArgonMemorySize, argon2MemorySize, ref validation);
                TimeBasedParamValidation.Validate(TimeBasedCrypterParam.ArgonNumberOfPasses, argon2NumberOfPasses, ref validation);
                TimeBasedParamValidation.Validate(TimeBasedCrypterParam.PlusMinusSeconds, PlusMinusSeconds, ref validation);
                TimeBasedParamValidation.Validate(TimeBasedCrypterParam.GoBackSeconds, GoBackSeconds, ref validation);
                TimeBasedParamValidation.Validate(TimeBasedCrypterParam.GuaranteedLagSeconds, GuaranteedLagSeconds, ref validation);

                if (validation.Valid)
                {
                    //Console.WriteLine("DECRYPTING");

                    for (int i = -1 * PlusMinusSeconds + GuaranteedLagSeconds; i <= PlusMinusSeconds + GoBackSeconds + GuaranteedLagSeconds; i++)
                    {
                        using (PadBasedCrypter worker = new PadBasedCrypter(passPhrase, encryptionDateTime.AddSeconds(-1 * i), secretDateTime, argon2MemorySize, argon2NumberOfPasses, validation))
                        {
                            result = new ResultObject(worker.Decrypt(cypherText, pad, validation).Bytes, encryptionDateTime.AddSeconds(-1 * i));
                        }

                        attempts++;

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

            Console.WriteLine();
            Console.WriteLine("All " + attempts.ToString() + " decryption attempts collectively took " + sw.ElapsedMilliseconds.ToString() + " milliseconds");

            return result;

        }

    }
    internal class PadBasedCrypter : IDisposable
    {
        // normal properties
       
        private readonly ExtendablePad extendablePad;     
               

        // used to dispose
        private bool disposedValue;

        internal PadBasedCrypter(byte[] passPhrase, DateTime encryptionDateTime, DateTime secretDateTime, Int32 memorySize, Int32 numberOfPasses, ValidationSummary validation)
        {
                TimeBasedParamValidation.Validate(TimeBasedCrypterParam.Passphrase, passPhrase, ref validation);
                TimeBasedParamValidation.Validate(TimeBasedCrypterParam.EncryptionDateTime, encryptionDateTime, ref validation);
                TimeBasedParamValidation.Validate(TimeBasedCrypterParam.SecretDateTime, secretDateTime, ref validation);
                TimeBasedParamValidation.Validate(TimeBasedCrypterParam.ArgonMemorySize, memorySize, ref validation);
                TimeBasedParamValidation.Validate(TimeBasedCrypterParam.ArgonNumberOfPasses, numberOfPasses, ref validation);

                if (validation.Valid)
                {
                    extendablePad = new ExtendablePad(passPhrase, encryptionDateTime, secretDateTime, memorySize, numberOfPasses, validation);
                }
                else
                {
                    throw validation.GetException();
                }

            
            //Console.WriteLine("**** Pad-based crypter created with encryption timestamp of " + encryptionDateTime.ToString("u"));
            //Console.WriteLine("******** Pad-based crypter created with secret timestamp of " + secretDateTime.ToString("u"));
        }

        internal ResultObject Encrypt(byte[] plainText, byte[] pad, ValidationSummary validation)
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

            PadBasedParamValidation.Validate(PadBasedCrypterParam.Plaintext, plainText, ref validation);
            PadBasedParamValidation.Validate(PadBasedCrypterParam.CryptionPadBytes, pad, ref validation);

            if (validation.Valid)
            {
                //Console.WriteLine("Plaintext is " + plainText.Length.ToString() + " long and starts with " + BitConverter.ToString(plainText.Take<byte>(10).ToArray<byte>()) + " and ends with " + BitConverter.ToString(plainText.TakeLast<byte>(10).ToArray<byte>()));

                //randomize the plaintext

                result = extendablePad.TimeBasedEncrypt(plainText, validation);

                if (result.Worked)
                {
                    //Console.WriteLine("Bytes to get offsets are " + result.bytes.Length.ToString() + " long and starts with " + BitConverter.ToString(result.bytes.Take<byte>(10).ToArray<byte>()) + " and ends with " + BitConverter.ToString(result.bytes.TakeLast<byte>(10).ToArray<byte>()));

                    //now lets build the list of offsets for each byte

                    //allocate byte array to hold two times 

                    //Console.WriteLine("Offset bytes should be " + (2 * result.bytes.Length).ToString() + " long");

                    //TODO PIN THIS

                    Byte[] offsets = new Byte[2 * result.Bytes.Length];

                    MemoryStream ms = new MemoryStream(offsets);

                    if (extendablePad.LoadPad(pad))
                    {
                        PadBasedCryptionIssue status = PadBasedCryptionIssue.None;

                        foreach (byte item in result.Bytes)
                        {
                            ms.Write(BitConverter.GetBytes(extendablePad.FindIndexOfByteInPad(item, ref status)));

                            if (status != PadBasedCryptionIssue.None)
                            {
                                result.RecordPadBasedCryptionIssue(status, activity);
                                break;
                            }

                        }

                        //Console.WriteLine("done encrypting");
                    }
                    else
                    {

                        result.RecordPadBasedCryptionIssue(PadBasedCryptionIssue.could_not_load_pad_for_encryption, activity);

                    }


                    //offsetsString = String.Join(',', offsets.ToArray());

                    //Console.WriteLine(offsetsString.Split(',').Count().ToString() + " Encrypted offsets are " + offsetsString);

                    if (result.Worked)
                    {
                        //Console.WriteLine("Offsets as bytes are " + ms.ToArray().Length.ToString() + " bytes long and start with " + BitConverter.ToString(ms.ToArray().Take<byte>(10).ToArray<byte>()) + " and ends with " + BitConverter.ToString(ms.ToArray().TakeLast<byte>(10).ToArray<byte>()));

                        //obfuscate the list of numeric offsets 

                        result = extendablePad.TimeBasedEncrypt(ms.ToArray(), validation);

                        if (result.Failed)
                        {
                            result.RecordPadBasedCryptionIssue(PadBasedCryptionIssue.could_not_obsfucate_offsets, activity);
                        }

                        //Console.WriteLine("Cyphertext is " + result.bytes.Length.ToString() + " long and starts with " + BitConverter.ToString(result.bytes.Take<byte>(10).ToArray<byte>()) + " and ends with " + BitConverter.ToString(result.bytes.TakeLast<byte>(10).ToArray<byte>()));

                    }
                }
                else
                {
                    result.RecordPadBasedCryptionIssue(PadBasedCryptionIssue.could_not_obsfucate_plaintext, activity);
                }

            }
            else
            {
                result = new ResultObject(validation);
            }

            sw.Stop();

            Console.Write("+");
            //Console.WriteLine("Encryption attempt took " + sw.ElapsedMilliseconds.ToString() + " milliseconds");

            return result;
        }

        internal ResultObject Decrypt(byte[] cypherText, byte[] pad, ValidationSummary validation)
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

            PadBasedParamValidation.Validate(PadBasedCrypterParam.Cyphertext, cypherText, ref validation);
            PadBasedParamValidation.Validate(PadBasedCrypterParam.CryptionPadBytes, pad, ref validation);

            if (validation.Valid)
            {
                //expose the obfuscated list of numeric offsets 
                result = extendablePad.TimeBasedDecrypt(cypherText, validation);

                if (result.Worked)
                {
                    //Console.WriteLine("Cyphertext is " + cypherText.Length.ToString() + " long and starts with " + BitConverter.ToString(cypherText.Take<byte>(10).ToArray<byte>()) + " and ends with " + BitConverter.ToString(cypherText.TakeLast<byte>(10).ToArray<byte>()));

                    if ((result.Bytes.Length % 2) < 1)
                    {
                        //Console.WriteLine("Offsets as bytes are " + result.bytes.Length.ToString() + " long and start with " + BitConverter.ToString(result.bytes.Take<byte>(10).ToArray<byte>()) + " and ends with " + BitConverter.ToString(result.bytes.TakeLast<byte>(10).ToArray<byte>()));

                        //now that we know the encryption time we start using the date time offset as well

                        //Console.WriteLine("Confirmed to be offsets");

                        //Console.WriteLine(offsetsString.Split(',').Count().ToString() + " Decrypted offsets are " + offsetsString);

                        //Console.WriteLine("Hash of " + offsetsString.Split(',').Count().ToString() + " offsets is " + BitConverter.ToString(MD5.HashData(result.value)));

                        if (extendablePad.LoadPad(pad))
                        {
                            Int32 i = 0;

                            //TODO PIN THESE

                            byte[] twoTempBytes = new byte[2];

                            byte[] recoveredResults = new byte[result.Bytes.Length / 2];

                            PadBasedCryptionIssue status = PadBasedCryptionIssue.None;

                            while (i < recoveredResults.Length)
                            {
                                Buffer.BlockCopy(result.Bytes, 2 * i, twoTempBytes, 0, 2);
                                recoveredResults[i] = extendablePad.RetrieveByteUsingIndex(BitConverter.ToUInt16(twoTempBytes), ref status);

                                if (status != PadBasedCryptionIssue.None)
                                {
                                    result.RecordPadBasedCryptionIssue(status, activity);
                                    break;
                                }

                                i++;
                            }

                            if (result.Worked)
                            {
                                //Console.WriteLine("Bytes which had offsets are " + recoveredResults.Length.ToString() + " long and start with " + BitConverter.ToString(recoveredResults.Take<byte>(10).ToArray<byte>()) + " and ends with " + BitConverter.ToString(recoveredResults.TakeLast<byte>(10).ToArray<byte>()));

                                result = extendablePad.TimeBasedDecrypt(recoveredResults,new ValidationSummary());

                                //Console.WriteLine("Plaintext bytes are " + result.bytes.Length.ToString() + " long and start with " + BitConverter.ToString(result.bytes.Take<byte>(10).ToArray<byte>()) + " and ends with " + BitConverter.ToString(result.bytes.TakeLast<byte>(10).ToArray<byte>()));

                            }
                        }
                        else
                        {
                            result.RecordPadBasedCryptionIssue(PadBasedCryptionIssue.could_not_load_pad_for_decryption, activity);
                        }

                    }
                    else
                    {
                        result.RecordPadBasedCryptionIssue(PadBasedCryptionIssue.data_Is_not_encrypted_offsets, activity);
                    }

                }
                else
                {
                    result.RecordPadBasedCryptionIssue(PadBasedCryptionIssue.wrong_time_based_key, activity);
                }
            }
            else
            {
                result = new ResultObject(validation);
            }

            sw.Stop();

            Console.Write("+");
            //Console.WriteLine("Decryption attempt took " + sw.ElapsedMilliseconds.ToString() + " milliseconds");

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

        internal void Redact()
        {
            extendablePad.Redact();
        }

    }
}
