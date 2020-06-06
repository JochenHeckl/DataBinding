using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace de.JochenHeckl.Unity.DataBinding.Example
{
    public class TwoWayDataBindingView : ViewBehaviour
    {
        public Action<int> HandleSelectionChangedCallback { get; set; }

        public void HandleDropDownSelectionChanged( int selectedValue )
        {
            if ( HandleSelectionChangedCallback != null )
            {
                HandleSelectionChangedCallback( selectedValue );
            }
        }
    }
}