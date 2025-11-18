using Common_Support;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Common_Support
{
    public enum TimeBasedCryptionIssue
    {
        None,
        failed_validation,
        encountered_exception,
        Redacted
    }

    public enum PadBasedCryptionIssue
    {
        None,
        data_Is_not_encrypted_offsets,
        could_not_load_pad_for_decryption,
        could_not_load_pad_for_encryption,
        could_not_obsfucate_plaintext,
        could_not_obsfucate_offsets,
        cryption_pad_not_long_enough_for_index,
        value_not_found_in_reshuffled_pad,
        value_not_found_soon_enough_in_reshuffled_pad,
        wrong_time_based_key,
        failed_validation,
        encountered_exception,
        Redacted
    }
    public enum ImageIssue
    {
        None,
        failed_validation,
        encountered_exception,
        could_not_retrieve_bytes_from_png,
        could_not_create_interlaced_image_from_cyphertext,
        could_not_create_padding_for_interlaced_image,
        could_not_recover_cyphertext_from_interlaced_image,
        could_not_recover_cyphertext_from_created_image,
        failed_to_create_new_image,
        could_not_create_padding_for_created_image,
        could_not_write_output,
        Redacted
    }
    public enum FacadeIssue
    {
        None,
        failed_validation,
        encountered_exception,
        cannot_parse_utc_timestamp,
        cannot_determine_file_extension,
        cannot_determine_mime_type,
        Redacted
    }

    public enum TransportIssue
    {
        None,
        file_missing,
        filename_missing,
        input_missing,
        temp_directory_invalid,
        file_not_in_temp_directory,
        operation_failed,
        Redacted,
        file_could_not_be_read,
        file_could_not_be_written,
        could_not_convert_bytes_to_base64,
        input_invalid,
        not_receipt_message,
        outbound_directory_invalid,
        received_directory_invalid,
        not_cached
    }

    public class ResultObject : IDisposable
    {
        private long _FileLength = 0;
        public long FileLength
        {
            get { return _FileLength; }
        }
        private byte[] pinnedBytes = Array.Empty<byte>();
        public byte[] Bytes
        {
            get { return pinnedBytes; }
        }
        private string _fileName = string.Empty;
        public string FileName
        {
            get { return _fileName; }
        }
        private string _Base64String = string.Empty;
        public string Base64String
        {
            get { return _Base64String; }
        }
        private DateTime? _EncryptionTime;
        public DateTime? EncryptionTime
        {
            get { return _EncryptionTime; }
        }
        private Exception? _ex = null;
        public Exception? Ex
        {
            get { return _ex; }
        }
        private List<string> _ValidationIssues = new List<string>();
        public List<string> ValidationIssues
                {
            get { return _ValidationIssues; }
        }        
        private FacadeIssue _FacadeBasedIssue = FacadeIssue.None;
        public FacadeIssue FacadeBasedIssue
                {
            get { return _FacadeBasedIssue; }
        }
        private TransportIssue _TransportBasedIssue = TransportIssue.None;
        public TransportIssue TransportBasedIssue
        {
            get { return _TransportBasedIssue; }
        }
        private ImageIssue _ImageBasedIssue = ImageIssue.None;
        public ImageIssue ImageBasedIssue
                {
            get { return _ImageBasedIssue; }
        }
        private PadBasedCryptionIssue _PadBasedIssue = PadBasedCryptionIssue.None;
        public PadBasedCryptionIssue PadBasedIssue
        {
            get { return _PadBasedIssue; }
        }
        private TimeBasedCryptionIssue _TimeBasedIssue = TimeBasedCryptionIssue.None;
        public TimeBasedCryptionIssue TimeBasedIssue
        {
            get { return _TimeBasedIssue; }
        }
        private string _activity = "";
        public string Activity
        {
            get { return _activity; }
        }

        private bool disposedValue;

        public string Snapshot
        {
            get { return string.Join(Environment.NewLine, Spill()); }
        }
        public bool Failed
        {
            get { return !Worked; }
        }
        public bool Worked
        {
            get
            {
                return (WroteBytes || WroteFile || WroteString)
                    && _ex == null
                    && _ValidationIssues.Count < 1
                    && _FacadeBasedIssue == FacadeIssue.None
                    && _TransportBasedIssue == TransportIssue.None
                    && _ImageBasedIssue == ImageIssue.None
                    && _PadBasedIssue == PadBasedCryptionIssue.None
                    && _TimeBasedIssue == TimeBasedCryptionIssue.None;
            }
        }
        public bool WroteFile
        {
            get { return _fileName.Length > 0; }
        }
        public bool WroteBytes
        {
            get { return Bytes.Length > 0; }
        }
        public bool WroteString
        {
            get { return _Base64String.Length > 0; }
        }
        public bool HasDate
        {
            get { return (_EncryptionTime.HasValue); }
        }
        public byte[] HashOfBytes
        {
            get { if (pinnedBytes.Length > 0) 
                    {
                        return MD5.HashData(pinnedBytes);
                    }
                    else 
                    {
                        return Array.Empty<byte>();
                    }
                }
        }

        public string DescribeBytes()
        {
            if (pinnedBytes.Length > 0)
            {
                Int32 endSize = int.Min(18, pinnedBytes.Length);
                StringBuilder sb = new StringBuilder();
                sb.Append("Message is " + pinnedBytes.Length.ToString() + " bytes long");
                sb.Append(" and starts with " + BitConverter.ToString(Bytes.Take<byte>(endSize).ToArray<byte>()));
                sb.Append(" and ends with " + BitConverter.ToString(Bytes.TakeLast<byte>(endSize).ToArray<byte>()));
                sb.Append(" and has a hash of " + BitConverter.ToString(HashOfBytes));
                return sb.ToString();
            }
            else
            {
                return "no data";
            }
        }

        
        
        public ResultObject()
        {

        }

        public ResultObject(Exception problem, string during)
        {
            _activity = during;
            _ex = problem;
        }

        public ResultObject(byte[]? input, DateTime? timestamp = null)
        {
            if (!(input == null))
            {

                if (input.Length < Int32.MaxValue)
                {
                    pinnedBytes = GC.AllocateArray<byte>(input.Length, true);
                    Buffer.BlockCopy(input, 0, pinnedBytes, 0, input.Length);
                }
                    else
                {
                    pinnedBytes = new byte[input.Length];
                    Buffer.BlockCopy(input, 0, pinnedBytes, 0, input.Length);
                }

                _EncryptionTime = timestamp;
            }

        }

        public ResultObject(FileInfo input, bool wait, DateTime? timestamp = null)
        {
            if (wait)
            {
                Task.Delay(1000);
            }

            input.Refresh();

            if (input.Exists)
            {
                _fileName = input.FullName;

                if (timestamp.HasValue)
                {
                    _EncryptionTime = timestamp;
                }
                else
                {
                    _EncryptionTime = input.CreationTimeUtc;
                }

                _FileLength = input.Length;        
            }
        }

        public ResultObject(string Base64)
        {
            _Base64String = Base64;
        }

        public ResultObject(ValidationSummary summary)
        {
            _ValidationIssues = summary.ListValidationIssues();
        }

        public void RecordPadBasedCryptionIssue(PadBasedCryptionIssue input, string during)
        {
            _activity = during;
            _PadBasedIssue = input;
        }

        public void RecordTimeBasedCryptionIssue(TimeBasedCryptionIssue input, string during)
        {
            _activity = during;
            _TimeBasedIssue = input;
        }

        public void RecordImageIssue(ImageIssue input, string during)
        {
            _activity = during;
            _ImageBasedIssue = input;
        }

        public void RecordFacadeIssue(FacadeIssue input, string during)
        {
            _activity = during;
            _FacadeBasedIssue = input;
        }

        public void RecordTransportIssue (TransportIssue input, string during)
        {
            _activity = during;
            _TransportBasedIssue = input;
        }


        public string SizeDesc() 
        {
            string result = "empty";

            if (Worked)
            {
                if (WroteBytes)
                {
                    result = Bytes.Length.ToString() + " bytes";
                }

                if (WroteFile)
                {
                    result = _FileLength.ToString() + " bytes";
                }

                if (WroteString)
                {
                    result = _Base64String.Length.ToString() + " characters";
                }
            }

            return result;
        }

        public List<string> Spill()
        {
            List<string> litany = new List<string>();

            if (Worked)
            {
                litany.Add("Succeeded");

                if (WroteBytes)
                {
                    litany.Add("Wrote " + Bytes.Length.ToString() + " bytes");
                }

                if (WroteFile)
                {
                    litany.Add("Wrote " + _fileName);
                }

                if (WroteString)
                {
                    litany.Add("Wrote " + _Base64String.Length.ToString() + " characters");
                }
            }
            else
            {
                litany.Add("Failed");

                if (!(_ex == null))
                {
                    litany.Add("There was an Exception:");
                    litany.Add(_ex.ToString());
                }

                if (_ValidationIssues.Count > 0)
                {
                    litany.Add("There were Validation Issues:");
                    foreach (string issue in _ValidationIssues)
                    {
                        litany.Add("> " + issue);
                    }
                }

                if (_TimeBasedIssue != TimeBasedCryptionIssue.None)
                {
                    litany.Add("Time Based Issue: " + _TimeBasedIssue.ToString());
                }

                if (_PadBasedIssue != PadBasedCryptionIssue.None)
                {
                    litany.Add("Pad Based Issue: " + _PadBasedIssue.ToString());
                }

                if (_ImageBasedIssue != ImageIssue.None)
                {
                    litany.Add("Image Based Issue: " + _ImageBasedIssue.ToString());
                }

                if (_FacadeBasedIssue != FacadeIssue.None)
                {
                    litany.Add("Facade Based Issue: " + _FacadeBasedIssue.ToString());
                }

                if (_TransportBasedIssue != TransportIssue.None)
                {
                    litany.Add("Transport Based Issue: " + _TransportBasedIssue.ToString());
                }

                if (_activity.Length > 0)
                {
                    litany.Add("while " + _activity);
                }
            }

            return litany;
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
            CommonSupport.RedactByteArray(ref pinnedBytes);
            
            //not secret 
            _fileName = string.Empty;
            _Base64String = string.Empty;

            //TODO FIX THIS
            _EncryptionTime = null;

            //not secret
            _ex = new Exception("Redacted");

            //not secret 
            _ValidationIssues = new List<string>();
            //not secret 
            _FacadeBasedIssue = FacadeIssue.Redacted;
            _TransportBasedIssue = TransportIssue.Redacted;
            _ImageBasedIssue = ImageIssue.Redacted;
            _PadBasedIssue = PadBasedCryptionIssue.Redacted;
            _TimeBasedIssue = TimeBasedCryptionIssue.Redacted;

            //not secret 
            _activity = string.Empty;
        }

        public void RecordTransportIssue(object input_invalid, object activity)
        {
            throw new NotImplementedException();
        }
    }
}
