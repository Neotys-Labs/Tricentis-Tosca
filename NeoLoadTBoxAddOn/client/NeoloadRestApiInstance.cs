using NeoLoad.Client;
using NeoLoad.Settings;
using Neotys.CommonAPI.Client;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace NeoLoadAddOn.client
{
    class NeoloadRestApiInstance
    {
        private static NeoloadRestApiInstance _instance;
        private static string TCAPI_VERSION;

        private NeotysAPIClientJson _client;

        private NeoloadRestApiInstance(string host, string port, string apiKey)
        {
            _client = new NeotysAPIClientJson(host, port, apiKey);
        }

        public static NeoloadRestApiInstance GetInstance()
        {
            if (_instance == null)
            {
                Dictionary<string, string> properties = NeoLoadSettings.ReadSettingsFromUserFile();
                string host = properties[NeoLoadSettings.API_HOSTNAME_KEY];
                string port = properties[NeoLoadSettings.API_PORT_KEY];
                string token = properties[NeoLoadSettings.API_KEY_KEY];
                TCAPI_VERSION = properties[NeoLoadSettings.TCAPI_VERSION];
                _instance = new NeoloadRestApiInstance(host, port, token);
            }
            return _instance;
        }


        public Task<WebResponse> SendUsageEvent(String _action, NeoLoadDesignApiInstance.Protocol _recordType)
        {
            var data = new
            {
                productName = "Tosca",
                productVersion = TCAPI_VERSION,
                action = _action,
                additionalInformation = new
                {
                    addonVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                    recordType = _recordType.ToString()
                }
            };

            return _client.PostUsage(new JavaScriptSerializer().Serialize(data));
        }
    }
}
