using NeoLoad.Settings;
using Neotys.DesignAPI.Client;
using Neotys.DesignAPI.Model;
using System;
using System.Collections.Generic;
using System.IO;

namespace NeoLoad.Client
{
    public class NeoLoadDesignApiInstance
    {
        private static NeoLoadDesignApiInstance _instance = null;

        private IDesignAPIClient _client = null;
        private string _userPathName = null;
        private bool _userPathExist = false;
        private bool _recordStarted = false;
        private string _apiHost = null;
        private int _apiPort = 0;
        private int _recorderProxyPort = 0;
        private SystemProxyHelper _systemProxyHelper = null;
        private bool _createTransactionBySapTCode;

        public enum Protocol
        {
            SAP,
            HTTP2
        };

        private NeoLoadDesignApiInstance(string host, string port, string token, bool createTransactionBySapTCode) {
            string url = "http://" + host + ":" + port + "/Design/v1/Service.svc/";
            _client = DesignAPIClientFactory.NewClient(url, token);
            _apiHost = host;
            _apiPort = int.Parse(port);
            _createTransactionBySapTCode = createTransactionBySapTCode;
        }

        private void intialize()
        {
            _recorderProxyPort = _client.GetRecorderSettings().ProxySettings.Port;
        }

        public static NeoLoadDesignApiInstance GetInstance()
        {
            if (_instance == null)
            {
                Dictionary<string, string> properties = NeoLoadSettings.ReadSettingsFromUserFile();
                string host = properties[NeoLoadSettings.API_HOSTNAME_KEY];
                string port = properties[NeoLoadSettings.API_PORT_KEY];
                string token = properties[NeoLoadSettings.API_KEY_KEY];
                bool createTransactionBySapTCode = Boolean.Parse(properties[NeoLoadSettings.CREATE_TRANSACTION_BY_SAP_TCODE_KEY]);
                _instance = new NeoLoadDesignApiInstance(host, port, token, createTransactionBySapTCode);
                _instance.intialize();
            }
            return _instance;
        }

        public void SetUserPathName(string name)
        {
            _userPathName = name;
        }

        public bool IsRecordStarted()
        {
            return _recordStarted;
        }

        public void StartRecording(Protocol protocol)
        {
            _recordStarted = true;
            try
            {
                StartRecordingParamsBuilder _startRecordingPB = new StartRecordingParamsBuilder();
                if (_userPathName != null && _userPathName.Length != 0)
                {
                    ContainsUserPathParamsBuilder _containsBuilder = new ContainsUserPathParamsBuilder();
                    _containsBuilder.name(_userPathName);
                    _userPathExist = _client.ContainsUserPath(_containsBuilder.Build());
                    if (_userPathExist)
                    {
                        _startRecordingPB.virtualUser(_userPathName + "_recording");
                    }
                    else
                    {
                        _startRecordingPB.virtualUser(_userPathName);
                    }
                }
                if (Protocol.SAP.Equals(protocol))
                {
                    _startRecordingPB.isSapGuiProtocol(true);
                    _startRecordingPB.isCreateTransactionBySapTCode(_createTransactionBySapTCode);
                }
                else if (Protocol.HTTP2.Equals(protocol))
                {
                    _startRecordingPB.isHTTP2Protocol(true);
                    if(_systemProxyHelper == null)
                    {
                        _systemProxyHelper = new SystemProxyHelper(_apiPort);
                        _systemProxyHelper.setProxy(_apiHost, _recorderProxyPort, "");
                    }
                }

                _client.StartRecording(_startRecordingPB.Build());
            } catch (Exception e)
            {
                _recordStarted = false;
                _instance = null;
                _systemProxyHelper = null;
                WriteExceptionToFile(e);
                throw e;
            }
        }

        private void WriteExceptionToFile(Exception ex)
        {
            string directoryPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string filePath = directoryPath + "/neoload-add-on-error.txt";

            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine("-----------------------------------------------------------------------------");
                writer.WriteLine("Date : " + DateTime.Now.ToString());
                writer.WriteLine();

                while (ex != null)
                {
                    writer.WriteLine(ex.GetType().FullName);
                    writer.WriteLine("Message : " + ex.Message);
                    writer.WriteLine("StackTrace : " + ex.StackTrace);

                    ex = ex.InnerException;
                }
            }
        }

        public void StopRecording()
        {
            StopRecordingParamsBuilder _stopRecordingBuilder = new StopRecordingParamsBuilder();

            if (_userPathExist)
            {
                UpdateUserPathParamsBuilder _updateUserPathBuilder = new UpdateUserPathParamsBuilder();
                _updateUserPathBuilder.name(_userPathName);
                _updateUserPathBuilder.deleteRecording(true);
                _stopRecordingBuilder.updateParams(_updateUserPathBuilder.Build());
            }

            try
            {
                _client.StopRecording(_stopRecordingBuilder.Build());
                _client.SaveProject();
                if (_systemProxyHelper != null)
                {
                    _systemProxyHelper.restoreProxy();
                }
            }
            finally
            {
                _recordStarted = false;
                _instance = null;
                _systemProxyHelper = null;
            }
        }
    }
}
