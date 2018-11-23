using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tricentis.Automation.AutomationInstructions.Configuration;
using Tricentis.TCAddOns;

namespace CustomSet
{
    public class NeoLoadSettings : TCAddOnOptionsDialogPage
    {
        protected override ApplicationSettingsBase GetSettingsObject()
        {
            return Settings.Default;
        }
    }
}
