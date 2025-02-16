using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace JH.DataBinding.Editor
{
    internal static class DataBindingCommonData
    {
        public static string DefaultDataSourceTemplateNamePlaceHolder => "{{name}}";
        public static string DefaultDataSourceTemplate =>
            File.ReadAllText(
                "Packages/de.jochenheckl.unity.databinding/Editor/DefaultDataSourceTemplate.txt"
            );

        public static string dataSourceTypeInspectorFilter { get; set; } = string.Empty;

        internal static readonly IDataBindingEditorDisplayText EditorDisplayText =
            new DataBindingEditorDisplayText();

        internal static Type[] GetValidDataSourceTypes()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            return assemblies
                .Where(x => !x.IsDynamic)
                .SelectMany(x => x.ExportedTypes)
                .Where(DataSourceFilterFunc)
                .OrderBy(x => x.GetFriendlyName())
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

        internal static (
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

        internal static (
            ComponentPropertyBindingState bindingState,
            PropertyInfo[] bindableProperties
        ) DetermineComponentPropertyBindingState(
            ComponentPropertyBinding binding,
            Type dataSourceType
        )
        {
            if (binding == null)
            {
                throw new ArgumentNullException(nameof(binding));
            }

            if (dataSourceType == null)
            {
                return (
                    ComponentPropertyBindingState.MissingDataSourceAssignment,
                    Array.Empty<PropertyInfo>()
                );
            }

            var bindableDataSourceProperties = GetBindableDataSourceProperties(dataSourceType);

            if (bindableDataSourceProperties.Length == 0)
            {
                return (
                    ComponentPropertyBindingState.NoBindableProperties,
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

        internal static (
            ContainerPropertyBindingState bindingState,
            PropertyInfo[] bindableProperties
        ) DetermineContainerPropertyBindingState(SerializedProperty serializedProperty)
        {
            var binding = serializedProperty.boxedValue as ContainerPropertyBinding;

            var view = serializedProperty.serializedObject.targetObject as View;

            if (view == null)
            {
                throw new InvalidOperationException(
                    "The provided ContainerPropertyBinding must be part of a view to dermine the binding state."
                );
            }

            return DetermineContainerPropertyBindingState(binding, view.dataSourceType.Type);
        }

        internal static (
            ContainerPropertyBindingState bindingState,
            PropertyInfo[] bindableDataSourceProperties
        ) DetermineContainerPropertyBindingState(
            ContainerPropertyBinding binding,
            Type dataSourceType
        )
        {
            if (binding == null)
            {
                throw new ArgumentNullException(nameof(binding));
            }

            if (dataSourceType == null)
            {
                return (
                    ContainerPropertyBindingState.MissingDataSourceAssignment,
                    Array.Empty<PropertyInfo>()
                );
            }

            var bindableDataSourceProperties = GetBindableDataSourceEnumerableProperties(
                dataSourceType
            );

            if (bindableDataSourceProperties.Length == 0)
            {
                return (
                    ContainerPropertyBindingState.NoBindableProperties,
                    bindableDataSourceProperties
                );
            }

            var sourceProperty = bindableDataSourceProperties.FirstOrDefault(x =>
                x.Name == binding.SourcePath
            );

            if (sourceProperty == null)
            {
                return (ContainerPropertyBindingState.SourceUnbound, bindableDataSourceProperties);
            }

            if (binding.TargetContainer == null)
            {
                return (ContainerPropertyBindingState.TargetUnbound, bindableDataSourceProperties);
            }

            if (binding.ElementTemplate == null)
            {
                return (
                    ContainerPropertyBindingState.ElementTemplateMissing,
                    bindableDataSourceProperties
                );
            }

            if (binding.ElementTemplate.GetComponent<View>() == null)
            {
                return (
                    ContainerPropertyBindingState.ElementTemplateIsNotAssignable,
                    bindableDataSourceProperties
                );
            }

            return (ContainerPropertyBindingState.Complete, bindableDataSourceProperties);
        }

        internal static PropertyInfo[] GetBindableDataSourceProperties(Type dataSourceType)
        {
            return dataSourceType.GetProperties().Where(x => x.CanRead).ToArray();
        }

        internal static PropertyInfo[] GetBindableDataSourceEnumerableProperties(
            Type dataSourceType
        )
        {
            return dataSourceType
                .GetProperties()
                .Where(x => x.CanRead && typeof(IEnumerable).IsAssignableFrom(x.PropertyType))
                .ToArray();
        }

        internal static PropertyInfo[] GetBindableComponentProperties(
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

        internal static string GetComponentDisplayName(Component component)
        {
            if (component != null)
            {
                return $"{component.GetType().Name} ({component.name})";
            }
            else
            {
                return null;
            }
        }
    }
}
