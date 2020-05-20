using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace de.JochenHeckl.Unity.DataBinding.Editor
{
    public class SerializableTypeDrawer : PropertyDrawer
    {
        protected virtual void OnGUIInternal( Rect position, SerializedProperty property, GUIContent label, Type[] dropDownTypes )
        {
            EditorGUI.BeginProperty(position, label, property);

            var propertyInstance = fieldInfo.GetValue( property.serializedObject.targetObject );
            var propertyType = propertyInstance.GetType();

            SerializableType affectedInstance = null;
            var drawLabel = !propertyType.IsArray;

            if ( propertyType.IsArray )
            {
                var propertyInstanceAsArray = propertyInstance as SerializableType[];
                var elementIndex = ExtractElementIndex( property.propertyPath );

                if ( elementIndex < propertyInstanceAsArray.Length )
                {
                    affectedInstance = propertyInstanceAsArray[ elementIndex ];
                }
            }
            else
            {
                affectedInstance = propertyInstance as SerializableType;
            }

            if ( affectedInstance != null )
            {
                var selectedIndex = Array.IndexOf( dropDownTypes, affectedInstance.Type );

                if ( drawLabel )
                {
                    selectedIndex = EditorGUI.Popup( position, label.text, selectedIndex, dropDownTypes.Select( x => x.GetFriendlyName() ).ToArray() );
                }
                else
                {
                    selectedIndex = EditorGUI.Popup( position, selectedIndex, dropDownTypes.Select( x => x.GetFriendlyName() ).ToArray() );
                }

                if ( selectedIndex != -1 )
                {
                    affectedInstance.Type = dropDownTypes[ selectedIndex ];
                }
            }
            EditorGUI.EndProperty();
        }

        private int ExtractElementIndex( string propertyPath )
        {
            return Convert.ToInt32( propertyPath.Split( '[' ).Last().Split( ']' ).First() );
        }
    }
}
