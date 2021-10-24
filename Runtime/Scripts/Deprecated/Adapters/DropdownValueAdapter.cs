using System;
using System.Globalization;
using UnityEngine.UI;

namespace de.JochenHeckl.Unity.DataBinding
{
    public class DropdownValueAdapter : ISignalingBindingAdapter<int>
    {
        public DropdownValueAdapter( Dropdown componentIn )
        {
            component = componentIn;
        }

        public int Value
        {
            get
            {
                return component.value;
            }
            set
            {
                component.value = value;
            }
        }

        private Dropdown component;
    }
}
