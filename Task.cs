using System;
using Tricentis.TCAddOns;
using Tricentis.TCAPIObjects.Objects;

namespace NeoLoadAddOn
{
    class Task : TCAddOnTask
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
                return "Send To NeoLoad";
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
