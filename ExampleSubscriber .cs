using System;
using Tricentis.Automation.Execution;

namespace NeoLoadAddOn
{
    public class ExampleSubscriber : ExecutionEventHandler<ExecutionEntryInfo, TestCaseInfo>
    {
        public override void BeforeExecutionEntry(ExecutionEntryInfo executionEntryInfo, TestCaseInfo testCaseInfo, bool isRetrying)
        {
           // TODO get test case entry for User Path
        }
        public override void AfterExecutionEntry(ExecutionEntryInfo executionEntryInfo,
                                                 TestCaseInfo testCaseInfo,
                                                 bool isRetrying,
                                                 Tricentis.Automation.Execution.Results.ExecutionResult executionResult)
        {
            if (NeoLoadSettings.IsSendingToNeoLoad())
            {
                NeoLoadDesignApiInstance.GetInstance().StopRecording();
            }
        }
        public override void AfterExecutionEntry(ExecutionEntryInfo executionEntryInfo, TestCaseInfo testCaseInfo, bool isRetrying, Exception exception)
        {
            if (NeoLoadSettings.IsSendingToNeoLoad())
            {
                NeoLoadDesignApiInstance.GetInstance().StopRecording();
            }
        }
    }
}
