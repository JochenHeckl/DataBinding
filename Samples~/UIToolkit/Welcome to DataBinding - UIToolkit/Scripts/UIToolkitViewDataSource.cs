using System.Collections;
using System.Collections.Generic;

using de.JochenHeckl.Unity.DataBinding;

using UnityEngine;
using UnityEngine.UIElements;

namespace de.JochenHeckl.Unity.DataBinding.Examples
{
    public class UIToolkitViewDataSource : DataSourceBase<UIToolkitViewDataSource>
    {
        public string HelloMessage { get; set; }
        public StyleColor HelloMessageTextColor { get; set; }
        public StyleColor HelloMessageTextOutlineColor { get; set; }
        public StyleFloat HelloMessageTextOutlineWidth { get; internal set; }

        //public Color HelloMessageTextOutline { get; set; }
    }
}