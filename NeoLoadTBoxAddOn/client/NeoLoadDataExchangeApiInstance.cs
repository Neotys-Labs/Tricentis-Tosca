using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Neotys.DataExchangeAPI.Client;
using Neotys.DataExchangeAPI.Error;
using Neotys.DataExchangeAPI.Model;

namespace NeoLoadAddOn.client
{
    public class NeoLoadDataExchangeApiInstance
    {
        private static NeoLoadDataExchangeApiInstance _instance;
        private IDataExchangeAPIClient _client;

        private NeoLoadDataExchangeApiInstance()
        {
            IsConnected = false;
        }

        public static NeoLoadDataExchangeApiInstance GetInstance()
        {
            if (_instance == null)
            {
                _instance = new NeoLoadDataExchangeApiInstance();
            }
            return _instance;
        }

        /// <summary>
        /// Returns true if the connection to the DataExchangeApi is already established
        /// </summary>
        public bool IsConnected { get; private set; }

        /// <summary>
        /// Connect to the DataExchangeApi if not already connected
        /// Will try to connect again if NL-DATAEXCHANGE-NO-TEST-RUNNING exception is thrown
        /// </summary>
        public bool Connect(string hostname, string port, string key)
        {
            if (IsConnected) return true;

            string url = "http://" + hostname + ":" + port + "/DataExchange/v1/Service.svc/";
            try
            {
                _client = DataExchangeAPIClientFactory.NewClient(url, key);
                IsConnected = true;
            }
            catch (NeotysAPIException e) when (e.Message == "NL-DATAEXCHANGE-NO-TEST-RUNNING")
            {
                IsConnected = false;
            }

            return IsConnected;
        }

        /// <summary>
        /// Send new external data entry to NeoLoad via the DataExchangeApi
        /// </summary>
        public void AddEntry(List<string> path, double value, string unit, bool passed = true, string message = "")
        {
            Entry entry = new EntryBuilder(path)
            {
                Value = value,
                Unit = unit,

                Status = new StatusBuilder
                {
                    State = passed ? Status.State.Pass : Status.State.Fail,
                    Message = message
                }.Build()
            }.Build();

            new Task(() => { _client.AddEntry(entry); }).Start();
        }

    }
}
