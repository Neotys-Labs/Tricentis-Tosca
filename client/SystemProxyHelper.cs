using System;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace NeoLoad.Client
{
    internal class SystemProxyHelper
    {
        private RegistryKey registry = null;
        private bool _proxyInUse = false;
        private string _proxySettings = null;
        private string _proxyOverride = null;
        private int _apiPort = 0;

        [DllImport("wininet.dll")]
        public static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);
        public const int INTERNET_OPTION_SETTINGS_CHANGED = 39;
        public const int INTERNET_OPTION_REFRESH = 37;
        static bool settingsReturn, refreshReturn;

        public SystemProxyHelper(int apiPort)
        {
            registry = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
            _proxyInUse = Convert.ToBoolean(registry.GetValue("ProxyEnable"));
            _proxySettings = (string)registry.GetValue("ProxyServer");
            _proxyOverride = (string)registry.GetValue("ProxyOverride");
            _apiPort = apiPort;
        }

        public void setProxy(string host, int port, string excluded)
        {
            registry.SetValue("ProxyEnable", 1);
            registry.SetValue("ProxyServer", String.Format("http={0}:{1};https={0}:{1}", host, port));
            string proxExclusion = host + ":" + _apiPort;
            if (!String.IsNullOrEmpty(excluded))
            {
                excluded = excluded + ";" + proxExclusion;
            }
            else
            {
                excluded = proxExclusion;
            }
            registry.SetValue("ProxyOverride", excluded);
            updateProxySettings();
        }

        public void restoreProxy()
        {
            try
            {
                if (_proxySettings != null && _proxySettings.Length > 0)
                {
                    registry.SetValue("ProxyEnable", _proxyInUse ? 1 : 0);
                    registry.SetValue("ProxyServer", _proxySettings);
                    registry.SetValue("ProxyOverride", _proxyOverride == null ? "" : _proxyOverride);
                }
                else
                {
                    registry.SetValue("ProxyEnable", 0);
                    registry.SetValue("ProxyServer", "");
                    registry.SetValue("ProxyOverride", "");
                }
                updateProxySettings();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /*
         * These lines implement the Interface in the beginning of program 
         * They cause the OS to refresh the settings, causing proxy to realy update
         */
        private void updateProxySettings()
        {
            settingsReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
            refreshReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);
        }
    }
}