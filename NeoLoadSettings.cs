using System;
using System.Collections.Generic;
using System.Configuration;
using Tricentis.TCAddOns;

namespace NeoLoadAddOn
{
    public class NeoLoadSettings : TCAddOnOptionsDialogPage
    {
        public const string API_PORT_KEY = "NeoLoadApiPort";
        public const string API_TOKEN_KEY = "NeoLoadApiToken";
        public const string API_HOSTNAME_KEY = "NeoLoadApiHostname";

        protected override ApplicationSettingsBase GetSettingsObject()
        {
            return Settings.Default;
        }

        private static string GetUserFilePath()
        {
            string directoryPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            return directoryPath + "/neoload-tosca.properties";
        }

        public static void WriteSettingsToUserFile()
        {
            string[] lines = { API_PORT_KEY + "=" + Settings.Default.NeoLoadApiPort,
                API_TOKEN_KEY + "=" + Settings.Default.NeoLoadApiToken,
                API_HOSTNAME_KEY + "=" + Settings.Default.NeoLoadApiHostname,
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
        protected override string SettingName { get; } = "NeoLoadApiToken";

        protected override string DisplayedName { get; } = "NeoLoadApiToken";
    }

    public class HostnameSetting : TCAddOnOptionsDialogEntry
    {
        protected override string SettingName { get; } = "NeoLoadApiHostname";

        protected override string DisplayedName { get; } = "NeoLoadApiHostname";
    }
}
