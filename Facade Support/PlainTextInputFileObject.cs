using Common_Support;

namespace Facade_Support
{
    public class PlainTextInputFileObject
    {
        public readonly bool SupportedFileType = false;
        public ResultObject fileContents = new ResultObject();
        public Exception? ex = null;
        public bool Valid 
        {
            get { return (ex == null && bytes.Length > 0); }
        }

        public byte[] bytes
        {
            get { 
            if (fileContents.Bytes.Length > 0)
                {
                    return fileContents.Bytes;
                }
            else
                {
                    return Array.Empty<byte>();
                }
            }
        } 
        public string error
        {
            get 
            {
                if (ex == null)
                {
                    return fileContents.Snapshot;
                }
                else
                {
                    return ex.Message;
                }

            }
        }
        public PlainTextInputFileObject(string filename, bool MyUseIsNonCommercial)
        {
            try
            {
                fileContents = FacadeSupport.GetBytesFromFile(filename, new ValidationSummary());
                if (fileContents.Worked)
                {
                    SupportedFileType = Path.GetExtension(filename).ToLower().Equals(FacadeSupport.GetFileExtensionFromBytes(ref fileContents, MyUseIsNonCommercial));
                }                
            }
            catch (Exception e)
            {
                ex = e;
            }
        }

        public PlainTextInputFileObject()
        {
        }
    }
}
