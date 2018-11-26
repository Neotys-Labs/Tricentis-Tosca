
using System.Collections.Generic;
using Tricentis.Automation.AutomationInstructions.TestActions;
using Tricentis.Automation.Creation;
using Tricentis.Automation.Engines.Monitoring;
using Tricentis.Automation.Execution.Results;

namespace NeoLoadAddOn
{
    public class TestActionListener : MonitoringTaskExecutor {

        private bool _sendingToNeoLoad;

        public TestActionListener(Validator validator) : base(validator) {
            this._sendingToNeoLoad = NeoLoadSettings.IsSendingToNeoLoad();
        }

        public override void PostExecution(ITestAction testAction, ExecutionResult result) {
            if (!_sendingToNeoLoad)
            {
                return;
            }
            if (testAction is SpecialExecutionTaskTestAction && (testAction as SpecialExecutionTaskTestAction).GetParameter("SapConnection") != null) {
                // We are after SAP Logon, we can start SAP recording in NeoLoad.
                NeoLoadDesignApiInstance.GetInstance().StartSapRecording();
            }

        }
    }
}
