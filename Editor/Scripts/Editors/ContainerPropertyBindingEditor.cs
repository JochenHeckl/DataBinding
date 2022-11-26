using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.UIElements;

namespace de.JochenHeckl.Unity.DataBinding.Editor
{
    internal class ContainerPropertyBindingEditor : BindingEditor<ContainerPropertyBinding>
    {
        private readonly Type dataSourceType;
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
        ) : base(displayText, binding)
        {
            this.dataSourceType = dataSourceType;
            this.bindingChanged = bindingChanged;

            bindableDataSourceProperties = dataSourceType
                .GetProperties()
                .Where(
                    x => x.CanRead && typeof(IEnumerable<object>).IsAssignableFrom(x.PropertyType)
                )
                .ToArray();

            if (dataSourceType == null)
            {
                return;
            }

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
                sourcePathElement.choices = bindableDataSourceProperties
                    .Select(x => x.Name)
                    .ToList();
                sourcePathElement.value = Binding.SourcePath;
                sourcePathElement.RegisterValueChangedCallback(HandleSourcePathChanged);

                Add(sourcePathElement);

                var targetContainerSelectionElement = new ObjectField("Target Container");
                targetContainerSelectionElement.allowSceneObjects = true;
                targetContainerSelectionElement.objectType = typeof(Transform);
                targetContainerSelectionElement.value = Binding.TargetContainer;
                targetContainerSelectionElement.RegisterValueChangedCallback(
                    HandleTargetContainerSelectionChanged
                );

                Add(targetContainerSelectionElement);

                var elementTemplateSelection = new ObjectField("Element Template");
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

            var moveBindingUpButton = new Button(
                moveBindingUp != null ? () => moveBindingUp(Binding) : (Action)null
            );
            moveBindingUpButton.text = DisplayText.MoveUpButtonText;
            AddHeaderButton(buttonContainer, moveBindingUpButton);

            var moveBindingDownButton = new Button(
                moveBindingDown != null ? () => moveBindingDown(Binding) : (Action)null
            );
            moveBindingDownButton.text = DisplayText.MoveDownButtonText;
            AddHeaderButton(buttonContainer, moveBindingDownButton);

            var toggleBindingExpansionButton = new Button(() => togglePropertyExpansion(Binding));
            toggleBindingExpansionButton.SetEnabled(
                bindingState == ContainerPropertyBindingState.Complete
            );

            toggleBindingExpansionButton.text = renderCondensed
                ? DisplayText.ExpandButtonText
                : DisplayText.CondenseButtonText;

            AddHeaderButton(buttonContainer, toggleBindingExpansionButton);

            var removeBindingButton = new Button(() => removeBinding(Binding));
            removeBindingButton.text = "✕";
            AddHeaderButton(buttonContainer, removeBindingButton);

            headerElement.Add(buttonContainer);

            headerElement.Add(MakeBindingStateLabel(bindingState));

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

            if (bindingState == ContainerPropertyBindingState.ElementTemplateMissing)
            {
                var errorLabel = new Label(DisplayText.BindingElementTemplateMissingMessageText);
                errorLabel.AddToClassList(DataBindingEditorStyles.ErrorText);
                return errorLabel;
            }

            var condensedLabel = new Label(MakeCondensedLabelText(Binding));
            condensedLabel.AddToClassList(DataBindingEditorStyles.condensedBindingLabel);

            return condensedLabel;
        }

        private string MakeCondensedLabelText(ContainerPropertyBinding binding)
        {
            var sourceProperty = bindableDataSourceProperties.Single(
                x => x.Name == Binding.SourcePath
            );

            var friendlySourceTypeName = sourceProperty.PropertyType
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
