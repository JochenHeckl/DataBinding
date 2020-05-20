using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace de.JochenHeckl.Unity.DataBinding.Example
{
    public class TwoWayDataBindingSetup : MonoBehaviour
    {
        public ViewBehaviour twoWayDataBindingView;

        public void Start()
        {
            twoWayDataBindingView.DataSource = new TwoWayDataBindingViewModel()
            {
                TwoWayProperty = 1.0
            };
        }
    }
}