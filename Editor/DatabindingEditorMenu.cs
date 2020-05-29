using System;
using System.Linq;
using System.Reflection;

using UnityEditor;
using UnityEngine;

namespace de.JochenHeckl.Unity.DataBinding.Editor
{
    public static class DatabindingEditorMenu
    {
        [MenuItem( "Tools/Data Binding/Find Unbound BindingBuilder" )]
        public static void FindAllUnboundBindings()
        {
            var bindings = GameObject.FindObjectsOfType<PropertyBindingBuilder>();

            var unboundBinding = bindings.FirstOrDefault( 
                x => x.enabled 
                && (string.IsNullOrEmpty( x.sourcePath ) || SourcePathMissmatch( x ) || IsUnboundPropertyBindingBuilder( x )) );

            
            if ( unboundBinding == null )
            {
                EditorUtility.DisplayDialog( 
                    "Data Binding",
                    $"Everything is fine.{Environment.NewLine}{Environment.NewLine}No unbound PropertyBindingBuilder was found.",
                    "OK");

                return;
            }

            Selection.activeObject = unboundBinding;
        }

        private static bool SourcePathMissmatch( PropertyBindingBuilder propertyBindingBuilder )
        {
            var dataSourceType = propertyBindingBuilder.GetDataSourceType();

            if ( dataSourceType != null )
            {
                var bindableProperties =
                    dataSourceType
                    .GetProperties( BindingFlags.Public | BindingFlags.Instance ).Select( x => x.Name )
                    .OrderBy( x => x )
                    .ToArray();

                return Array.IndexOf( bindableProperties, propertyBindingBuilder.sourcePath ) == -1;
            }

            return true;
        }



        private static bool IsUnboundPropertyBindingBuilder( PropertyBindingBuilder bindingBuilder )
        {
            var componentPropertyBindingBuilder = bindingBuilder as ComponentPropertyBindingBuilder;

            return (componentPropertyBindingBuilder == null) || string.IsNullOrEmpty( componentPropertyBindingBuilder.targetPath );
        }
    }
}