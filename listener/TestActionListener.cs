
using NeoLoad.Client;
using NeoLoad.Settings;
using System;
using Tricentis.Automation.AutomationInstructions.TestActions;
using Tricentis.Automation.Creation;
using Tricentis.Automation.Engines.Monitoring;
using Tricentis.Automation.Execution.Context;
using Tricentis.Automation.Execution.Results;

namespace NeoLoad.Listener
{
    public class TestActionListener : MonitoringTaskExecutor {

        public TestActionListener(Validator validator) : base(validator) {
        }

        private bool IsSendingToNeoLoad()
        {
            return NeoLoadSettings.IsSendingToNeoLoad();
        }

        public override void PreExecution(ITestAction testAction)
        {
            if (!IsSendingToNeoLoad() || NeoLoadDesignApiInstance.GetInstance().IsRecordStarted())
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
            if (!IsSendingToNeoLoad() || NeoLoadDesignApiInstance.GetInstance().IsRecordStarted())
            {
                return;
            }
            if (testAction is ISpecialExecutionTaskTestAction && ((testAction as ISpecialExecutionTaskTestAction).GetParameter("SapConnection", true) != null || testAction.Name.Value.Contains("SAP Logon"))) {
                // We are after SAP Logon, we can start SAP recording in NeoLoad.
                NeoLoadDesignApiInstance.GetInstance().StartSapRecording();
            }

        }

        public override void PreExecution()
        {
            if (IsSendingToNeoLoad())
            {
                string testCaseName = RunContext.GetAdditionalExecutionInfo("testcase.name");
                NeoLoadDesignApiInstance.GetInstance().SetUserPathName(testCaseName);
            }
        }

        public override void PostExecution(ExecutionResult result)
        {
            afterExecution();
        }

        public override void PostExecution(Exception result)
        {
            afterExecution();
        }

        private void afterExecution()
        {
            if (IsSendingToNeoLoad())
            {
                System.Threading.Thread.Sleep(2000);
                NeoLoadDesignApiInstance.GetInstance().StopRecording();
            }
        }
    }
}
