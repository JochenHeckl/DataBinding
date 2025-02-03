using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace JH.DataBinding.Editor
{
    [CustomPropertyDrawer(typeof(ContainerPropertyBinding))]
    public class ContainerPropertyBindingPropertyDrawer : PropertyDrawer
    {
        internal static readonly IDataBindingEditorDisplayText EditorDisplayText =
            new DataBindingEditorDisplayText();

        private VisualElement rootVisualElement;
        private PropertyField propertyField;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            rootVisualElement = new();
            rootVisualElement.name = $"ContainerPropertyBinding for {property.displayName}";

            FillInRoot(rootVisualElement, property);
            return rootVisualElement;
        }

        private void FillInRoot(VisualElement root, SerializedProperty property)
        {
            root.Clear();

            propertyField = new PropertyField(property, MakeLabelHeaderText(property));
            root.Add(propertyField);
            root.MarkDirtyRepaint();
        }

        private string MakeLabelHeaderText(SerializedProperty property)
        {
            var (bindingState, bindableDataSourceProperties) =
                DataBindingCommonData.DetermineContainerPropertyBindingState(property);

            return MakeLabelHeaderText(
                property.boxedValue as ContainerPropertyBinding,
                bindingState,
                bindableDataSourceProperties
            );
        }

        private string MakeLabelHeaderText(
            ContainerPropertyBinding binding,
            ContainerPropertyBindingState bindingState,
            PropertyInfo[] bindableDataSourceProperties
        )
        {
            var displayText = DataBindingCommonData.EditorDisplayText;

            switch (bindingState)
            {
                case ContainerPropertyBindingState.MissingDataSourceAssignment:
                    return displayText.BindingMissingDataSourceAssignment;

                case ContainerPropertyBindingState.NoBindableProperties:
                    return displayText.BindingNoBindableProperties;

                case ContainerPropertyBindingState.SourceUnbound:
                    return displayText.BindingSourceUnboundMessageText;

                case ContainerPropertyBindingState.TargetUnbound:
                    return displayText.BindingTargetUnboundMessageText;

                case ContainerPropertyBindingState.ElementTemplateMissing:
                    return displayText.BindingElementTemplateMissingMessageText;

                case ContainerPropertyBindingState.ElementTemplateIsNotAssignable:
                    return displayText.BindingElementTemplateIsNotAssignableMessageText;

                case ContainerPropertyBindingState.Complete:

                    var sourceProperty = bindableDataSourceProperties.Single(x =>
                        x.Name == binding.SourcePath
                    );

                    var friendlySourceTypeName = sourceProperty
                        .PropertyType.GetTypeInfo()
                        .GetFriendlyName();

                    var condensedLabelString = string.Format(
                        displayText.ContainerPropertyBindingCondensedLabelFormat_Type_Source_Target_Template,
                        friendlySourceTypeName,
                        binding.SourcePath,
                        binding.TargetContainer.name,
                        binding.ElementTemplate.name
                    );

                    return condensedLabelString;

                default:
                    return $"Unknown {nameof(ContainerPropertyBinding)} detected.";
            }
        }
    }
}
