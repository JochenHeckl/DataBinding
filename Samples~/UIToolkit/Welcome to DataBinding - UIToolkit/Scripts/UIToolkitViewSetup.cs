using System.Collections;
using System.Collections.Generic;
using de.JochenHeckl.Unity.DataBinding;
using UnityEngine;
using UnityEngine.UIElements;

namespace de.JochenHeckl.Unity.DataBinding.Examples
{
    public class UIToolkitViewSetup : MonoBehaviour
    {
        public string helloMessage;
        public UIDocumentView view;
        public Color helloMessageTextColor;
        public Color helloMessageTextOutlineColor;

        [Range( 0f, 2f)]
        public float helloMessageTextOutlineWidth = 1f;

        public void Start()
        {

            view.DataSource = new UIToolkitViewDataSource()
            {
                HelloMessage = helloMessage,
                HelloMessageTextColor = helloMessageTextColor,
                HelloMessageTextOutlineColor = helloMessageTextOutlineColor,
                HelloMessageTextOutlineWidth = helloMessageTextOutlineWidth
            };
        }
    }
}