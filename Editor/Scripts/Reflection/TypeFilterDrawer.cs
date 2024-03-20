using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace IC.DataBinding.Editor
{
    [CustomPropertyDrawer( typeof( TypeFilterAttribute ) )]
    public class TypeFilterDrawer : SerializableTypeDrawer
    {
        private Type[] validTypes;

        public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
        {
            var typeFilterAttribute = (TypeFilterAttribute) attribute;

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            if ( validTypes == null )
            {
                validTypes = assemblies
                    .Where( x => !x.IsDynamic )
                    .SelectMany( x => x.ExportedTypes )
                    .Where( t => typeFilterAttribute.FilterFunc( t ) )
                    .ToArray();
            }

            OnGUIInternal( position, property, label, validTypes );
        }
    }
}