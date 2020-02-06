using System;
using System.Collections.Generic;
using System.Configuration;
using Tricentis.TCAddOns;

namespace NeoLoad.Settings
{
    public class NeoLoadSettings : TCAddOnOptionsDialogPage
    {
        public static readonly string API_PORT_KEY = "NeoLoadApiPort";
        public static readonly string API_KEY_KEY = "NeoLoadApiKey";
        public static readonly string API_HOSTNAME_KEY = "NeoLoadApiHostname";
        public static readonly string CREATE_TRANSACTION_BY_SAP_TCODE_KEY = "CreateTransactionBySapTCode";
        public static readonly string RECORD_WEB_OR_SAP = "RecordWebOrSap";
        public static readonly string RECORD_WEB = "web";
        public static readonly string RECORD_SAP = "sap";

        protected override ApplicationSettingsBase GetSettingsObject()
        {
            return Settings.Default;
        }

        private static string GetUserFilePath()
        {
            string directoryPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            return directoryPath + "/neoload-tosca.properties";
        }

        public static void WriteSettingsToUserFile(String recordWebOrSap)
        {
            string[] lines = { API_PORT_KEY + "=" + Settings.Default.NeoLoadApiPort,
                API_KEY_KEY + "=" + Settings.Default.NeoLoadApiKey,
                API_HOSTNAME_KEY + "=" + Settings.Default.NeoLoadApiHostname,
                CREATE_TRANSACTION_BY_SAP_TCODE_KEY + "=" + Settings.Default.CreateTransactionBySapTCode,
                RECORD_WEB_OR_SAP + "=" + recordWebOrSap
            };
            System.IO.File.WriteAllLines(GetUserFilePath(), lines);
        }

        public static void DeleteUserFile()
        {
            System.IO.File.Delete(GetUserFilePath());
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

    public class PortSetting : TCAddOnOptionsDialogEntry
    {
        protected override string SettingName { get; } = "NeoLoadApiPort";

        protected override string DisplayedName { get; } = "NeoLoadApiPort";
    }

    public class TokenSetting : TCAddOnOptionsDialogEntry
    {
        protected override string SettingName { get; } = "NeoLoadApiKey";

        protected override string DisplayedName { get; } = "NeoLoadApiKey";
    }

    public class HostnameSetting : TCAddOnOptionsDialogEntry
    {
        protected override string SettingName { get; } = "NeoLoadApiHostname";

        protected override string DisplayedName { get; } = "NeoLoadApiHostname";
    }
    public class CreateTransactionBySapTCodeSetting : TCAddOnOptionsDialogEntry
    {
        protected override string SettingName { get; } = "CreateTransactionBySapTCode";

        protected override string DisplayedName { get; } = "CreateTransactionBySapTCode";
    }
}
