using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tricentis.Automation.Resources;
using Tricentis.TCAddOns;
using Tricentis.TCAPIObjects.Objects;

namespace CustomSet
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
            taskContext.ShowMessageBox("Coucou", "Send to NeoLoad " + Settings.Default.TestNL);
            var exec = (objectToExecuteOn as ExecutionEntry).Run();
            MyResource resource;
            if (ResourceManager.Instance.TryRemove("ID", out resource))
            {
                taskContext.ShowMessageBox("Coucou2", "OjectId " + resource.id);
            }
            else
            {
                taskContext.ShowMessageBox("Coucou2", "OjectId not found");
            }
            return exec;
        }
    }
}
