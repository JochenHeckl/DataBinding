using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace de.JochenHeckl.Unity.DataBinding.Editor
{
    internal enum ComponentPropertyBindingState
    {
        SourceUnbound,
        TargetUnbound,
        Unassignable,
        Complete,
    }

    internal enum VisualElementPropertyBindingState
    {
        RootVisualElementUnboud,
        SourceUnbound,
        TargetElementUnbound,
        TargetPropertyUnbound,
        Unassignable,
        Complete,
    }
}
