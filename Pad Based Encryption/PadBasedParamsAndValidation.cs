using Common_Support;
using Time_Based_Encryption;

namespace Pad_Based_Encryption
{

    public enum PadBasedCrypterParam
    {
        CryptionPadBytes,
        Cyphertext,
        Plaintext,
        EncryptionAttempts
    }

    public class PadBasedParamValidation
    {
        public static void Validate(PadBasedCrypterParam param, object input, ref ValidationSummary summary)
        {
            if (summary.StillNeeds(param.ToString()))
            {
                switch (param)
                {
                    case PadBasedCrypterParam.CryptionPadBytes:

                        if (input == null)
                        {
                            summary.RecordValidationResult(param.ToString(), ValidationResult.@null);
                        }
                        else
                        {
                            if (input is byte[] Value)
                            {
                                if (Value.Length > 0)
                                {
                                    if (Value.Length < PadBasedCryptionLimits.MinimumCryptionPadBytes)
                                    {
                                        summary.RecordValidationResult(param.ToString(), ValidationResult.too_short);

                                    }
                                    else
                                    {
                                        if (Value.Length > PadBasedCryptionLimits.MaximumCryptionPadBytes)
                                        {
                                            summary.RecordValidationResult(param.ToString(), ValidationResult.too_long);
                                        }
                                        else
                                        {
                                            summary.RecordValidationResult(param.ToString(), ValidationResult.perfect);                                            
                                        }
                                    }
                                }
                                else
                                {
                                    summary.RecordValidationResult(param.ToString(), ValidationResult.empty);
                                }
                            }
                            else
                            {
                                summary.RecordValidationResult(param.ToString(), ValidationResult.wrong_type);
                            }
                        }
                        break;

                    case PadBasedCrypterParam.Cyphertext:

                        if (input == null)
                        {
                            summary.RecordValidationResult(param.ToString(), ValidationResult.@null);
                        }
                        else
                        {
                            if (input is byte[] Value)
                            {
                                if (Value.Length > 0)
                                {
                                    if (Value.Length < PadBasedCryptionLimits.MinimumCyphertextBytes)
                                    {
                                        summary.RecordValidationResult(param.ToString(), ValidationResult.too_small);
                                    }
                                    else
                                    {                                        
                                        if (Value.Length > PadBasedCryptionLimits.MaximumCyphertextBytes)
                                        {
                                            summary.RecordValidationResult(param.ToString(), ValidationResult.too_large);
                                        }
                                        else
                                        {
                                            summary.RecordValidationResult(param.ToString(), ValidationResult.perfect);
                                        }
                                    }                                    
                                }
                                else
                                {
                                    summary.RecordValidationResult(param.ToString(), ValidationResult.empty);
                                }
                            }
                            else
                            {
                                summary.RecordValidationResult(param.ToString(), ValidationResult.wrong_type);
                            }
                        }
                        break;

                    case PadBasedCrypterParam.Plaintext:

                        if (input == null)
                        {
                            summary.RecordValidationResult(param.ToString(), ValidationResult.@null);
                        }
                        else
                        {
                            if (input is byte[] Value)
                            {
                                if (Value.Length > 0)
                                {
                                    if (Value.Length < PadBasedCryptionLimits.MinimumPlaintextBytes)
                                    {
                                        summary.RecordValidationResult(param.ToString(), ValidationResult.too_small);
                                    }
                                    else
                                    {
                                        if (Value.Length > PadBasedCryptionLimits.MaximumPlaintextBytes)
                                        {
                                            summary.RecordValidationResult(param.ToString(), ValidationResult.too_large);
                                        }
                                        else
                                        {
                                            summary.RecordValidationResult(param.ToString(), ValidationResult.perfect);
                                        }
                                    }
                                }
                                else
                                {
                                    summary.RecordValidationResult(param.ToString(), ValidationResult.empty);
                                }
                            }
                            else
                            {
                                summary.RecordValidationResult(param.ToString(), ValidationResult.wrong_type);
                            }
                        }
                        break;

                    case PadBasedCrypterParam.EncryptionAttempts:

                        if (input == null)
                        {
                            summary.RecordValidationResult(param.ToString(), ValidationResult.@null);
                        }
                        else
                        {
                            if (input is Int32 Value)
                            {
                                if (Value < PadBasedCryptionLimits.MinimumEncryptionAttempts)
                                {
                                    summary.RecordValidationResult(param.ToString(), ValidationResult.too_few);
                                }
                                else
                                {
                                    if (Value > PadBasedCryptionLimits.MaximumEncryptionAttempts)
                                    {
                                        summary.RecordValidationResult(param.ToString(), ValidationResult.too_many);
                                    }
                                    else
                                    {
                                        summary.RecordValidationResult(param.ToString(), ValidationResult.perfect);
                                    }
                                }
                            }
                            else
                            {
                                summary.RecordValidationResult(param.ToString(), ValidationResult.wrong_type);
                            }
                        }
                        break;

                    default:
                        summary.RecordValidationResult(param.ToString(), ValidationResult.out_of_scope);
                        break;

                }
            }
        }        
    }
}







