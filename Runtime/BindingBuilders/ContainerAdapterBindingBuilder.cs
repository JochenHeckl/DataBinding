using System;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.Linq;

namespace de.JochenHeckl.Unity.DataBinding
{
    [DebuggerDisplay( "ContainerBinding[ {sourcePath}, {elementPrefab.transform.gameObject.name} ]" )]
    public class ContainerAdapterBindingBuilder : PropertyBindingBuilder, IBindingBuilder
    {
        #region Unity Editor Properties
        
        public ViewBehaviour elementPrefab;
		public Transform targetTransform;

		#endregion

		public IBinding BuildBinding()
        {
            var boundPropertyType = GetSourcePropertyType();

            if( boundPropertyType == null )
            {
                return null;
            }

            var elementType = GetElementType( boundPropertyType );

            var adapterType = typeof(ContainerAdapter<>).MakeGenericType(elementType);
            var adapter = Activator.CreateInstance(adapterType, elementPrefab.gameObject, targetTransform );

            var enumerableType = typeof( IEnumerable<> ).MakeGenericType( elementType );
            var bindingType = typeof(OneWayPropertyAdapterBinding<>).MakeGenericType( enumerableType );
            
            var binding = Activator.CreateInstance(bindingType, adapter, sourcePath ) as IBinding;
            
            return binding;
        }

        private Type GetElementType( Type containerType )
        {
            if (containerType.IsArray)
            {
                return containerType.GetElementType();
            }

            if (containerType.IsGenericType)
            {
                return containerType.GetGenericArguments().Single();
            }

            throw new InvalidOperationException( $"Can not derive element type for container type {containerType.Name}." );
        }
    }
}
