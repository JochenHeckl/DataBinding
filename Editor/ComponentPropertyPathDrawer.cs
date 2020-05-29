using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace de.JochenHeckl.Unity.DataBinding.Editor
{
    [CustomPropertyDrawer( typeof( ComponentPropertyPathAttribute ) )]
    internal class ComponentPropertyPathDrawer : PropertyDrawer
    {
        public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
        {
            var componentPropertyBindingBuilder = property.serializedObject.targetObject as ComponentPropertyBindingBuilder;
            var boundComponent = componentPropertyBindingBuilder.targetComponent;

            if (boundComponent != null)
            {
                var boundTypeFilter = GetSourcePropertyTypeFilter( componentPropertyBindingBuilder );

                var bindableProperties = boundComponent.GetType()
                    .GetProperties( BindingFlags.Public | BindingFlags.Instance )
                    .Where( x => boundTypeFilter( x.PropertyType ) )
                    .Select( x => x.Name )
                    .OrderBy( x => x )
                    .ToArray();

                EditorGUI.BeginProperty( position, label, property );

                if (bindableProperties.Any())
                {
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
                }
                else
                {
                    EditorGUI.PropertyField( position, property );
                }

                EditorGUI.EndProperty();
            }
        }

        private Func<Type, bool> GetSourcePropertyTypeFilter( ComponentPropertyBindingBuilder componentPropertyBindingBuilder )
        {
            var sourcePropertyType = componentPropertyBindingBuilder.GetSourcePropertyType();

            if (sourcePropertyType != null)
            {
                return (x => x.IsAssignableFrom( sourcePropertyType ));
            }

            return ( x ) => false;
        }
    }
}


