using System;
using System.Runtime.InteropServices;

namespace NeoLoad.Client
{
    public class SystemProxyHelper
    {

        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool InternetGetConnectedState(out int lpdwFlags, int dwReserved);

        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool InternetGetConnectedStateEx(out int lpdwFlags, out string lpszConnectionName, int dwNameLen, int dwReserved);

        [Flags]
        enum ConnectionStates
        {
            Modem = 0x1,
            LAN = 0x2,
            Proxy = 0x4,
            RasInstalled = 0x10,
            Offline = 0x20,
            Configured = 0x40,
        };

        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);

        /// <summary>
        /// Queries an Internet option on the specified handle. The Handle will be always 0.
        /// </summary>
        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true, EntryPoint = "InternetQueryOption")]
        private extern static bool InternetQueryOptionList(IntPtr Handle, INTERNET_OPTION OptionFlag, ref INTERNET_PER_CONN_OPTION_LIST OptionList, ref int size);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct INTERNET_PER_CONN_OPTION_LIST
        {
            public int Size;

            // The connection to be set. NULL means LAN.
            public System.IntPtr Connection;

            public int OptionCount;
            public int OptionError;

            // List of INTERNET_PER_CONN_OPTIONs.
            public System.IntPtr pOptions;
        }

        public enum INTERNET_OPTION
        {
            // Sets or retrieves an INTERNET_PER_CONN_OPTION_LIST structure that specifies
            // a list of options for a particular connection.
            INTERNET_OPTION_PER_CONNECTION_OPTION = 75,

            // Notify the system that the registry settings have been changed so that
            // it verifies the settings on the next call to InternetConnect.
            INTERNET_OPTION_SETTINGS_CHANGED = 39,

            // Causes the proxy data to be reread from the registry for a handle.
            INTERNET_OPTION_REFRESH = 37

        }

        private enum INTERNET_PER_CONN_OptionEnum
        {
            INTERNET_PER_CONN_FLAGS = 1,
            INTERNET_PER_CONN_PROXY_SERVER = 2,
            INTERNET_PER_CONN_PROXY_BYPASS = 3,
            INTERNET_PER_CONN_AUTOCONFIG_URL = 4,
            INTERNET_PER_CONN_AUTODISCOVERY_FLAGS = 5,
            INTERNET_PER_CONN_AUTOCONFIG_SECONDARY_URL = 6,
            INTERNET_PER_CONN_AUTOCONFIG_RELOAD_DELAY_MINS = 7,
            INTERNET_PER_CONN_AUTOCONFIG_LAST_DETECT_TIME = 8,
            INTERNET_PER_CONN_AUTOCONFIG_LAST_DETECT_URL = 9,
            INTERNET_PER_CONN_FLAGS_UI = 10
        }

        private enum INTERNET_OPTION_PER_CONN_FLAGS
        {
            PROXY_TYPE_DIRECT = 0x00000001,   // direct to net
            PROXY_TYPE_PROXY = 0x00000002,   // via named proxy
            PROXY_TYPE_AUTO_PROXY_URL = 0x00000004,   // autoproxy URL
            PROXY_TYPE_AUTO_DETECT = 0x00000008   // use autoproxy detection
        }

        /// <summary>
        /// Used in INTERNET_PER_CONN_OPTION.
        /// When create a instance of OptionUnion, only one filed will be used.
        /// The StructLayout and FieldOffset attributes could help to decrease the struct size.
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        public struct INTERNET_PER_CONN_OPTION_OptionUnion
        {
            // A value in INTERNET_OPTION_PER_CONN_FLAGS.
            [FieldOffset(0)]
            public int dwValue;
            [FieldOffset(0)]
            public System.IntPtr pszValue;
            [FieldOffset(0)]
            public System.Runtime.InteropServices.ComTypes.FILETIME ftValue;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class INTERNET_PER_CONN_OPTION
        {
            // A value in INTERNET_PER_CONN_OptionEnum.
            public int dwOption;
            public INTERNET_PER_CONN_OPTION_OptionUnion Value;
        }

        public const int INTERNET_OPTION_SETTINGS_CHANGED = 39;
        public const int INTERNET_OPTION_REFRESH = 37;

        static bool settingsReturn, refreshReturn;

        private string _connectionName;
        private string _bypass;
        private int _proxyKind;
        private string _proxySettings;
        private int _apiPort = 0;

        public SystemProxyHelper(int apiPort)
        {
            _apiPort = apiPort;
            _connectionName = getConnectionName();
            storeProxyInfo();
        }

        void storeProxyInfo()
        {
            // Query following options. 
            INTERNET_PER_CONN_OPTION[] Options = new INTERNET_PER_CONN_OPTION[3];

            Options[0] = new INTERNET_PER_CONN_OPTION();
            Options[0].dwOption = (int)INTERNET_PER_CONN_OptionEnum.INTERNET_PER_CONN_FLAGS;
            Options[1] = new INTERNET_PER_CONN_OPTION();
            Options[1].dwOption = (int)INTERNET_PER_CONN_OptionEnum.INTERNET_PER_CONN_PROXY_SERVER;
            Options[2] = new INTERNET_PER_CONN_OPTION();
            Options[2].dwOption = (int)INTERNET_PER_CONN_OptionEnum.INTERNET_PER_CONN_PROXY_BYPASS;

            // Allocate a block of memory of the options.
            System.IntPtr buffer = Marshal.AllocCoTaskMem(Marshal.SizeOf(Options[0])
                + Marshal.SizeOf(Options[1]) + Marshal.SizeOf(Options[2]));

            System.IntPtr current = (System.IntPtr)buffer;

            // Marshal data from a managed object to an unmanaged block of memory.
            for (int i = 0; i < Options.Length; i++)
            {
                Marshal.StructureToPtr(Options[i], current, false);
                current = (System.IntPtr)(current + Marshal.SizeOf(Options[i]));
            }

            // Initialize a INTERNET_PER_CONN_OPTION_LIST instance.
            INTERNET_PER_CONN_OPTION_LIST Request = new INTERNET_PER_CONN_OPTION_LIST();

            // Point to the allocated memory.
            Request.pOptions = buffer;

            Request.Size = Marshal.SizeOf(Request);

            Request.Connection = Marshal.StringToHGlobalAuto(_connectionName);

            Request.OptionCount = Options.Length;
            Request.OptionError = 0;
            int size = Marshal.SizeOf(Request);

            // Query internet options. 
            bool result = InternetQueryOptionList(IntPtr.Zero, INTERNET_OPTION.INTERNET_OPTION_PER_CONNECTION_OPTION, ref Request, ref size);
            if (result)
            {
                Marshal.PtrToStructure(new IntPtr(Request.pOptions.ToInt64()), Options[0]);
                Marshal.PtrToStructure(new IntPtr(Request.pOptions.ToInt64() + Marshal.SizeOf(typeof(INTERNET_PER_CONN_OPTION))), Options[1]);
                Marshal.PtrToStructure(new IntPtr(Request.pOptions.ToInt64() + Marshal.SizeOf(typeof(INTERNET_PER_CONN_OPTION)) + Marshal.SizeOf(typeof(INTERNET_PER_CONN_OPTION))), Options[2]);
                _proxyKind = Options[0].Value.dwValue;
                _proxySettings = Marshal.PtrToStringAuto(Options[1].Value.pszValue);
                _bypass = Marshal.PtrToStringAuto(Options[2].Value.pszValue);
            }
            else
            {
                _proxyKind = -1;
                _bypass = null;
                _proxySettings = null;
            }

            Marshal.FreeCoTaskMem(Request.pOptions);
        }
        private String getConnectionName()
        {
            int state = getConnectedState();
            if ((state & (int)ConnectionStates.LAN) > 0)
            {
                // LAN connection detected
                // returns null that means Local Network
                return null;
            }
            // Modem: find connection name
            return getCurrentConnectionName();
        }

        private int getConnectedState()
        {
            int flags;
            InternetGetConnectedState(out flags, 0);
            return flags;
        }

        private String getCurrentConnectionName()
        {
            int flags;
            string buffStr = new String(' ', 255);
            bool connected = InternetGetConnectedStateEx(out flags, out buffStr, 255, 0);
            if (!connected)
            {
                // not connected
                return null;
            }
            else
            {
                return buffStr;
            }
        }



        public void setProxy(string host, int port, string excluded)
        {
            string newProxyServer = String.Format("http={0}:{1};https={0}:{1}", host, port);
            int newProxyKind = (int)(INTERNET_OPTION_PER_CONN_FLAGS.PROXY_TYPE_DIRECT | INTERNET_OPTION_PER_CONN_FLAGS.PROXY_TYPE_PROXY);
            setConnectionProxy(newProxyKind, _connectionName, newProxyServer, null);
            updateProxySettings();
        }

        public void restoreProxy()
        {
            try
            {
                setConnectionProxy(_proxyKind, _connectionName, _proxySettings, _bypass);
                updateProxySettings();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private static bool setConnectionProxy(int proxyKind, string connectionName, string proxyServer, string bypass)
        {

            // Create 3 options.
            INTERNET_PER_CONN_OPTION[] Options = new INTERNET_PER_CONN_OPTION[3];

            // Set PROXY flags.
            Options[0] = new INTERNET_PER_CONN_OPTION();
            Options[0].dwOption = (int)INTERNET_PER_CONN_OptionEnum.INTERNET_PER_CONN_FLAGS;
            Options[0].Value.dwValue = proxyKind;

            // Set proxy name.           
            Options[1] = new INTERNET_PER_CONN_OPTION();
            Options[1].dwOption =
                (int)INTERNET_PER_CONN_OptionEnum.INTERNET_PER_CONN_PROXY_SERVER;
            Options[1].Value.pszValue = Marshal.StringToHGlobalAuto(proxyServer);

            // Set bypass        
            Options[2] = new INTERNET_PER_CONN_OPTION();
            Options[2].dwOption =
                (int)INTERNET_PER_CONN_OptionEnum.INTERNET_PER_CONN_PROXY_BYPASS;
            Options[2].Value.pszValue = bypass == null ? IntPtr.Zero : Marshal.StringToHGlobalAuto(bypass);

            // Initialize a INTERNET_PER_CONN_OPTION_LIST instance.
            INTERNET_PER_CONN_OPTION_LIST option_list = new INTERNET_PER_CONN_OPTION_LIST();

            // default stuff
            option_list.Size = Marshal.SizeOf(option_list);
            option_list.Connection = connectionName == null ? IntPtr.Zero : Marshal.StringToHGlobalAuto(connectionName); ;
            option_list.OptionCount = Options.Length;
            option_list.OptionError = 0;

            var optSize = Marshal.SizeOf(typeof(INTERNET_PER_CONN_OPTION));
            // make a pointer out of all that ...
            var optionsPtr = Marshal.AllocCoTaskMem(optSize * Options.Length);
            // copy the array over into that spot in memory ...
            for (var i = 0; i < Options.Length; i++)
            {
                var opt = new IntPtr(optionsPtr.ToInt64() + (i * optSize));
                Marshal.StructureToPtr(Options[i], opt, false);
            }

            option_list.pOptions = optionsPtr;

            // and then make a pointer out of the whole list
            var ipcoListPtr = Marshal.AllocCoTaskMem(option_list.Size);
            Marshal.StructureToPtr(option_list, ipcoListPtr, false);

            // Set internet settings.
            bool bReturn = InternetSetOption(IntPtr.Zero, (int)INTERNET_OPTION.INTERNET_OPTION_PER_CONNECTION_OPTION, ipcoListPtr, option_list.Size);

            // Free the allocated memory.
            Marshal.FreeCoTaskMem(optionsPtr);
            Marshal.FreeCoTaskMem(ipcoListPtr);

            return bReturn;
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