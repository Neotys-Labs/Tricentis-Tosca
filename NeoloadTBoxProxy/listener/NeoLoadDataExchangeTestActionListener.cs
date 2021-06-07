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

        /// <summary>
        /// Establish connection to the DataExchangeApi by extracting the connection details from the test configuration parameters
        /// </summary>
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

        /// <summary>
        /// Returns true if the test configuration parameter UseNeoLoadDataExchangeApi is set
        /// </summary>
        public bool DataExchangeApiEnabled => MainConfiguration.Instance.TryGetBool("UseNeoLoadDataExchangeApi",
                                                   out bool exchangePerformanceData) && exchangePerformanceData;

        /// <summary>
        /// Returns true if the current execution is triggered via an execution list
        /// </summary>
        public bool IsUsingExecutionList =>
            !string.IsNullOrEmpty(RunContext.GetAdditionalExecutionInfo("executionlist.name"));

        /// <summary>
        /// Sends the elapsed time after the execution of a testAction via the the DataExchangeApi
        /// </summary>
        public override void PostExecution(ITestAction testAction, ExecutionResult result)
        {
            if (NeoLoadDataExchangeApiInstance.GetInstance().IsConnected)
            {
                var path = CreatePath();
                var elapsedMs = (result.EndTime - result.StartTime).TotalMilliseconds;
                NeoLoadDataExchangeApiInstance.GetInstance().AddEntry(path, elapsedMs, "Milliseconds", result.IsPositive(), result.Message);
            }
        }

        /// <summary>
        /// Creates a List of strings of the currently executed AbstractAuomationObject and its parent up to the TestCase level 
        /// </summary>
        public List<string> CreatePath()
        {
            var list = CreatePathInternal(RunContext.Current, new List<string>());
            list.Reverse();
            return list;
        }

        /// <summary>
        /// Iterates from the currently executed item up to the test case level
        /// </summary>
        private List<string> CreatePathInternal(RunContext parent, List<string> path)
        {
            if (parent == null)
            {
                return path;
            }

            if (parent.ExecutedItem is AbstractAutomationObject ao) // Folder, TestStep
            {
                path.Add(ao.Name);
            }

            return CreatePathInternal(parent.Parent, path);
        }
    }
}
