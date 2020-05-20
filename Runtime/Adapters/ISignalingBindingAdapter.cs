using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace de.JochenHeckl.Unity.DataBinding
{
    public interface ISignalingBindingAdapter<ValueType>
    {
        ValueType Value
        {
            get;
            set;
        }
    }
}
