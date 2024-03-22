using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.UIElements;

namespace JH.DataBinding.Editor
{
    internal class ContainerPropertyBindingEditor : BindingEditor<ContainerPropertyBinding>
    {
        private readonly PropertyInfo[] bindableDataSourceProperties;
        private readonly Action bindingChanged;

        public ContainerPropertyBindingEditor(
            IDataBindingEditorDisplayText displayText,
            Type dataSourceType,
            ContainerPropertyBinding binding,
            Action bindingChanged,
            Func<ContainerPropertyBinding, bool> showExpanded,
            Action<ContainerPropertyBinding> moveBindingUp,
            Action<ContainerPropertyBinding> moveBindingDown,
            Action<ContainerPropertyBinding> togglePropertyExpansion,
            Action<ContainerPropertyBinding> removeBinding
        )
            : base(displayText, binding)
        {
            this.bindingChanged = bindingChanged;

            if (dataSourceType == null)
            {
                return;
            }

            bindableDataSourceProperties = dataSourceType
                .GetProperties()
                .Where(
                    x => x.CanRead && typeof(IEnumerable<object>).IsAssignableFrom(x.PropertyType)
                )
                .ToArray();

            try
            {
                MakeEditorUI(
                    showExpanded,
                    moveBindingUp,
                    moveBindingDown,
                    togglePropertyExpansion,
                    removeBinding
                );
            }
            catch (Exception exception)
            {
                MakeErrorUI(exception, removeBinding);
            }
        }

        private void MakeEditorUI(
            Func<ContainerPropertyBinding, bool> showExpanded,
            Action<ContainerPropertyBinding> moveBindingUp,
            Action<ContainerPropertyBinding> moveBindingDown,
            Action<ContainerPropertyBinding> togglePropertyExpansion,
            Action<ContainerPropertyBinding> removeBinding
        )
        {
            Clear();
            ClearClassList();

            AddToClassList(DataBindingEditorStyles.bindingContainer);

            var bindingState = DetermineBindingState(Binding);
            bool isBindingComplete = bindingState == ContainerPropertyBindingState.Complete;
            var renderCondensed = isBindingComplete && !showExpanded(Binding);

            Add(
                MakeBindingHeader(
                    bindingState,
                    moveBindingUp,
                    moveBindingDown,
                    togglePropertyExpansion,
                    removeBinding,
                    renderCondensed
                )
            );

            if (!renderCondensed)
            {
                var sourcePathElement = new DropdownField(DisplayText.SourcePathText);
                sourcePathElement.AddToClassList(DataBindingEditorStyles.bindingProperty);
                sourcePathElement.choices = bindableDataSourceProperties
                    .Select(x => x.Name)
                    .ToList();
                sourcePathElement.value = Binding.SourcePath;
                sourcePathElement.RegisterValueChangedCallback(HandleSourcePathChanged);

                Add(sourcePathElement);

                var targetContainerSelectionElement = new ObjectField("Target Container");
                targetContainerSelectionElement.AddToClassList(
                    DataBindingEditorStyles.bindingProperty
                );
                targetContainerSelectionElement.allowSceneObjects = true;
                targetContainerSelectionElement.objectType = typeof(Transform);
                targetContainerSelectionElement.value = Binding.TargetContainer;
                targetContainerSelectionElement.RegisterValueChangedCallback(
                    HandleTargetContainerSelectionChanged
                );

                Add(targetContainerSelectionElement);

                var elementTemplateSelection = new ObjectField("Element Template");
                elementTemplateSelection.AddToClassList(DataBindingEditorStyles.bindingProperty);
                elementTemplateSelection.allowSceneObjects = false;
                elementTemplateSelection.objectType = typeof(View);
                elementTemplateSelection.value = Binding.ElementTemplate;
                elementTemplateSelection.RegisterValueChangedCallback(HandleElementTemplateChanged);

                Add(elementTemplateSelection);
            }
        }

        private VisualElement MakeBindingHeader(
            ContainerPropertyBindingState bindingState,
            Action<ContainerPropertyBinding> moveBindingUp,
            Action<ContainerPropertyBinding> moveBindingDown,
            Action<ContainerPropertyBinding> togglePropertyExpansion,
            Action<ContainerPropertyBinding> removeBinding,
            bool renderCondensed
        )
        {
            VisualElement headerElement = new VisualElement();
            headerElement.AddToClassList(DataBindingEditorStyles.bindingHeaderRow);

            VisualElement buttonContainer = new VisualElement();

            buttonContainer.AddToClassList(
                DataBindingEditorStyles.bindingInteractionButtonContainer
            );

            // if (renderCondensed)
            {
                buttonContainer.style.position = Position.Absolute;
                buttonContainer.style.left = StyleKeyword.Auto;
                buttonContainer.style.top = 2;
                buttonContainer.style.right = 2;
            }

            var moveBindingUpButton = new Button(
                moveBindingUp != null ? () => moveBindingUp(Binding) : null
            )
            {
                text = DisplayText.MoveUpButtonText
            };
            AddHeaderButton(buttonContainer, moveBindingUpButton);

            var moveBindingDownButton = new Button(
                moveBindingDown != null ? () => moveBindingDown(Binding) : null
            )
            {
                text = DisplayText.MoveDownButtonText
            };
            AddHeaderButton(buttonContainer, moveBindingDownButton);

            var toggleBindingExpansionButton = new Button(() => togglePropertyExpansion(Binding));
            toggleBindingExpansionButton.SetEnabled(
                bindingState == ContainerPropertyBindingState.Complete
            );

            toggleBindingExpansionButton.text = renderCondensed
                ? DisplayText.ExpandButtonText
                : DisplayText.CondenseButtonText;

            AddHeaderButton(buttonContainer, toggleBindingExpansionButton);

            var removeBindingButton = new Button(() => removeBinding(Binding)) { text = "✕" };
            AddHeaderButton(buttonContainer, removeBindingButton);

            headerElement.Add(MakeBindingStateLabel(bindingState));

            headerElement.Add(buttonContainer);

            return headerElement;
        }

        private VisualElement MakeBindingStateLabel(ContainerPropertyBindingState bindingState)
        {
            if (bindingState == ContainerPropertyBindingState.SourceUnbound)
            {
                var errorLabel = new Label(DisplayText.BindingSourceUnboundMessageText);
                errorLabel.AddToClassList(DataBindingEditorStyles.ErrorText);
                return errorLabel;
            }

            if (bindingState == ContainerPropertyBindingState.TargetUnbound)
            {
                var errorLabel = new Label(DisplayText.BindingTargetUnboundMessageText);
                errorLabel.AddToClassList(DataBindingEditorStyles.ErrorText);
                return errorLabel;
            }

            if (bindingState == ContainerPropertyBindingState.ElementTemplateMissing)
            {
                var errorLabel = new Label(DisplayText.BindingElementTemplateMissingMessageText);
                errorLabel.AddToClassList(DataBindingEditorStyles.ErrorText);
                return errorLabel;
            }

            if (bindingState == ContainerPropertyBindingState.ElementTemplateIsNotAssignable)
            {
                var errorLabel = new Label(
                    DisplayText.BindingElementTemplateIsNotAssignableMessageText
                );
                errorLabel.AddToClassList(DataBindingEditorStyles.ErrorText);
                return errorLabel;
            }

            var condensedLabel = new Label(MakeCondensedLabelText());
            condensedLabel.AddToClassList(DataBindingEditorStyles.condensedBindingLabel);

            return condensedLabel;
        }

        private string MakeCondensedLabelText()
        {
            var sourceProperty = bindableDataSourceProperties.Single(
                x => x.Name == Binding.SourcePath
            );

            var friendlySourceTypeName = sourceProperty
                .PropertyType
                .GetTypeInfo()
                .GetFriendlyName();

            return String.Format(
                DisplayText.ContainerPropertyBindingCondensedLabelFormat_Type_Source_Target_Template,
                friendlySourceTypeName,
                Binding.SourcePath,
                Binding.TargetContainer.name,
                Binding.ElementTemplate.name
            );
        }

        private ContainerPropertyBindingState DetermineBindingState(
            ContainerPropertyBinding binding
        )
        {
            var sourceProperty = bindableDataSourceProperties.FirstOrDefault(
                x => x.Name == binding.SourcePath
            );

            if (sourceProperty == null)
            {
                return ContainerPropertyBindingState.SourceUnbound;
            }

            if (Binding.TargetContainer == null)
            {
                return ContainerPropertyBindingState.TargetUnbound;
            }

            if (binding.ElementTemplate == null)
            {
                return ContainerPropertyBindingState.ElementTemplateMissing;
            }

            if (binding.ElementTemplate.GetComponent<View>() == null)
            {
                return ContainerPropertyBindingState.ElementTemplateIsNotAssignable;
            }

            return ContainerPropertyBindingState.Complete;
        }

        private void HandleSourcePathChanged(ChangeEvent<string> change)
        {
            Binding.SourcePath = change.newValue;

            MarkDirtyRepaint();
            bindingChanged();
        }

        private void HandleTargetContainerSelectionChanged(
            ChangeEvent<UnityEngine.Object> changeEvent
        )
        {
            Binding.TargetContainer = changeEvent.newValue as Transform;

            MarkDirtyRepaint();
            bindingChanged();
        }

        private void HandleElementTemplateChanged(ChangeEvent<UnityEngine.Object> changeEvent)
        {
            Binding.ElementTemplate = changeEvent.newValue as View;

            MarkDirtyRepaint();
            bindingChanged();
        }
    }
}
