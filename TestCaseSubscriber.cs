using System;
using Tricentis.Automation.Execution;

namespace NeoLoadAddOn
{
    public class TestCaseSubscriber : ExecutionEventHandler<ExecutionEntryInfo, TestCaseInfo>
    {
        private bool _sendingToNeoLoad;

        public TestCaseSubscriber() {
            _sendingToNeoLoad = NeoLoadSettings.IsSendingToNeoLoad();
        }

        public override void BeforeExecutionEntry(ExecutionEntryInfo executionEntryInfo, TestCaseInfo testCaseInfo, bool isRetrying)
        {
            if (_sendingToNeoLoad)
            {
                NeoLoadDesignApiInstance.GetInstance().SetUserPathName(testCaseInfo.Name);
            }
        }

        public override void AfterExecutionEntry(ExecutionEntryInfo executionEntryInfo,
                                                 TestCaseInfo testCaseInfo,
                                                 bool isRetrying,
                                                 Tricentis.Automation.Execution.Results.ExecutionResult executionResult)
        {
            afterExecution();
        }

        public override void AfterExecutionEntry(ExecutionEntryInfo executionEntryInfo, TestCaseInfo testCaseInfo, bool isRetrying, Exception exception)
        {
            afterExecution();
        }

        private void afterExecution()
        {
            if (_sendingToNeoLoad)
            {
                System.Threading.Thread.Sleep(2000);
                NeoLoadDesignApiInstance.GetInstance().StopRecording();
            }
        }
    }
}
