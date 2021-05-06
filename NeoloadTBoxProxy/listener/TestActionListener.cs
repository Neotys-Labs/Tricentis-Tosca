
using NeoLoad.Client;
using NeoLoad.Settings;
using System;
using Tricentis.Automation.AutomationInstructions.TestActions;
using Tricentis.Automation.Contract;
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
            if (!IsSendingToNeoLoad())
            {
                return;
            }
            if (NeoLoadDesignApiInstance.GetInstance().IsRecordStarted())
            {
                if (NeoLoadDesignApiInstance.GetInstance().IsRecordWeb() || !NeoLoadDesignApiInstance.GetInstance().IsCreateTransactionBySapTCode())
                {
                    UpdateTransaction(testAction);
                }
                return;
            }

            if (NeoLoadDesignApiInstance.GetInstance().IsRecordSap() && (testAction.Name.Value.Equals("SAP") || testAction.Name.Value.Contains("SAP Login")))
            {
                // We are before SAP Login, we can start SAP recording in NeoLoad.
                System.Threading.Thread.Sleep(2000);
                NeoLoadDesignApiInstance.GetInstance().StartRecording(NeoLoadDesignApiInstance.Protocol.SAP);
                if (!NeoLoadDesignApiInstance.GetInstance().IsCreateTransactionBySapTCode())
                {
                    UpdateTransaction(testAction);
                }
            }

            if (NeoLoadDesignApiInstance.GetInstance().IsRecordWeb())
            {
                // We are before a web event, we can start WEB recording in NeoLoad.
                NeoLoadDesignApiInstance.GetInstance().StartRecording(NeoLoadDesignApiInstance.Protocol.WEB);
                UpdateTransaction(testAction);
            }
        }

        public override void PostExecution(ITestAction testAction, ExecutionResult result)
        {
            if (!IsSendingToNeoLoad() || NeoLoadDesignApiInstance.GetInstance().IsRecordStarted())
            {
                return;
            }
            if ((testAction is ISpecialExecutionTaskTestAction && (testAction as ISpecialExecutionTaskTestAction).GetParameter("SapConnection", true) != null) || testAction.Name.Value.Contains("Logon")) {
                // We are after SAP Logon, we can start SAP recording in NeoLoad.
                NeoLoadDesignApiInstance.GetInstance().StartRecording(NeoLoadDesignApiInstance.Protocol.SAP);
            }
        }

        private void UpdateTransaction(ITestAction testAction)
        {
            var parentItem = RunContext.Current.Parent.Parent != null ? 
                RunContext.Current.Parent.Parent.ExecutedItem 
                : RunContext.Current.Parent.ExecutedItem;
            var transactionName = parentItem is Folder ? (parentItem as Folder).Name : testAction.Name.Value;
            NeoLoadDesignApiInstance.GetInstance().CreateTransaction(transactionName);
        }

        public override void PreExecution()
        {
            if (IsSendingToNeoLoad())
            {
                string testCaseId = RunContext.GetAdditionalExecutionInfo("testcase.uniqueid");
                string testCaseName = RunContext.GetAdditionalExecutionInfo("testcase.name");
                NeoLoadDesignApiInstance.GetInstance().SetUserPathName(testCaseName + " - Tosca");

                // Start recording for API Testing test cases
                if (!NeoLoadDesignApiInstance.GetInstance().IsRecordStarted() && NeoLoadDesignApiInstance.GetInstance().IsRecordWeb())
                {
                    // We are before a web event, we can start WEB recording in NeoLoad.
                    NeoLoadDesignApiInstance.GetInstance().StartRecording(NeoLoadDesignApiInstance.Protocol.WEB);
                }
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
