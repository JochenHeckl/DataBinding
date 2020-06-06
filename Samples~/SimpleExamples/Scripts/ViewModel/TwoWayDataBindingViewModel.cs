using System;
using System.Collections.Generic;
using UnityEngine.UI;
using static UnityEngine.UI.Dropdown;

namespace de.JochenHeckl.Unity.DataBinding.Example
{
    public class TwoWayDataBindingViewModel : ViewModelBase
    {
        public double TwoWayProperty { get; set; }
        public string OneWayTextProperty { get { return TwoWayProperty.ToString(); } }

        public List<OptionData>  DropDownOptions { get; set; }

        private int selectedDropDownOption;
        public int SelectedDropDownOption
        {
            get
            {
                return selectedDropDownOption;
            }
            set
            {
                selectedDropDownOption = value;
                SelectionMessage = $"You selected {DropDownOptions[value].text}.";

                // k, so we trigger NotifyViewModelChanged from within a reverse binding.
                
                NotifyViewModelChanged();

                // I imho generally consider this bad practice, but for the sake of notifying the change we have to do it.

                // If you are tempted to implement something similar within your ViewModel to actually execute application level code, you are doing it wrong!
                // To do it right, go write an action or a command and bind it to a property off your view
                // see ReactingToViewSelectionChangedDoneRight and ExplicitSelectionMessage.
            }
        }

        public string SelectionMessage { get; set; }

        public Action<int> ReactingToViewSelectionChangedDoneRight { get; set; }
        public string ExplicitSelectionMessage { get; set; }
    }
}