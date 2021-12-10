using NeoLoad.Settings;
using System;
using Tricentis.TCAddOns;
using Tricentis.TCAPIObjects.Objects;

namespace NeoLoad.AddOn
{
    class TransferSapAndWebToNeoLoadTask : TCAddOnTask
    {
        public override Type ApplicableType => typeof(ExecutionEntry);

        public override string Name => "Transfer SAP GUI and Web test case to NeoLoad";

        public override TCObject Execute(TCObject objectToExecuteOn, TCAddOnTaskContext taskContext)
        {
            NeoLoadSettings.WriteSettingsToUserFile("SAP_AND_WEB");
            var exec = (objectToExecuteOn as ExecutionEntry).Run();
            NeoLoadSettings.DeleteUserFile();
            return exec;
        }

        public override int CompareTo(TCAddOnTask taskToCompare)
        {
            return 1;
        }
    }
}
