using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tricentis.TCAddOns;

namespace CustomSet
{
    public class ReviewAddOnSetting : TCAddOnOptionsDialogEntry
    {
        protected override string SettingName { get; } = "TestNL";

        protected override string DisplayedName { get; } = "TestNL";
    }
}
