using NeoLoad.Settings;
using System;
using Tricentis.TCAddOns;
using Tricentis.TCAPIObjects.Objects;

namespace NeoLoad.AddOn
{
    class TransferWebToNeoLoadTask : TCAddOnTask
    {
        public override Type ApplicableType => typeof(ExecutionEntry);

        public override string Name => "Transfer Web test case to NeoLoad";

        public override TCObject Execute(TCObject objectToExecuteOn, TCAddOnTaskContext taskContext)
        {
            NeoLoadSettings.WriteSettingsToUserFile("WEB");
            var exec = (objectToExecuteOn as ExecutionEntry).Run();
            NeoLoadSettings.DeleteUserFile();
            return exec;
        }
    }
}
