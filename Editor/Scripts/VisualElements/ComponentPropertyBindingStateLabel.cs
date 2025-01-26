using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;

namespace JH.DataBinding.Editor
{
    internal class ComponentPropertyBindingStateLabel : VisualElement
    {
        public ComponentPropertyBindingStateLabel(SerializedProperty property)
        {
            var displayText = DataBindingCommonData.EditorDisplayText;
            var (bindingState, bindableDataSourceProperties) =
                DataBindingCommonData.DetermineComponentPropertyBindingState(property);

            if (bindingState == ComponentPropertyBindingState.MissingDataSourceAssignment)
            {
                var errorLabel = new Label(displayText.BindingMissingDataSourceAssignment);
                errorLabel.AddToClassList(DataBindingEditorStyles.ErrorText);

                Add(errorLabel);
                return;
            }

            if (bindingState == ComponentPropertyBindingState.SourceUnbound)
            {
                var errorLabel = new Label(displayText.BindingSourceUnboundMessageText);
                errorLabel.AddToClassList(DataBindingEditorStyles.ErrorText);

                Add(errorLabel);
                return;
            }

            if (bindingState == ComponentPropertyBindingState.TargetUnbound)
            {
                var errorLabel = new Label(displayText.BindingTargetUnboundMessageText);
                errorLabel.AddToClassList(DataBindingEditorStyles.ErrorText);

                Add(errorLabel);
                return;
            }

            if (bindingState == ComponentPropertyBindingState.Unassignable)
            {
                var errorLabel = new Label(displayText.BindingUnassignableMessageText);
                errorLabel.AddToClassList(DataBindingEditorStyles.ErrorText);

                Add(errorLabel);
                return;
            }

            if (property.boxedValue is ComponentPropertyBinding binding)
            {
                var sourceProperty = bindableDataSourceProperties.Single(x =>
                    x.Name == binding.SourcePath
                );

                var friendlySourceTypeName = sourceProperty
                    .PropertyType.GetTypeInfo()
                    .GetFriendlyName();

                var condensedLabelString = string.Format(
                    displayText.ComponentPropertyBindingCondensedLabelFormat_Type_Source_Target_Component,
                    friendlySourceTypeName,
                    binding.SourcePath,
                    binding.TargetComponent.name,
                    $"{binding.TargetComponent.GetType().Name}.{binding.TargetPath}"
                );

                var condensedLabel = new Label(condensedLabelString);
                condensedLabel.AddToClassList(DataBindingEditorStyles.condensedBindingLabel);

                Add(condensedLabel);
            }
        }
    }
}
