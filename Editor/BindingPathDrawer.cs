using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace de.JochenHeckl.Unity.DataBinding.Editor
{
    [CustomPropertyDrawer( typeof( BindingPathAttribute ) )]
    internal class BindingPathDrawer : PropertyDrawer
    {
        public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
        {
            var propertyBindingBuilder = property.serializedObject.targetObject as PropertyBindingBuilder;

            if (propertyBindingBuilder == null)
            {
                throw new InvalidOperationException( "BindingPath Attribute can only be used on PropertyBindingBuilder Components." );
            }

            var dataSourceType = propertyBindingBuilder.GetDataSourceType();

            if (dataSourceType != null)
            {
                var bindableProperties =
                    dataSourceType
                    .GetProperties( BindingFlags.Public | BindingFlags.Instance ).Select( x => x.Name )
                    .OrderBy( x => x )
                    .ToArray();

                EditorGUI.BeginProperty( position, label, property );

                var selectedIndex = Array.IndexOf( bindableProperties, property.stringValue );

                if ( selectedIndex == -1 )
                {
                    GUI.backgroundColor = Color.red;
                }

                var newSelectedIndex = EditorGUI.Popup( position, label.text, selectedIndex, bindableProperties );

                if (newSelectedIndex != -1)
                {
                    property.stringValue = bindableProperties[newSelectedIndex];
                }

                EditorGUI.EndProperty();
            }
            else
            {
                EditorGUI.BeginProperty( position, label, property );
                EditorGUI.PropertyField( position, property );
                EditorGUI.EndProperty();
            }
        }
    }
}


