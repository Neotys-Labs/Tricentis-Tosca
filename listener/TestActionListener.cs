
using NeoLoad.Client;
using NeoLoad.Settings;
using Tricentis.Automation.AutomationInstructions.TestActions;
using Tricentis.Automation.Creation;
using Tricentis.Automation.Engines.Monitoring;
using Tricentis.Automation.Execution.Results;

namespace NeoLoad.Listener
{
    public class TestActionListener : MonitoringTaskExecutor {

        private bool _sendingToNeoLoad;

        public TestActionListener(Validator validator) : base(validator) {
            _sendingToNeoLoad = NeoLoadSettings.IsSendingToNeoLoad();
        }

        public override void PreExecution(ITestAction testAction)
        {
            if (!_sendingToNeoLoad || NeoLoadDesignApiInstance.GetInstance().IsRecordStarted())
            {
                return;
            }

            if (testAction.Name.Value.Equals("SAP") || testAction.Name.Value.Contains("SAP Login"))
            {
                // We are before SAP Login, we can start SAP recording in NeoLoad.
                System.Threading.Thread.Sleep(2000);
                NeoLoadDesignApiInstance.GetInstance().StartSapRecording();
            }
        }

        public override void PostExecution(ITestAction testAction, ExecutionResult result)
        {
            if (!_sendingToNeoLoad || NeoLoadDesignApiInstance.GetInstance().IsRecordStarted())
            {
                return;
            }
            if (testAction is SpecialExecutionTaskTestAction && ((testAction as SpecialExecutionTaskTestAction).GetParameter("SapConnection", true) != null || testAction.Name.Value.Contains("SAP Logon"))) {
                // We are after SAP Logon, we can start SAP recording in NeoLoad.
                NeoLoadDesignApiInstance.GetInstance().StartSapRecording();
            }

        }
    }
}
