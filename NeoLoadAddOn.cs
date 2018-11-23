using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tricentis.TCAddOns;

namespace CustomSet
{
    public class NeoLoadAddOn : TCAddOn
    {
        public override string UniqueName
        {
            get
            {
                return "NeoLoadAddOn";
            }
        }

        public override string DisplayedName
        {
            get
            {
                return "NeoLoad AddOn";
            }
        }
    }
}
