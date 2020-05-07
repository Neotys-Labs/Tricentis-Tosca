using System;
using System.Configuration;
using System.Reflection;
using Tricentis.TCAddOns;
using Tricentis.TCAPI;

namespace NeoLoad.Settings
{
    public class NeoLoadSettings : TCAddOnOptionsDialogPage
    {
        public static readonly string API_PORT_KEY = "NeoLoadApiPort";
        public static readonly string API_KEY_KEY = "NeoLoadApiKey";
        public static readonly string API_HOSTNAME_KEY = "NeoLoadApiHostname";
        public static readonly string CREATE_TRANSACTION_BY_SAP_TCODE_KEY = "CreateTransactionBySapTCode";
        public static readonly string RECORD_WEB_OR_SAP = "RecordWebOrSap";
        public static readonly string TCAPI_VERSION = "tcapiVersion";

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
            using (TCAPI tcapi = TCAPI.Instance is null ? TCAPI.CreateInstance() : TCAPI.Instance)
            {
                PropertyInfo APIVersionString = tcapi.GetType().GetProperty("APIVersionString");
                string[] lines = { API_PORT_KEY + "=" + Settings.Default.NeoLoadApiPort,
                    API_KEY_KEY + "=" + Settings.Default.NeoLoadApiKey,
                    API_HOSTNAME_KEY + "=" + Settings.Default.NeoLoadApiHostname,
                    CREATE_TRANSACTION_BY_SAP_TCODE_KEY + "=" + Settings.Default.CreateTransactionBySapTCode,
                    RECORD_WEB_OR_SAP + "=" + recordWebOrSap,
                    TCAPI_VERSION + "=" + (APIVersionString == null ? tcapi.APIVersionAndBuild.ToString() : APIVersionString.GetValue(tcapi)),
                };
                System.IO.File.WriteAllLines(GetUserFilePath(), lines);
            }
        }

        public static void DeleteUserFile()
        {
            System.IO.File.Delete(GetUserFilePath());
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
