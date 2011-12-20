using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;
using System.Text;
using EWSNotifier.Properties;

namespace EWSNotifier.Utility
{
    class Configuration
    {
        public static void SaveSettings()
        {
            Settings.Default.Save();
        }

        public static string EWSUsername
        {
            get { return Settings.Default.EWSUsername; }
            set { Settings.Default.EWSUsername = value; }
        }

        public static string EWSPassword
        {
            get
            { 
                byte[] enc = Convert.FromBase64String(Settings.Default.EWSPassword);
                if (enc.Length == 0)
                    return string.Empty;
                byte[] entropy = UnicodeEncoding.UTF8.GetBytes("whatever");
                byte[] dec = ProtectedData.Unprotect(enc, entropy, DataProtectionScope.CurrentUser);
                return UnicodeEncoding.UTF8.GetString(dec);        
            }
            set 
            { 
                byte[] dec = UnicodeEncoding.UTF8.GetBytes(value);
                byte[] entropy = UnicodeEncoding.UTF8.GetBytes("whatever");
                byte[] enc = ProtectedData.Protect(dec, entropy, DataProtectionScope.CurrentUser);
                Settings.Default.EWSPassword = Convert.ToBase64String(enc);
            }
        }

        public static string EWSDomain
        {
            get { return Settings.Default.EWSDomain;  }
            set { Settings.Default.EWSDomain = value;  }
        }

        public static string EWSURL
        {
            get { return Settings.Default.EWSURL; }
            set { Settings.Default.EWSURL = value; }
        }

        public static string EWSServer
        {
            get { return Settings.Default.EWSServer; }
            set { Settings.Default.EWSServer = value; } 
        }

        public static string BuildEWSURL(string server)
        {
            return string.Format(Settings.Default.EWSURLPattern, server);
        }

        public static List<string> FoldersToWatch
        {
            get { return Settings.Default.FoldersToWatch.Split(';').ToList(); }
            set { Settings.Default.FoldersToWatch = String.Join(";", value.ToArray()); }
        }
    }
}
