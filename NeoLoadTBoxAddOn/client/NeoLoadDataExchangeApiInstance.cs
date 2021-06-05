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
        private static NeoLoadDataExchangeApiInstance _instance = null;
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

        public bool IsConnected { get; private set; }

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

        public void AddEntry(string name, double value, string unit, bool passed = true, string message = "")
        {
            AddEntry(new List<string> {name}, value, unit, passed, message);
        }

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
