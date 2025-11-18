// Ignore Spelling: Stringify Stringified

using Common_Support;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Password_Editing_Tool
{
    public enum Topic
    {
        Grantor,
        Purpose,
        URL,
        Account,
        Identity,
        Password,
        RecoveryEmail,
        RecoveryPhone,
        Authenticator,
        Other
    }
    public class PasswordData
    {
        public bool NoSave = true;
        private Dictionary<string, string> data = new Dictionary<string, string>();
        public string Stringified
        {
            get { return presentData(); }
        }
        public PasswordData()
        {
            foreach (Topic item in Enum.GetValues<Topic>())
            {                
                data.Add(item.ToString(), "Default");                
            }
        }
        public PasswordData(string input)
        {
            foreach (string row in input.Split(Environment.NewLine))
            {
                string[] tmp = row.Split(':');
                if (row.Length > (2 + tmp[0].Length))
                {
                    data.Add(tmp[0], row.Substring(2 + tmp[0].Length));
                }
                else
                {
                    data.Add(tmp[0], "");
                }
            }
        }

        public void setValue(Topic selection, string value)
        {
            if (data.ContainsKey(selection.ToString()))
            {
                data[selection.ToString()] = value;
            }
            else
            {
                data.Add(selection.ToString(), value);
            }
            NoSave = false;
        }

        public string getValue(Topic selection)
        {
            string? result = null;
            data.TryGetValue(selection.ToString(), out result);
            return result ?? "";
        }
        private string presentData()
        {
            List<string> tmp = new List<string>();

            foreach (Topic item in Enum.GetValues<Topic>() )
            {
                if (data.ContainsKey(item.ToString()))
                {
                    tmp.Add(string.Concat(item.ToString(), ": ", data[item.ToString()] ?? ""));
                }
            }

            return string.Join(Environment.NewLine, tmp.ToArray());
        }
        
    }
}
