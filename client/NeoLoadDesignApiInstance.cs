using NeoLoad.Settings;
using Neotys.DesignAPI.Client;
using Neotys.DesignAPI.Model;
using System;
using System.Collections.Generic;
using System.IO;

namespace NeoLoad.Client
{
    class NeoLoadDesignApiInstance
    {
        private static NeoLoadDesignApiInstance _instance = null;

        private IDesignAPIClient _client = null;
        private string _userPathName = null;
        private bool _userPathExist = false;
        private bool _recordStarted = false;

        private NeoLoadDesignApiInstance(string host, string port, string token) {
            string url = "http://" + host + ":" + port + "/Design/v1/Service.svc/";
            _client = DesignAPIClientFactory.NewClient(url, token);
        }

        public static NeoLoadDesignApiInstance GetInstance()
        {
            if (_instance == null)
            {
                Dictionary<string, string> properties = NeoLoadSettings.ReadSettingsFromUserFile();
                string host = properties[NeoLoadSettings.API_HOSTNAME_KEY];
                string port = properties[NeoLoadSettings.API_PORT_KEY];
                string token = properties[NeoLoadSettings.API_KEY_KEY];
                _instance = new NeoLoadDesignApiInstance(host, port, token);
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

        public void StartSapRecording()
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
                _startRecordingPB.isSapGuiProtocol(true);
            
                _client.StartRecording(_startRecordingPB.Build());
            } catch (Exception e)
            {
                _recordStarted = false;
                _instance = null;
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
            }
            finally
            {
                _recordStarted = false;
                _instance = null;
            }
        }
    }
}
