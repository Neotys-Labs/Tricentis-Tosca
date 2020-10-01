using System;
using System.Collections.Generic;
namespace NeoLoad.Settings
{
    public class NeoLoadSettings
    {
        public static readonly string API_PORT_KEY = "NeoLoadApiPort";
        public static readonly string API_KEY_KEY = "NeoLoadApiKey";
        public static readonly string API_HOSTNAME_KEY = "NeoLoadApiHostname";
        public static readonly string CREATE_TRANSACTION_BY_SAP_TCODE_KEY = "CreateTransactionBySapTCode";
        public static readonly string HTTP2 = "Http2";
        public static readonly string RECORD_WEB_OR_SAP = "RecordWebOrSap";
        public static readonly string TCAPI_VERSION = "tcapiVersion";


        private static string GetUserFilePath()
        {
            string directoryPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            return directoryPath + "/neoload-tosca.properties";
        }

     
        public static Dictionary<string, string> ReadSettingsFromUserFile()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            foreach (var row in System.IO.File.ReadAllLines(GetUserFilePath()))
                data.Add(row.Split('=')[0], row.Split('=')[1]);
            return data;
        }

        public static bool IsSendingToNeoLoad()
        {
            return System.IO.File.Exists(GetUserFilePath());
        }
    }

}
