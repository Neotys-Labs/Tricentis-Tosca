using NeoLoad.Settings;
using System;
using Tricentis.TCAddOns;
using Tricentis.TCAPIObjects.Objects;

namespace NeoLoad.AddOn
{
    class TransferSapToNeoLoadTask : TCAddOnTask
    {
        public override Type ApplicableType => typeof(ExecutionEntry);

        public override string Name => "Transfer SAP test case to NeoLoad";

        public override TCObject Execute(TCObject objectToExecuteOn, TCAddOnTaskContext taskContext)
        {
            NeoLoadSettings.WriteSettingsToUserFile(NeoLoadSettings.RECORD_SAP);
            var exec = (objectToExecuteOn as ExecutionEntry).Run();
            NeoLoadSettings.DeleteUserFile();
            return exec;
        }
    }
}
