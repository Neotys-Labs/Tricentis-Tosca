
using System.Collections.Generic;
using Tricentis.Automation.AutomationInstructions.TestActions;
using Tricentis.Automation.Creation;
using Tricentis.Automation.Engines.Monitoring;
using Tricentis.Automation.Execution.Results;

namespace NeoLoadAddOn
{
    public class NeoloadTracer : MonitoringTaskExecutor {

        bool _sendingToNeoLoad;

        public NeoloadTracer(Validator validator) : base(validator) {
            this._sendingToNeoLoad = NeoLoadSettings.IsSendingToNeoLoad();
        }

        public override void PostExecution(ITestAction testAction, ExecutionResult result) {
            if (!_sendingToNeoLoad)
            {
                return;
            }
            if (testAction is SpecialExecutionTaskTestAction && (testAction as SpecialExecutionTaskTestAction).GetParameter("SapConnection") != null) {
                // We are after SAP Logon, we can start SAP recording in NeoLoad.
                Dictionary<string, string> properties = NeoLoadSettings.ReadSettingsFromUserFile();
                string host = properties[NeoLoadSettings.API_HOSTNAME_KEY];
                string port = properties[NeoLoadSettings.API_PORT_KEY];
                string token = properties[NeoLoadSettings.API_TOKEN_KEY];
                NeoLoadDesignApiInstance.NewInstance(host, port, token).StartSapRecording();
            }

        }
    }
}
