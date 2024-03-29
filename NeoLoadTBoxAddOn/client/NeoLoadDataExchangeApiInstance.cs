﻿using System;
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
        private Context _context;
        private IDataExchangeAPIClient _client;
        private List<Entry> _entryCache;
        private string _connectedUrl;
        private string _connectedApiKey;
        private string _currentScript;

        private List<Task> _tasks;
        
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
        public bool Connect(string hostname, string port, string key, string scriptInfo = "", string softwareInfo = "", string osInfo = "", string hardwareInfo = "", string locationInfo = "")
        {
            string url = "http://" + hostname + ":" + port + "/DataExchange/v1/Service.svc/";

            if (IsConnected)
            {
               Disconnect();
            }

            _connectedUrl = url;
            _connectedApiKey = key;
            _currentScript = scriptInfo;
            _entryCache = new List<Entry>();
            _tasks = new List<Task>();

            try
            {
                _context = CreateContext(scriptInfo, softwareInfo, osInfo, hardwareInfo, locationInfo);
                _client = DataExchangeAPIClientFactory.NewClient(_connectedUrl, _context, _connectedApiKey);

                IsConnected = true;
            }
            catch (NeotysAPIException e) when (e.Message == "NL-DATAEXCHANGE-NO-TEST-RUNNING")
            {
                IsConnected = false;
            }

            return IsConnected;
        }

        /// <summary>
        /// Create context used when connection is established
        /// </summary>
        public Context CreateContext(string script = "", string software = "", string os = "", string hardware = "", string location = "", string instanceId = "")
        {
            Context context = new ContextBuilder
            {
                InstanceId = string.IsNullOrEmpty(instanceId) ? Environment.MachineName + "_" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK") : instanceId,
                Location = string.IsNullOrEmpty(location) ? TimeZone.CurrentTimeZone.StandardName
                    .Replace("Standard", "").Replace("Time", "") : location,
                Hardware = string.IsNullOrEmpty(location) ? (Environment.Is64BitOperatingSystem ? "64-bit, " : "32-bit, ") + 
                                                            Environment.ProcessorCount + " Processors" : hardware,
                Os = string.IsNullOrEmpty(os) ? Environment.OSVersion.ToString() : os,
                Software = software,
                Script = script
            }.build();

            return context;
        }

        /// <summary>
        /// Builds and returns a new DataExchange entry from the given parameters
        /// </summary>
        protected Entry CreateEntry(List<string> path, long timestamp, double value, string unit, string url, bool passed, string message)
        {
            if (timestamp == 0) timestamp = EntryBuilder.CurrentTimeMilliseconds;

            Entry entry = new EntryBuilder(path, timestamp)
            {
                Value = value,
                Unit = unit,
                Url = url,
                Status = new StatusBuilder
                {
                    State = passed ? Status.State.Pass : Status.State.Fail,
                    Message = message
                }.Build()
            }.Build();

            return entry;
        }

        /// <summary>
        /// Send external data entry to NeoLoad via the DataExchangeApi
        /// </summary>
        public void SendEntry(List<string> path, long timestamp, double value, string unit, string url = "", bool passed = true, string message = "")
        {
            Entry entry = CreateEntry(path, timestamp, value, unit, url, passed, message);
            _tasks.Add(Task.Factory.StartNew(() => { _client.AddEntry(entry); }));
        }

        /// <summary>
        /// Send external data entry to NeoLoad via the DataExchangeApi
        /// </summary>
        public void SendEntry(List<string> path, DateTime time, double value, string unit, bool passed = true, string message = "")
        {
            long timestamp = GetTimestampFromDate(time);
            SendEntry(path, timestamp, value, unit, "", passed, message);
        }

        /// <summary>
        /// Add an entry to the entry cache, to send it in bulk afterwards using SendCachedEntries()
        /// </summary>
        public void AddEntryToCache(List<string> path, long timestamp, double value, string unit, string url = "", bool passed = true, string message = "")
        {
            Entry entry = CreateEntry(path, timestamp, value, unit, url, passed, message);
            _entryCache.Add(entry);
        }

        /// <summary>
        /// Add an entry to the entry cache, to send it in bulk afterwards using SendCachedEntries()
        /// </summary>
        public void AddEntryToCache(List<string> path, DateTime time, double value, string unit, bool passed = true, string message = "")
        {
            long timestamp = GetTimestampFromDate(time);
            AddEntryToCache(path, timestamp, value, unit, "", passed, message);
        }

        /// <summary>
        /// Send all data entries cached in the EntryCache to NeoLoad via the DataExchangeApi
        /// </summary>
        public void SendCachedEntries()
        {
            if (_entryCache.Count > 0)
            {
                List<Entry> entries = _entryCache;
                _tasks.Add(Task.Factory.StartNew(() => { _client.AddEntries(entries); }));
                _entryCache = new List<Entry>();
            }
        }

        public void Disconnect()
        {
            //Wait for all task to finish before disconnecting
            Task.WaitAll(_tasks.ToArray());

            _client = null;
            IsConnected = false;
        }

        /// <summary>
        /// Converts the given DateTime to Unix time in ms as long
        /// </summary>
        public static long GetTimestampFromDate(DateTime time)
        {
            DateTime java1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long) time.ToUniversalTime().Subtract(java1970).TotalMilliseconds;
        }
    }
}
