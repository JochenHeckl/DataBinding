using System;
using System.Linq;
using System.Reflection;

using UnityEditor;

using UnityEngine;

namespace de.JochenHeckl.Unity.DataBinding.Editor
{
    [CustomEditor( typeof( ComponentPropertyBindingBuilder ) )]
    internal class ComponentPropertyBindingBuilderEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var componentPropertyBindingBuilder = serializedObject.targetObject as ComponentPropertyBindingBuilder;
            var dataSourceType = componentPropertyBindingBuilder.gameObject.GetComponent<ViewBehaviour>().dataSourceType.Type;

            var validationData = IsValidBindingBuilder( dataSourceType, componentPropertyBindingBuilder );

            if ( validationData.isValid )
            {
                EditorGUILayout.LabelField( new GUIContent(
                    $"<b>{componentPropertyBindingBuilder.sourcePath}</b> :" +
                    $" <b><color=blue>{validationData.sourceProperty.PropertyType.Name}</color></b> is bound to " +
                    $"<b>{componentPropertyBindingBuilder.targetComponent.GetType().Name}.{componentPropertyBindingBuilder.targetPath}</b>",
                    $"Bound to the component {componentPropertyBindingBuilder.targetComponent} on game object {componentPropertyBindingBuilder.gameObject.name}." ),
                    new GUIStyle(){ richText = true });
            }
            else
            {
                DrawPropertiesExcluding( serializedObject, "m_Script" );
            }

            serializedObject.ApplyModifiedProperties();
        }

		private (bool isValid, PropertyInfo sourceProperty) IsValidBindingBuilder( Type dataSourceType, ComponentPropertyBindingBuilder componentPropertyBindingBuilder )
		{
            var sourceProperty = Array.Find( dataSourceType.GetProperties(), x => x.Name == componentPropertyBindingBuilder.sourcePath );

            var isValid = (sourceProperty != null);

            return ( isValid, sourceProperty );
		}
	}
}
