using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace JH.DataBinding.Editor
{
    [CustomPropertyDrawer(typeof(BindingTargetComponentAttribute))]
    public class ComponentPropertyBindingTargetComponentPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
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
                return new PropertyField(property);
            }

            var componentPropertyBindingValue =
                componentPropertyBinding.boxedValue as ComponentPropertyBinding;

            if (componentPropertyBindingValue.TargetGameObject == null)
            {
                return new PropertyField(property);
            }

            var sourceType = bindableProperties.Single(x =>
                x.Name == componentPropertyBindingValue.SourcePath
            );

            var candidateComponts =
                componentPropertyBindingValue.TargetGameObject.GetComponentsInChildren<Component>();

            var options = candidateComponts
                .Where(x =>
                    DataBindingCommonData
                        .GetBindableComponentProperties(x, sourceType.PropertyType)
                        .Any()
                )
                .Select(x => new
                {
                    component = x,
                    displayName = DataBindingCommonData.GetComponentDisplayName(x),
                })
                .ToArray();

            var stringOptions = options.Select(x => x.displayName).ToList();

            var rootVisualElement = new DropdownField(
                property.displayName,
                stringOptions,
                stringOptions.IndexOf(
                    DataBindingCommonData.GetComponentDisplayName(
                        componentPropertyBindingValue.TargetComponent
                    )
                )
            );

            rootVisualElement.AddToClassList("unity-base-field__aligned");

            rootVisualElement.RegisterValueChangedCallback(changeEvent =>
            {
                property.objectReferenceValue = options
                    .FirstOrDefault(x => x.displayName == changeEvent.newValue)
                    .component;
                property.serializedObject.ApplyModifiedProperties();
            });

            return rootVisualElement;
        }
    }
}
