using System;
using UnityEngine;

namespace JH.DataBinding
{
    [Serializable]
    public class SerializableType
    {
        public Type Type
        {
            get
            {
                if (!String.IsNullOrEmpty(assemblyQualifiedName))
                {
                    return System.Type.GetType(assemblyQualifiedName);
                }

                return null;
            }
            set
            {
                if (value != null)
                {
                    assemblyQualifiedName = value.AssemblyQualifiedName;
                }
                else
                {
                    assemblyQualifiedName = string.Empty;
                }
            }
        }

        [SerializeField]
        public string assemblyQualifiedName;
    }
}
