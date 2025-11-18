using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Common_Support
{ 
    public class TempByteArray : IDisposable
    {
        private byte[] pinnedBytes = Array.Empty<byte>();

        private bool disposedValue;
        public byte[] bytes
        {
            get { return pinnedBytes; }
        }

       // https://learn.microsoft.com/en-us/dotnet/api/system.gc.allocatearray?view=net-9.0&WT.mc_id=email

        public TempByteArray(byte[] input)
        {
            if (input.Length < Int32.MaxValue)
            {
                pinnedBytes = GC.AllocateArray<byte>(input.Length, true);
                Buffer.BlockCopy(input, 0, pinnedBytes, 0, input.Length);
                CommonSupport.RedactByteArray(ref input);
            }
        }

        public TempByteArray(string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open))
            {
                if (fs.Length < Int32.MaxValue)
                {
                    pinnedBytes = GC.AllocateArray<byte>((int)(fs.Length % Int32.MaxValue), true);
                    fs.Read(pinnedBytes);
                }
            }
        }

        public TempByteArray(int len)
        {
            Random rand = new Random((Int32)(DateTime.UtcNow.Ticks % Int32.MaxValue));
            pinnedBytes = pinnedBytes = GC.AllocateArray<byte>(len, true);
            rand.NextBytes(pinnedBytes);
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

        public void Redact(bool truncate = true )
        {
            CommonSupport.RedactByteArray(ref pinnedBytes, truncate);
        }
    }

    public class TempBytesThatHoldsDateTime : IDisposable
    {
        private TempByteArray stored;

        private bool disposedValue;

        public long Seconds 
        {
            get { return BitConverter.ToInt64(stored.bytes); }
        } 

        public DateTime Timestamp
        {
            get { return new DateTime(BitConverter.ToInt64(stored.bytes) * TimeSpan.TicksPerSecond, DateTimeKind.Utc); }
        }
        
        public TempBytesThatHoldsDateTime(DateTime input)
        {
            stored = new TempByteArray(BitConverter.GetBytes((long)Math.Truncate((decimal)(input.Ticks / TimeSpan.TicksPerSecond))));
            input = DateTime.UtcNow;
        }

        public TempBytesThatHoldsDateTime()
        {
            DateTime dt = new DateTime(0, DateTimeKind.Utc);
            stored = new TempByteArray(BitConverter.GetBytes((long)Math.Truncate((decimal)(dt.Ticks / TimeSpan.TicksPerSecond))));
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

        public void Redact(bool truncate = true)
        {
            stored.Redact(truncate);
        }
    }
}
