using System;
using System.Collections.Generic;

using static UnityEngine.UI.Dropdown;

namespace de.JochenHeckl.Unity.DataBinding.Example
{
    public class UserInputHandlingDataSource
        : DataSourceBase<UserInputHandlingDataSource>
    {
        public string TypedText { get; set; }
        public string SelectedDropDownText { get; set; }
        public string PressCounterText { get; set; }

        public List<OptionData> DropDownOptions { get; set; }
    }
}
