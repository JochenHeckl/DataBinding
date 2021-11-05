using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace de.JochenHeckl.Unity.DataBinding
{
	interface IPropertyBinding : IBinding
	{
        object Target
        {
            get;
            set;
        }

        string TargetPath
        {
            get;
            set;
        }
    }
}
