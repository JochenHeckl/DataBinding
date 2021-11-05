using System;
using System.Globalization;
using UnityEngine.UI;

namespace de.JochenHeckl.Unity.DataBinding
{
    public class InputFieldTextAdapter<ValueType> : ISignalingBindingAdapter<ValueType>
    {
        public InputFieldTextAdapter( InputField componentIn )
        {
            component = componentIn;
        }

        public ValueType Value
        {
            get
            {
                return (ValueType) Convert.ChangeType( component.text, typeof(ValueType), CultureInfo.InvariantCulture);
            }
            set
            {
                if ( value != null )
                {
                    component.text = value.ToString();
                }
                else
                {
                    component.text = string.Empty;
                }
            }
        }

        private InputField component;
    }
}
