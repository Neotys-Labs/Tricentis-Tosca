using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomSet
{
    class MyResource : Tricentis.Automation.Resources.IResource
    {

        public string id;

        public MyResource(string absoluteId)
        {
            this.id = absoluteId;
        }

        public void Dispose()
        {
        }
    }
}
