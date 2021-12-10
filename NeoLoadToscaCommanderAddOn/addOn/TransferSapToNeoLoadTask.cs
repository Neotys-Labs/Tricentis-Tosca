using NeoLoad.Settings;
using System;
using Tricentis.TCAddOns;
using Tricentis.TCAPIObjects.Objects;

namespace NeoLoad.AddOn
{
    class TransferSapToNeoLoadTask : TCAddOnTask
    {
        public override Type ApplicableType => typeof(ExecutionEntry);

        public override string Name => "Transfer SAP GUI test case to NeoLoad";

        public override TCObject Execute(TCObject objectToExecuteOn, TCAddOnTaskContext taskContext)
        {
            NeoLoadSettings.WriteSettingsToUserFile("SAP");
            var exec = (objectToExecuteOn as ExecutionEntry).Run();
            NeoLoadSettings.DeleteUserFile();
            return exec;
        }

        public override int CompareTo(TCAddOnTask taskToCompare)
        {
           if(taskToCompare is TransferSapAndWebToNeoLoadTask)
            {
                return -1;
            } 
            else if (taskToCompare is TransferWebToNeoLoadTask)
            {
                return 1;
            }
            return 0;
        }
    }
}
