using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Time_Based_Encryption
{
    public class TimeStampObject
    {

        public DateTime TimeStampValue = DateTime.MinValue.AddDays(1);
        public bool Valid
        {
            get { return TimeStampValue > DateTime.MinValue.AddDays(2); }
        }

        public TimeStampObject(DateTime input)
        {
            TimeStampValue = input;
        }

        public TimeStampObject(string input)
        {
            DateTime goodValue = DateTime.MinValue;
            CultureInfo culture = CultureInfo.InvariantCulture;
            DateTimeStyles styles = DateTimeStyles.None;
            bool parsed = DateTime.TryParse(input, culture, styles, out goodValue);
            if (parsed)
            {
                TimeStampValue = goodValue;
            }
        }

    }
}
