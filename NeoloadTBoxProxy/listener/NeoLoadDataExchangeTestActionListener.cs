using System.Collections.Generic;
using System.Linq;
using NeoLoadAddOn.client;
using Tricentis.Automation.AutomationInstructions.Configuration;
using Tricentis.Automation.AutomationInstructions.Dynamic.Values;
using Tricentis.Automation.AutomationInstructions.TestActions;
using Tricentis.Automation.Contract;
using Tricentis.Automation.Creation;
using Tricentis.Automation.Engines.Monitoring;
using Tricentis.Automation.Engines.Representations;
using Tricentis.Automation.Engines.Technicals.Html;
using Tricentis.Automation.Execution.Context;
using Tricentis.Automation.Execution.Generic;
using Tricentis.Automation.Execution.Results;

namespace NeoLoadAddOn.listener
{
    public class NeoLoadDataExchangeTestActionListener : MonitoringTaskExecutor
    {
        public static bool NewTestStep = false;
        public static string PreviousBrowserDomain;
        public static Dictionary<string, long> PreviousBrowserTimings = new Dictionary<string, long>();

        public NeoLoadDataExchangeTestActionListener(Validator validator) : base(validator)
        {
            validator.AssertTrue(() => IsUsingExecutionList && DataExchangeApiEnabled);
        }

        /// <summary>
        /// Returns true if the test configuration parameter SendEndUserExperienceToNeoLoad is set
        /// </summary>
        public bool DataExchangeApiEnabled => MainConfiguration.Instance.TryGetBool("SendEndUserExperienceToNeoLoad",
                                                   out bool dataExchangeApiEnabled) && dataExchangeApiEnabled;

        /// <summary>
        /// Returns true if the current execution is triggered via an execution list
        /// </summary>
        public bool IsUsingExecutionList =>
            !string.IsNullOrEmpty(RunContext.AdditionalExecutionInfo
                .SingleOrDefault(add => add.Key == "executionlist.name").Value);

        /// <summary>
        /// Establish connection to the DataExchangeApi by extracting the connection details from the test configuration parameters
        /// </summary>
        protected void EstablishConnection()
        {
            if (!MainConfiguration.Instance.TryGet("NeoLoadApiHost", out string hostname))
                hostname = "localhost";
            if (!MainConfiguration.Instance.TryGet("NeoLoadApiPort", out string port))
                port = "7400";
            if (!MainConfiguration.Instance.TryGet("NeoLoadApiKey", out string key))
                key = "";

            RunContext.Instance.TryGetGetAdditionalExecutionInfo("executionentry.nodepath", out string scriptInfo);
            string softwareInfo = NeoLoadExtensionHelper.GetForeGroundWindowCaption();

            NeoLoadDataExchangeApiInstance.GetInstance().Connect(hostname, port, key, scriptInfo, softwareInfo);
        }

        /// <summary>
        /// Only try to connect once at the beginning of the TestCase execution
        /// </summary>
        public override void PreExecution()
        {
            EstablishConnection();
        }

        /// <summary>
        /// Called when a new TestStep is executed
        /// Used afterwards to indicate if the current TestStepValue is the first one inside the TestStep
        /// </summary>
        public override void PreExecution(ITestAction testAction)
        {
            // Only monitor TestSteps/RootTransactions but not TestStepValues
            NewTestStep = true;
        }

        /// <summary>
        /// Called with every TestStepValue but only used once with the first TestStepValue in a new TestStep
        /// As Performance metrics don't need to be extracted again with every single TestStepValue
        /// </summary>
        public override void PreExecution(IRepresentation context, ITestAction testAction)
        {
            if (NewTestStep && NeoLoadDataExchangeApiInstance.GetInstance().IsConnected)
            {
                List<string> basePath = NeoLoadExtensionHelper.CreatePath();
                HandleTechnologySpecificPerformanceTimings(context, basePath);
                NewTestStep = false;
            }
        }

        /// <summary>
        /// Called when the TestStep execution is finished
        /// Sends the TestStep timing via the DataExchange Api to NeoLoad
        /// </summary>
        public override void PostExecution(ITestAction testAction, ExecutionResult result)
        {
            // Only monitor TestSteps/RootTransactions not TestStepValues
            if (NeoLoadDataExchangeApiInstance.GetInstance().IsConnected &&
                (testAction as AutomationObjectTestAction)?.IsRootAction == true)
            {
                double elapsedMs = (result.EndTime - result.StartTime).TotalMilliseconds;
                List<string> path = NeoLoadExtensionHelper.CreatePath();
                path.Add("TestStep Duration");
                NeoLoadDataExchangeApiInstance.GetInstance().SendEntry(path, result.EndTime, elapsedMs, "Ms", result.IsPositive(), result.Message);
            }
        }

        /// <summary>
        /// Called when the TestCase execution is finished
        /// Sends the TestCase timing via the DataExchange Api to NeoLoad
        /// </summary>
        public override void PostExecution(ExecutionResult result)
        {
            if (NeoLoadDataExchangeApiInstance.GetInstance().IsConnected &&
                RunContext.Instance.Current?.Executor is IAutomationObjectTestCaseItemExecutor<ExecutionEntry> executor)
            {
                List<string> path = new List<string>
                {
                    executor.ItemToExecute.Name,
                    "TestCase Duration"
                };

                double elapsedMs = (result.EndTime - result.StartTime).TotalMilliseconds;
                NeoLoadDataExchangeApiInstance.GetInstance().SendEntry(path, result.EndTime, elapsedMs, "Ms", result.IsPositive(), result.Message);
            }
        }

        /// <summary>
        /// Handles technology specific performance timings for HTML
        /// </summary>
        protected virtual void HandleTechnologySpecificPerformanceTimings(IRepresentation context, List<string> basePath)
        {
            // Extract js performance timings if the context is a HtmlDocument
            if (context?.Adapter?.Technical is IHtmlDocumentTechnical htmlTechnical)
            {
                IHtmlEntryPointTechnical htmlEntryPoint = htmlTechnical.EntryPoint;
                string htmlBrowserDomain = htmlTechnical.Url;
                Dictionary<string, long> htmlBrowserTimings = NeoLoadExtensionHelper.GetBrowserPerformanceTimings(htmlEntryPoint);

                // Extract and use the actual domLoadEventEnd timestamp 
                htmlBrowserTimings.TryGetValue("timestamp", out long timestamp);
                htmlBrowserTimings.Remove("timestamp");

                // No need to send the values again if they haven't changed
                if (htmlBrowserDomain != PreviousBrowserDomain || !htmlBrowserTimings.SequenceEqual(PreviousBrowserTimings))
                {
                    foreach (KeyValuePair<string, long> timing in htmlBrowserTimings)
                    {
                        List<string> timingPath = new List<string>(basePath) { timing.Key };
                        NeoLoadDataExchangeApiInstance.GetInstance().AddEntryToCache(timingPath, timestamp, timing.Value, "Ms", htmlBrowserDomain);
                    }

                    PreviousBrowserDomain = htmlBrowserDomain;
                    PreviousBrowserTimings = htmlBrowserTimings;
                }
            }

            NeoLoadDataExchangeApiInstance.GetInstance().SendCachedEntries();
        }

    }
}
