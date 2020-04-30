using NeoLoad.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using Tricentis.TCAPI;
using Tricentis.TCAPIObjects.Objects;


namespace NeoLoad.AddOn
{
    public class NeoloadToscaApi
    {
        // See documentation at https://support.tricentis.com/community/manuals_detail.do?version=13.1.0&url=topic3.html&tcapi=tcapi
        public static void UpdateTestCaseDescription()
        {
            using (TCAPI tcapi = TCAPI.Instance is null ? TCAPI.CreateInstance() : TCAPI.Instance)
            {
                Dictionary<string, string> properties = NeoLoadSettings.ReadSettingsFromUserFile();
                string testCaseId = properties[NeoLoadSettings.TEST_CASE_UNIQUE_ID];
                // FIXME Problem here the ActiveWorkspace is not set, and we don't have access to the opened workspace path.
                // TCWorkspace workspace = tcapi.ActiveWorkspace;
                TCWorkspace workspace = tcapi.OpenWorkspace(@"C:\Tosca_Projects\Tosca_Workspaces\Jpetstore\Jpetstore.tws","Admin", "");
                TCProject project = workspace.GetProject();
                TestCase testCase = (TestCase)project.Search($"=>SUBPARTS:TestCase[UniqueId==\"{testCaseId}\"]").First();
                testCase.SetAttibuteValue("description", $"{testCase.Description}\nTransferred to Neoload on {DateTime.Now.ToString("MM/dd/yyyy HH:mm")}");
            }
        }
    }
}
