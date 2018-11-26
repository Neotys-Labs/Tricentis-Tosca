using Neotys.DesignAPI.Client;
using Neotys.DesignAPI.Model;

namespace NeoLoadAddOn
{
    class NeoLoadDesignApiInstance
    {
        private static NeoLoadDesignApiInstance _instance = null;

        private IDesignAPIClient _client = null;

        private NeoLoadDesignApiInstance(string host, string port, string token) {
            string url = "http://" + host + ":" + port + "/Design/v1/Service.svc/";
            _client = DesignAPIClientFactory.NewClient(url, token);
        }

        public static NeoLoadDesignApiInstance GetInstance()
        {
            return _instance;
        }


        public static NeoLoadDesignApiInstance NewInstance(string host, string port, string token)
        {
            _instance = new NeoLoadDesignApiInstance(host, port, token);
            return _instance;
        }

        public void StartSapRecording()
        {
            StartRecordingParamsBuilder _startRecordingPB = new StartRecordingParamsBuilder();
            _startRecordingPB.isSapGuiProtocol(true);

            _client.StartRecording(_startRecordingPB.Build());
        }

        public void StopRecording()
        {
            StopRecordingParamsBuilder _stopRecordingPB = new StopRecordingParamsBuilder();

            _client.StopRecording(_stopRecordingPB.Build());
        }
    }
}
