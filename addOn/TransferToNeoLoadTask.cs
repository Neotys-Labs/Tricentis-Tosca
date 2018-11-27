using NeoLoad.Settings;
using System;
using Tricentis.TCAddOns;
using Tricentis.TCAPIObjects.Objects;

namespace NeoLoad.AddOn
{
    class TransferToNeoLoadTask : TCAddOnTask
    {
        public override Type ApplicableType
        {
            get
            {
                return typeof(ExecutionEntry);
            }
        }

        public override string Name
        {
            get
            {
                return "Transfert to NeoLoad";
            }
        }

        public override TCObject Execute(TCObject objectToExecuteOn, TCAddOnTaskContext taskContext)
        {
            NeoLoadSettings.WriteSettingsToUserFile();
            var exec = (objectToExecuteOn as ExecutionEntry).Run();
            NeoLoadSettings.DeleteUserFile();
            return exec;
        }
    }
}
