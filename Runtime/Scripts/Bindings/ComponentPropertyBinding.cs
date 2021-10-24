using System;

using UnityEngine;

namespace de.JochenHeckl.Unity.DataBinding
{
    [Serializable]
    public class ComponentPropertyBinding : PropertyBinding
    {
        public GameObject targetGameObject;
        public Component targetComponent;
    }
}