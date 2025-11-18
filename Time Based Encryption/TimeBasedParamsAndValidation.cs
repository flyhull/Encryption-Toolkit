using Common_Support;

//TooLarge,
//TooSmall,
//TooLong,
//TooShort,
//TooLongAgo,
//TooFarInTheFuture,
//NullOrEmpty

namespace Time_Based_Encryption
{
    public enum TimeBasedCrypterParam
    {
        Passphrase,
        EncryptionDateTime,
        SecretDateTime,
        ArgonMemorySize,
        ArgonNumberOfPasses,
        Cyphertext,
        Plaintext,
        PlusMinusSeconds,
        GoBackSeconds,
        GuaranteedLagSeconds
    }
    public class TimeBasedParamValidation
    {
        public static void Validate(TimeBasedCrypterParam param, object input, ref ValidationSummary summary) 
        {
            if (summary.StillNeeds(param.ToString()))
            {
                switch (param)
                {
                    case TimeBasedCrypterParam.PlusMinusSeconds:

                        if (input == null)
                        {
                            summary.RecordValidationResult(param.ToString(), ValidationResult.@null);
                        }
                        else
                        {
                            if (input is Int32 Value)
                            {
                                if (((Int32)Value) > TimeBasedCryptionLimits.MaximumPlusMinusSeconds)
                                {
                                    summary.RecordValidationResult(param.ToString(), ValidationResult.too_long);
                                }
                                else
                                {
                                    if (((Int32)Value) < TimeBasedCryptionLimits.MinimumPlusMinusSeconds)
                                    {
                                        summary.RecordValidationResult(param.ToString(), ValidationResult.too_short);

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

                    case TimeBasedCrypterParam.GoBackSeconds:

                        if (input == null)
                        {
                            summary.RecordValidationResult(param.ToString(), ValidationResult.@null);
                        }
                        else
                        {
                            if (input is Int32 Value)
                            {
                                if (((Int32)Value) > TimeBasedCryptionLimits.MaximumGoBackSeconds)
                                {
                                    summary.RecordValidationResult(param.ToString(), ValidationResult.too_long);
                                }
                                else
                                {
                                    if (((Int32)Value) < TimeBasedCryptionLimits.MinimumGoBackSeconds)
                                    {
                                        summary.RecordValidationResult(param.ToString(), ValidationResult.too_short);

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

                    case TimeBasedCrypterParam.GuaranteedLagSeconds:

                        if (input == null)
                        {
                            summary.RecordValidationResult(param.ToString(), ValidationResult.@null);
                        }
                        else
                        {
                            if (input is Int32 Value)
                            {
                                if (((Int32)Value) > TimeBasedCryptionLimits.MaximumGuaranteedLagSeconds)
                                {
                                    summary.RecordValidationResult(param.ToString(), ValidationResult.too_long);
                                }
                                else
                                {
                                    if (((Int32)Value) < TimeBasedCryptionLimits.MinimumGuaranteedLagSeconds)
                                    {
                                        summary.RecordValidationResult(param.ToString(), ValidationResult.too_short);

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

                    case TimeBasedCrypterParam.Passphrase:

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
                                    if (Value.Length < TimeBasedCryptionLimits.MinimumPassPhraseLength)
                                    {
                                        summary.RecordValidationResult(param.ToString(), ValidationResult.too_short);

                                    }
                                    else
                                    {
                                        if (Value.Length > TimeBasedCryptionLimits.MaximumPassPhraseLength)
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

                    case TimeBasedCrypterParam.EncryptionDateTime:

                        if (input == null)
                        {
                            summary.RecordValidationResult(param.ToString(), ValidationResult.@null);
                        }
                        else
                        {
                            if (input is DateTime Value)
                            {
                                if (((DateTime)Value) > TimeBasedCryptionLimits.LatestEncryptionDate)
                                {
                                    summary.RecordValidationResult(param.ToString(), ValidationResult.too_far_in_the_future);
                                }
                                else
                                {
                                    if (((DateTime)Value) < TimeBasedCryptionLimits.EarliestEncryptionDate)
                                    {
                                        summary.RecordValidationResult(param.ToString(), ValidationResult.too_long_ago);

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

                    case TimeBasedCrypterParam.SecretDateTime:

                        if (input == null)
                        {
                            summary.RecordValidationResult(param.ToString(), ValidationResult.@null);
                        }
                        else
                        {
                            if (input is DateTime Value)
                            {
                                if (((DateTime)Value) > TimeBasedCryptionLimits.LatestSecretDate)
                                {
                                    summary.RecordValidationResult(param.ToString(), ValidationResult.too_far_in_the_future);
                                }
                                else
                                {
                                    if (((DateTime)Value) < TimeBasedCryptionLimits.EarliestSecretDate)
                                    {
                                        summary.RecordValidationResult(param.ToString(), ValidationResult.too_long_ago);

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
                                            
                    case TimeBasedCrypterParam.ArgonMemorySize:

                        if (input == null)
                        {
                            summary.RecordValidationResult(param.ToString(), ValidationResult.@null);
                        }
                        else
                        {
                            if (input is Int32 Value)
                            {
                                if (((Int32)Value) > TimeBasedCryptionLimits.MaximumArgon2MemorySize)
                                {
                                    summary.RecordValidationResult(param.ToString(), ValidationResult.too_large);
                                }
                                else
                                {
                                    if (((Int32)Value) < TimeBasedCryptionLimits.MinimumArgon2MemorySize)
                                    {
                                        summary.RecordValidationResult(param.ToString(), ValidationResult.too_small);

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

                    case TimeBasedCrypterParam.ArgonNumberOfPasses:

                        if (input == null)
                        {
                            summary.RecordValidationResult(param.ToString(), ValidationResult.@null);
                        }
                        else
                        {
                            if (input is Int32 Value)
                            {
                                if (((Int32)Value) > TimeBasedCryptionLimits.MaximumArgon2NumberOfPasses)
                                {
                                    summary.RecordValidationResult(param.ToString(), ValidationResult.too_many);
                                }
                                else
                                {
                                    if (((Int32)Value) < TimeBasedCryptionLimits.MinimumArgon2NumberOfPasses)
                                    {
                                        summary.RecordValidationResult(param.ToString(), ValidationResult.too_few);

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

                    case TimeBasedCrypterParam.Cyphertext:

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
                                    if (Value.Length < TimeBasedCryptionLimits.MinimumCyphertextBytes)
                                    {
                                        summary.RecordValidationResult(param.ToString(), ValidationResult.too_small);
                                    }
                                    else
                                    {
                                        if (Value.Length > TimeBasedCryptionLimits.MaximumCyphertextBytes)
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

                    case TimeBasedCrypterParam.Plaintext:

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
                                    if (Value.Length < TimeBasedCryptionLimits.MinimumPlaintextBytes)
                                    {
                                        summary.RecordValidationResult(param.ToString(), ValidationResult.too_small);
                                    }
                                    else
                                    {
                                        if (Value.Length > TimeBasedCryptionLimits.MaximumPlaintextBytes)
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

                    default:
                        summary.RecordValidationResult(param.ToString(), ValidationResult.out_of_scope);
                        break;
                }

            }
        }
    }
}



    



