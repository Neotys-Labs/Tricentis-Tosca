
using Neotys.DesignAPI.Client;
using Neotys.DesignAPI.Model;
using Tricentis.Automation.AutomationInstructions.TestActions;
using Tricentis.Automation.Creation;
using Tricentis.Automation.Engines.Monitoring;
using Tricentis.Automation.Engines.Representations;
using Tricentis.Automation.Engines.Technicals.Sap.Interfaces;
using Tricentis.Automation.Execution.Results;
using Tricentis.Automation.Resources;

namespace CustomSet {
    public class NeoloadTracer : MonitoringTaskExecutor {
        public NeoloadTracer(Validator validator) : base(validator) {
        }

        public override void PreExecution(ITestAction testAction) {
            //TestStep(Value) has been started
            string name = testAction.Name.Value;
            string[] lines = { "type: " + testAction.GetType().Name };
            System.IO.File.WriteAllLines(@"C:\Users\anouvel\Desktop\pre"+name+".txt", lines);
        }

        public override void PreExecution(ISubTestAction testAction)
        {
            string name = testAction.TestAction.ToString();
            string[] lines = { "type: " + testAction.GetType().Name };
            System.IO.File.WriteAllLines(@"C:\Users\anouvel\Desktop\preSub" + name + ".txt", lines);
        }

        public override void PreExecution(IRepresentation context, ITestAction testAction) {
            string name = testAction.Name.Value;
            string[] lines = { "type: " + testAction.GetType().Name };
            System.IO.File.WriteAllLines(@"C:\Users\anouvel\Desktop\preContext" + name + ".txt", lines);
        }

        public override void PreExecution(IRepresentation context, ISubTestAction testAction)
        {
            string name = testAction.TestAction.ToString();
            string[] lines = { "type: " + testAction.GetType().Name };
            System.IO.File.WriteAllLines(@"C:\Users\anouvel\Desktop\preContextSub" + name + ".txt", lines);
        }

        public override void PostExecution(ITestAction testAction, ExecutionResult result) {
            //TestStep(Value) done 
            if (testAction is SpecialExecutionTaskTestAction && (testAction as SpecialExecutionTaskTestAction).GetParameter("SapConnection") != null) {
                // We are in SAP Logon
                IDesignAPIClient client = DesignAPIClientFactory.NewClient("http://SAMOS.intranet.neotys.com:7400/Design/v1/Service.svc/", "");
                StartRecordingParamsBuilder _startRecordingPB = new StartRecordingParamsBuilder();
                _startRecordingPB.isSapGuiProtocol(true);

                client.StartRecording(_startRecordingPB.Build());
            }
            else
            {
                string name = testAction.Name.Value;
                string[] lines = { "type: " + testAction.GetType().Name };
                System.IO.File.WriteAllLines(@"C:\Users\anouvel\Desktop\post" + name + ".txt", lines);
            }
        }
    }
}
