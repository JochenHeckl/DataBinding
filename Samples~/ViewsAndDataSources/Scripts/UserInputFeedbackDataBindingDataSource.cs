using System;
using System.Collections.Generic;

using static UnityEngine.UI.Dropdown;

namespace de.JochenHeckl.Unity.DataBinding.Example
{
    public class UserInputFeedbackDataBindingDataSource: DataSourceBase<UserInputFeedbackDataBindingDataSource>
    {
        public string TypedText { get; set; }
        public string SelectedDropDownText { get; set; }

        public List<OptionData>  DropDownOptions { get; set; }

        public Action<int> HandleDropDownSelectionChanged { get; set; }
        public Action<string> HandleTypedTextChanged { get; set; }

        public List<ElementDataSource> DynamicElements { get; set; }
    }
}