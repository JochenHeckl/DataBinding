using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace JH.DataBinding.Editor
{
    internal static class DataBindingCommonData
    {
        internal static readonly IDataBindingEditorDisplayText EditorDisplayText =
            new DataBindingEditorDisplayText();

        internal static Type[] GetValidDataSourceTypes()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            return assemblies
                .Where(x => !x.IsDynamic)
                .SelectMany(x => x.ExportedTypes)
                .Where(DataSourceFilterFunc)
                .ToArray();

            bool DataSourceFilterFunc(Type x) =>
                !x.IsAbstract
                && !x.IsGenericType
                && !x.IsInterface
                && x.InheritsOrImplements(typeof(INotifyDataSourceChanged));
        }

        internal static Type GuessDataSourceTypeName(string viewName)
        {
            return GetValidDataSourceTypes()
                .Select(x => new { x, distance = viewName.DamerauLevenshteinDistance(x.Name) })
                .OrderBy(x => x.distance)
                .Select(x => x.x)
                .FirstOrDefault();
        }

        internal static string FindDataSourceSourceFile(Type type)
        {
            var assetGuid = AssetDatabase.FindAssets(type.Name).FirstOrDefault();

            if (assetGuid == null)
            {
                var allSourceFiles = Directory.GetFiles(
                    Application.dataPath,
                    "*.cs",
                    SearchOption.AllDirectories
                );

                var sourceFile = allSourceFiles.FirstOrDefault(x =>
                    File.ReadAllText(x).Contains($"class {type.Name}")
                );

                var assetName = Path.GetFileNameWithoutExtension(sourceFile);

                assetGuid = AssetDatabase.FindAssets(assetName).FirstOrDefault();
            }

            if (assetGuid != null)
            {
                return AssetDatabase.GUIDToAssetPath(assetGuid);
            }

            return null;
        }

        public static (
            ComponentPropertyBindingState bindingState,
            PropertyInfo[] bindableProperties
        ) DetermineComponentPropertyBindingState(SerializedProperty serializedProperty)
        {
            var binding = serializedProperty.boxedValue as ComponentPropertyBinding;

            var view = serializedProperty.serializedObject.targetObject as View;

            if (view == null)
            {
                throw new InvalidOperationException(
                    "The provided ComponentPropertyBinding must be part of a view to dermine the binding state."
                );
            }

            return DetermineComponentPropertyBindingState(binding, view.dataSourceType.Type);
        }

        public static (
            ComponentPropertyBindingState bindingState,
            PropertyInfo[] bindableProperties
        ) DetermineComponentPropertyBindingState(
            ComponentPropertyBinding binding,
            Type dataSourceType
        )
        {
            if (binding == null)
            {
                throw new InvalidOperationException(
                    "Provided serialized property is not of type ComponentPropertyBinding."
                );
            }

            var bindableDataSourceProperties = GetBindableDataSourceProperties(dataSourceType);

            if (dataSourceType == null)
            {
                return (
                    ComponentPropertyBindingState.MissingDataSourceAssignment,
                    bindableDataSourceProperties
                );
            }

            var sourceProperty = bindableDataSourceProperties.FirstOrDefault(x =>
                x.Name == binding.SourcePath
            );

            if (sourceProperty == null)
            {
                return (ComponentPropertyBindingState.SourceUnbound, bindableDataSourceProperties);
            }

            var targetProperty =
                binding.TargetComponent != null
                    ? binding
                        .TargetComponent.GetType()
                        .GetProperties()
                        .FirstOrDefault(x => x.Name == binding.TargetPath)
                    : null;

            if (targetProperty == null)
            {
                return (ComponentPropertyBindingState.TargetUnbound, bindableDataSourceProperties);
            }

            if (!targetProperty.PropertyType.IsAssignableFrom(sourceProperty.PropertyType))
            {
                return (ComponentPropertyBindingState.Unassignable, bindableDataSourceProperties);
            }

            return (ComponentPropertyBindingState.Complete, bindableDataSourceProperties);
        }

        public static PropertyInfo[] GetBindableDataSourceProperties(Type dataSourceType)
        {
            return dataSourceType.GetProperties().Where(x => x.CanRead).ToArray();
        }

        public static PropertyInfo[] GetBindableComponentProperties(
            Component component,
            Type sourcePropertyType
        )
        {
            var assignableProperties = component
                .GetType()
                .GetProperties()
                .Where(x => x.CanWrite && x.PropertyType.IsAssignableFrom(sourcePropertyType))
                .ToArray();

            return assignableProperties;
        }

        public static string GetComponentDisplayName(Component component)
        {
            return $"{component.GetType().Name} ({component.name})";
        }
    }
}
