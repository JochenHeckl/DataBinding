using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JH.DataBinding
{
    [Serializable]
    public class SerializableTypes
    {
        public Type this [ int i ]
        {
            get
            {
                if ( !String.IsNullOrEmpty( assemblyQualifiedName[i] ) )
                {
                    return Type.GetType( assemblyQualifiedName[i] );
                }

                return null;
            }
            set
            {
                if ( value != null )
                {
                    assemblyQualifiedName[ i ] = value.AssemblyQualifiedName;
                }
                else
                {
                    assemblyQualifiedName[ i ] = string.Empty;
                }
            }
        }

        public IEnumerable<Type> Types
        {
            get { return assemblyQualifiedName.Select( x => Type.GetType( x ) ); }
        }

        [SerializeField]
        public string[] assemblyQualifiedName;
    }
}
