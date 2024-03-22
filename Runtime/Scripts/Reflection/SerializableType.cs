using System;
using UnityEngine;

namespace JH.DataBinding
{
    public class TypeFilterAttribute : PropertyAttribute
    {
        public TypeFilterAttribute( Type filterTypeIn )
        {
            FilterType = filterTypeIn;
            FilterFunc = (x) => !x.IsAbstract && !x.IsGenericType && !x.IsInterface && x.InheritsOrImplements( filterTypeIn );
        }

        public Type FilterType { get; }
        public Func<Type, bool> FilterFunc { get; }
    }

    [Serializable]
    public class SerializableType
    {
        public Type Type
        {
            get
            {
                if ( !String.IsNullOrEmpty(assemblyQualifiedName) )
                {
                    return System.Type.GetType(assemblyQualifiedName);
                }

                return null;
            }
            set
            {
                if ( value != null )
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
