
using System;
using System.Collections.Generic;
using Tricentis.Automation.AutomationInstructions.TestActions;
using Tricentis.Automation.Creation;
using Tricentis.Automation.Engines.Monitoring;
using Tricentis.Automation.Execution.Results;

namespace NeoLoadAddOn
{
    public class TestActionListener : MonitoringTaskExecutor {

        private bool _sendingToNeoLoad;
        private bool _recordStarted = false;

        public TestActionListener(Validator validator) : base(validator) {
            _sendingToNeoLoad = NeoLoadSettings.IsSendingToNeoLoad();
        }

        public override void PreExecution(ITestAction testAction)
        {
            if (!_sendingToNeoLoad || _recordStarted)
            {
                return;
            }

            if (testAction.Name.Value.Equals("SAP") || testAction.Name.Value.Contains("SAP Login"))
            {
                _recordStarted = true;
                // We are before SAP Login, we can start SAP recording in NeoLoad.
                NeoLoadDesignApiInstance.GetInstance().StartSapRecording();
            }
        }

        public override void PostExecution(ITestAction testAction, ExecutionResult result)
        {
            if (!_sendingToNeoLoad || _recordStarted)
            {
                return;
            }
            if (testAction is SpecialExecutionTaskTestAction && ((testAction as SpecialExecutionTaskTestAction).GetParameter("SapConnection", true) != null || testAction.Name.Value.Contains("SAP Logon"))) {
                _recordStarted = true;
                // We are after SAP Logon, we can start SAP recording in NeoLoad.
                NeoLoadDesignApiInstance.GetInstance().StartSapRecording();
            }

        }
    }
}
