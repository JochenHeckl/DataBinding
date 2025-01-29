using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace JH.DataBinding.Editor
{
    [CustomPropertyDrawer(typeof(BindingTargetPathAttribute))]
    public class ComponentPropertyBindingTargetPathPropertyDrawer : PropertyDrawer
    {
        private VisualElement rootVisualElement;
        private List<string> options;
        private DropdownField dropDown;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            rootVisualElement = new();

            FillInRoot(rootVisualElement, property);

            return rootVisualElement;
        }

        private void FillInRoot(VisualElement root, SerializedProperty property)
        {
            root.Clear();

            var parentPath = property.propertyPath.Replace($".{property.name}", "");
            SerializedProperty componentPropertyBinding = property.serializedObject.FindProperty(
                parentPath
            );

            (var bindingState, var bindableProperties) =
                DataBindingCommonData.DetermineComponentPropertyBindingState(
                    componentPropertyBinding
                );

            if (
                (bindingState == ComponentPropertyBindingState.MissingDataSourceAssignment)
                || (bindingState == ComponentPropertyBindingState.SourceUnbound)
            )
            {
                root.Add(new PropertyField(property));
                return;
            }

            var componentPropertyBindingValue =
                componentPropertyBinding.boxedValue as ComponentPropertyBinding;

            if (componentPropertyBindingValue.TargetComponent == null)
            {
                root.Add(new PropertyField(property));
                return;
            }

            var sourceType = bindableProperties.Single(x =>
                x.Name == componentPropertyBindingValue.SourcePath
            );

            options = DataBindingCommonData
                .GetBindableComponentProperties(
                    componentPropertyBindingValue.TargetComponent,
                    sourceType.PropertyType
                )
                .Select(x => x.Name)
                .ToList();

            dropDown = new DropdownField(
                property.displayName,
                options,
                options.IndexOf(componentPropertyBindingValue.TargetPath)
            );

            dropDown.AddToClassList("unity-base-field__aligned");

            dropDown.RegisterValueChangedCallback(x =>
            {
                property.stringValue = x.newValue;
                property.serializedObject.ApplyModifiedProperties();

                EditorUtility.SetDirty(property.serializedObject.targetObject);
            });

            //dropDown.TrackPropertyValue(property.FindPropertyRelative(), HandleObjectChanged);

            root.Add(dropDown);
        }

        private void HandleObjectChanged(SerializedObject @object) { }
    }
}
