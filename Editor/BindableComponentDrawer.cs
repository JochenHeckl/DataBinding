using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace de.JochenHeckl.Unity.DataBinding.Editor
{
    [CustomPropertyDrawer( typeof( BindableComponentAttribute ) )]
    internal class BindableComponentAttributeDrawer : PropertyDrawer
    {
        private struct BindableComponentData
        {
            public Component Component;
            public string Label;
        }

        public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
        {
            var propertyBindingBuilder = property.serializedObject.targetObject as PropertyBindingBuilder;

            if (propertyBindingBuilder == null)
            {
                throw new InvalidOperationException( "BindableComponent Attribute can only be used on PropertyBindingBuilder Components." );
            }

            var boundField = property.serializedObject.targetObject.GetType().GetField( property.name );

            BindableComponentData[] bindableComponents = null;

            if (propertyBindingBuilder.targetGameObject == null)
            {
                bindableComponents = propertyBindingBuilder.gameObject.GetComponentsInChildren( boundField.FieldType, true )
                .Select( x => new BindableComponentData()
                {
                    Component = x,
                    Label = $"{x.GetType().Name} ({x.gameObject.name})"
                } )
                .ToArray();
            }
            else
            {
                bindableComponents = propertyBindingBuilder.targetGameObject.GetComponents( boundField.FieldType )
               .Select( x => new BindableComponentData()
               {
                   Component = x,
                   Label = $"{x.GetType().Name} ({x.gameObject.name})"
               } )
               .ToArray();
            }

            EditorGUI.BeginProperty( position, label, property );

            var selectedIndex = Array.IndexOf( bindableComponents.Select( x => x.Component ).ToArray(), property.objectReferenceValue );

            var newSelectedIndex = EditorGUI.Popup( position, label.text, selectedIndex, bindableComponents.Select( x => x.Label ).ToArray() );

            if (newSelectedIndex != -1)
            {
                propertyBindingBuilder.targetGameObject = bindableComponents[newSelectedIndex].Component.gameObject;
                property.objectReferenceValue = bindableComponents[newSelectedIndex].Component;
            }

            EditorGUI.EndProperty();
        }
    }
}