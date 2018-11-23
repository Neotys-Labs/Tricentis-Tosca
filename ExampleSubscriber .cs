using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tricentis.Automation.Execution;

namespace CustomSet
{
    public class ExampleSubscriber : ExecutionEventHandler<ExecutionEntryInfo, TestCaseInfo>
    {
        public static readonly string NL = Environment.NewLine;
        public override void BeforeExecutionEntry(ExecutionEntryInfo executionEntryInfo, TestCaseInfo testCaseInfo, bool isRetrying)
        {
            AppendToTempFile($"========================{NL}BEFORE TestCase: {NL}========================{NL}TestCase: {testCaseInfo.Name}{NL}ExecEntry: {executionEntryInfo.UniqueId}");
        }
        public override void AfterExecutionEntry(ExecutionEntryInfo executionEntryInfo,
                                                 TestCaseInfo testCaseInfo,
                                                 bool isRetrying,
                                                 Tricentis.Automation.Execution.Results.ExecutionResult executionResult)
        {
            AppendToTempFile($"========================{NL}AFTER TestCase: {NL}========================{NL}TestCase passed: {executionResult.IsPositive()}{NL}TestCase: {testCaseInfo.Name}{NL}ExecEntry: {executionEntryInfo.UniqueId}");
        }
        public override void AfterExecutionEntry(ExecutionEntryInfo executionEntryInfo, TestCaseInfo testCaseInfo, bool isRetrying, Exception exception)
        {
            AppendToTempFile($"========================{NL}AFTER TestCase: {NL}========================{NL}Exception occurred: {exception}{NL}TestCase: {testCaseInfo.Name}{NL}ExecEntry: {executionEntryInfo.UniqueId}");
        }
        private static void AppendToTempFile(string line)
        {
            File.AppendAllText(@"C:\Users\anouvel\Desktop\Demo.txt", $"{line}{NL}{NL}");
        }
    }
}
