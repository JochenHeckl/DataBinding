using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace de.JochenHeckl.Unity.DataBinding.Editor
{
    [CustomPropertyDrawer( typeof( TypeFilterAttribute ) )]
    public class TypeFilterDrawer : SerializableTypeDrawer
    {
        public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
        {
            var typeFilterAttribute = (TypeFilterAttribute) attribute;

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var validTypes = assemblies
                .Where( x => !x.IsDynamic )
                .SelectMany( x => x.ExportedTypes )
                .Where( t => typeFilterAttribute.FilterFunc( t ) );

            OnGUIInternal( position, property, label, validTypes.ToArray() );
        }
    }
}