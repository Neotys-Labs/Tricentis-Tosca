using NeoLoad.Settings;
using NeoLoadAddOn.client;
using Neotys.DesignAPI.Client;
using Neotys.DesignAPI.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

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
        private bool _http2;
        private Protocol _protocolToRecord;
        private string currentTransaction = null;

        public enum Protocol
        {
            SAP,
            WEB
        };

        private NeoLoadDesignApiInstance(string host, string port, string token, bool createTransactionBySapTCode, bool http2, Protocol protocolToRecord) {
            string url = "http://" + host + ":" + port + "/Design/v1/Service.svc/";
            _client = DesignAPIClientFactory.NewClient(url, token);
            _apiHost = host;
            _apiPort = int.Parse(port);
            _createTransactionBySapTCode = createTransactionBySapTCode;
            _http2 = http2;
            _protocolToRecord = protocolToRecord;
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
                bool http2 = Boolean.Parse(properties[NeoLoadSettings.HTTP2]);
                Protocol protocolToRecord = (Protocol) Enum.Parse(typeof(Protocol), properties[NeoLoadSettings.RECORD_WEB_OR_SAP]);
                _instance = new NeoLoadDesignApiInstance(host, port, token, createTransactionBySapTCode, http2, protocolToRecord);
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

        public bool IsRecordSap()
        {
            return Protocol.SAP.Equals(_protocolToRecord);
        }

        public bool IsRecordWeb()
        {
            return Protocol.WEB.Equals(_protocolToRecord);
        }

        public bool IsCreateTransactionBySapTCode()
        {
            return _createTransactionBySapTCode;
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
                else if (Protocol.WEB.Equals(protocol))
                {
                    _startRecordingPB.isHTTP2Protocol(_http2);
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
                if (_systemProxyHelper != null)
                {
                    try
                    {
                        _systemProxyHelper.restoreProxy();
                    }
                    catch (Exception ignored)
                    {
                    }
                    _systemProxyHelper = null;
                }
                WriteExceptionToFile(e);
                throw e;
            }

            try
            {
                NeoloadRestApiInstance.GetInstance().SendUsageEvent("recording", protocol);
            } catch(Exception e)
            {
                // Do nothing if send event fails
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

        public void CreateTransaction(string _transactionName)
        {
            if (string.Equals(_transactionName, currentTransaction))
            {
                return;
            }
            currentTransaction = _transactionName;
            try
            {
                _client.SetContainer(new SetContainerParams(_transactionName));
            }
            catch (Exception e)
            {
                WriteExceptionToFile(e);
                throw e;
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
            }
            finally
            {
                _recordStarted = false;
                _instance = null;
                if (_systemProxyHelper != null)
                {
                    try
                    {
                        _systemProxyHelper.restoreProxy();
                    }
                    finally
                    {
                        _systemProxyHelper = null;
                    }
                }
            }
        }
    }
}
