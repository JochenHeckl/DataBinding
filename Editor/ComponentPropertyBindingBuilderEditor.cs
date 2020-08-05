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
            var viewBehaviour = componentPropertyBindingBuilder.gameObject.GetComponent<ViewBehaviour>();
            
            var collapsedView = TestForCollapsedView( viewBehaviour, componentPropertyBindingBuilder );

            if ( collapsedView )
            {
                var sourceProperty = Array.Find( viewBehaviour.dataSourceType.Type.GetProperties(), x => x.Name == componentPropertyBindingBuilder.sourcePath );

                EditorGUILayout.LabelField( new GUIContent(
                    $"<b>{componentPropertyBindingBuilder.sourcePath}</b> :" +
                    $" <b><color=blue>{sourceProperty.PropertyType.Name}</color></b> is bound to " +
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

		private bool TestForCollapsedView( ViewBehaviour viewBehaviour, ComponentPropertyBindingBuilder componentPropertyBindingBuilder )
		{
            if( componentPropertyBindingBuilder == null )
			{
                return false;
			}

            var dataSourceType = viewBehaviour.dataSourceType.Type;
            var targetComponentType = componentPropertyBindingBuilder.targetComponent.GetType();

            var sourceProperty = Array.Find( dataSourceType.GetProperties(), x => x.Name == componentPropertyBindingBuilder.sourcePath );
            var targetProperty = Array.Find( targetComponentType.GetProperties(), x => x.Name == componentPropertyBindingBuilder.targetPath );

            var isValid =
                (sourceProperty != null)
                && (targetProperty != null )
                && targetProperty.PropertyType.IsAssignableFrom( sourceProperty.PropertyType );

            return viewBehaviour.condenseValidBuilders && isValid;
		}
	}
}
