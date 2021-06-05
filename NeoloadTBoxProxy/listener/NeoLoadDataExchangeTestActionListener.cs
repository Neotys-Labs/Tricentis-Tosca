using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoLoadAddOn.client;
using Tricentis.Automation.AutomationInstructions.Configuration;
using Tricentis.Automation.AutomationInstructions.TestActions;
using Tricentis.Automation.Contract;
using Tricentis.Automation.Creation;
using Tricentis.Automation.Engines.Monitoring;
using Tricentis.Automation.Execution;
using Tricentis.Automation.Execution.Context;
using Tricentis.Automation.Execution.Results;

namespace NeoLoadAddOn.listener
{
    public class NeoLoadDataExchangeTestActionListener : MonitoringTaskExecutor
    {

        public NeoLoadDataExchangeTestActionListener(Validator validator) : base(validator)
        {
            validator.AssertTrue(() => IsUsingExecutionList && DataExchangeApiEnabled);

            if (validator.IsValid && !NeoLoadDataExchangeApiInstance.GetInstance().IsConnected)
            {
                EstablishConnection();
            }
        }

        private void EstablishConnection()
        {
            if (!MainConfiguration.Instance.TryGet("NeoLoadDataExchangeApiHost", out string hostname))
                hostname = "localhost";
            if (MainConfiguration.Instance.TryGet("NeoLoadDataExchangeApiPort", out string port))
                port = "7400";
            if (MainConfiguration.Instance.TryGet("NeoLoadDataExchangeApiKey", out string key))
                key = "";
            NeoLoadDataExchangeApiInstance.GetInstance().Connect(hostname, port, key);
        }

        public bool DataExchangeApiEnabled => MainConfiguration.Instance.TryGetBool("UseNeoLoadDataExchangeApi",
                                                   out bool exchangePerformanceData) && exchangePerformanceData;

        public bool IsUsingExecutionList =>
            !string.IsNullOrEmpty(RunContext.GetAdditionalExecutionInfo("executionlist.name"));

        public override void PostExecution(ITestAction testAction, ExecutionResult result)
        {
            if (NeoLoadDataExchangeApiInstance.GetInstance().IsConnected)
            {
                var path = CreatePath();
                var elapsedMs = (result.EndTime - result.StartTime).TotalMilliseconds;
                NeoLoadDataExchangeApiInstance.GetInstance().AddEntry(path, elapsedMs, "Milliseconds", result.IsPositive(), result.Message);
            }
        }

        public List<string> CreatePath()
        {
            List<string> list = new List<string>();
            var stack = CreatePathRecursive(RunContext.Current, new Stack());

            foreach (var item in stack)
            {
                list.Add(item.ToString());
            }

            return list;
        }

        private Stack CreatePathRecursive(RunContext parent, Stack path)
        {
            if (parent == null)
            {
                return path;
            }

            if (parent.ExecutedItem is AbstractAutomationObject ao) // Folder, TestStep
            {
                path.Push(ao.Name);
            }

            return CreatePathRecursive(parent.Parent, path);
        }
    }
}
