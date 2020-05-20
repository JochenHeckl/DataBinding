using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace de.JochenHeckl.Unity.DataBinding.Example
{

    public class ArrayContainerDataBindingSetup : MonoBehaviour
    {
        public ViewBehaviour container;

        void Start()
        {
            container.DataSource = new ArrayContainerDataBindingViewModel()
            {
                Elements = new int[] { 1, 2, 3 }
            };
        }
    }

}