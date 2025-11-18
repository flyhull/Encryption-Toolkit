using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common_Support
{
    public enum ValidationResult
    {
        out_of_scope,
        perfect,
        too_many,
        too_few,
        too_large,
        too_small,
        too_long,
        too_short,
        too_long_ago,
        too_far_in_the_future,
        wrong_type,
        empty,
        negative,
        @null,
        not_a_png,
        not_present,
        orphaned_file,
        not_on_drive_root,
        unreadable,
        missing,
        not_a_transparent_png,
        is_invalid,
        has_wrong_extension,
        not_on_removable_drive,
        not_base64,
        missing_period,
        is_relative,
        has_path
    }
    public class ValidationSummary
    {
        private Dictionary<string, ValidationResult> ValidationResults = new Dictionary<string, ValidationResult>();

        public bool Valid
        {
            get { return ValidationResults.Count > 0 && ValidationResults.Values.All(x => x == ValidationResult.perfect); }
        }

        public bool StillNeeds(string fieldName)
        {
            return !ValidationResults.ContainsKey(fieldName);
        }

        public bool RecordValidationResult(string fieldValidated, ValidationResult result)
        {
            if (ValidationResults.TryAdd(fieldValidated, result))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<string> ListValidationIssues()
        {
            List<string> result = new List<string>();

            foreach (KeyValuePair<String, ValidationResult> item in ValidationResults)
            {
                if (!(item.Value == ValidationResult.perfect))
                {
                    result.Add(string.Concat(item.Key, " is ", item.Value.ToString().Replace('_', ' ')));
                }
            }

            return result;
        }

        public OperationCanceledException GetException()
        {
            return new OperationCanceledException("The following parameters are invalid: " + String.Join(" and ", ListValidationIssues().ToArray()));
        }
    }
}
