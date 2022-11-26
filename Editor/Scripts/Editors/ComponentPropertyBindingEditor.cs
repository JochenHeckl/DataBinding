using System;
using System.Linq;
using System.Numerics;
using System.Reflection;

using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.UIElements;

namespace de.JochenHeckl.Unity.DataBinding.Editor
{
    internal class ComponentPropertyBindingEditor : BindingEditor<ComponentPropertyBinding>
    {
        private DropdownField targetComponentSelectionDropdownField;

        private readonly IDataBindingEditorDisplayText displayText;
        private readonly Type dataSourceType;
        private readonly PropertyInfo[] bindableDataSourceProperties;
        private readonly Action bindingChanged;

        public ComponentPropertyBindingEditor(
            IDataBindingEditorDisplayText displayText,
            Type dataSourceType,
            ComponentPropertyBinding binding,
            Action bindingChanged,
            Func<ComponentPropertyBinding, bool> showExpanded,
            Action<ComponentPropertyBinding> moveBindingUp,
            Action<ComponentPropertyBinding> moveBindingDown,
            Action<ComponentPropertyBinding> togglePropertyExpansion,
            Action<ComponentPropertyBinding> removeBinding
        ) : base(binding)
        {
            this.displayText = displayText;
            this.dataSourceType = dataSourceType;
            this.bindingChanged = bindingChanged;

            bindableDataSourceProperties = dataSourceType
                .GetProperties()
                .Where(x => x.CanRead)
                .ToArray();

            if (this.dataSourceType == null)
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
            Func<ComponentPropertyBinding, bool> showExpanded,
            Action<ComponentPropertyBinding> moveBindingUp,
            Action<ComponentPropertyBinding> moveBindingDown,
            Action<ComponentPropertyBinding> togglePropertyExpansion,
            Action<ComponentPropertyBinding> removeBinding
        )
        {
            Clear();
            ClearClassList();

            AddToClassList(DataBindingEditorStyles.bindingContainer);

            var bindingState = DetermineBindingState(Binding);
            bool isBindingComplete = bindingState == ComponentPropertyBindingState.Complete;
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
                var sourcePathElement = new DropdownField(displayText.SourcePathText);
                sourcePathElement.choices = bindableDataSourceProperties
                    .Select(x => x.Name)
                    .ToList();
                sourcePathElement.value = Binding.SourcePath;
                sourcePathElement.RegisterValueChangedCallback(HandleSourcePathChanged);

                Add(sourcePathElement);

                var targetObjectSelectionElement = new ObjectField(
                    displayText.TargetGameObjectText
                );
                targetObjectSelectionElement.allowSceneObjects = true;
                targetObjectSelectionElement.objectType = typeof(UnityEngine.GameObject);
                targetObjectSelectionElement.value = Binding.TargetGameObject;

                targetObjectSelectionElement.RegisterValueChangedCallback(
                    HandleTargetObjectSelectionChanged
                );

                Add(targetObjectSelectionElement);

                targetComponentSelectionDropdownField = new DropdownField(
                    displayText.TargetComponentText
                );
                targetComponentSelectionDropdownField.RegisterValueChangedCallback(
                    HandleTargetComponentChanged
                );

                Add(targetComponentSelectionDropdownField);

                var targetPathElement = new DropdownField("Target Path");
                targetPathElement.RegisterValueChangedCallback(HandleTargetPathChanged);

                Add(targetPathElement);

                UpdateTargetComponentChoices();
                UpdateTargetPathChoises(targetPathElement);
            }
        }

        private VisualElement MakeBindingHeader(
            ComponentPropertyBindingState bindingState,
            Action<ComponentPropertyBinding> moveBindingUp,
            Action<ComponentPropertyBinding> moveBindingDown,
            Action<ComponentPropertyBinding> togglePropertyExpansion,
            Action<ComponentPropertyBinding> removeBinding,
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
            moveBindingUpButton.text = "▲";
            AddHeaderButton(buttonContainer, moveBindingUpButton);

            var moveBindingDownButton = new Button(
                moveBindingDown != null ? () => moveBindingDown(Binding) : (Action)null
            );
            moveBindingDownButton.text = "▼";
            AddHeaderButton(buttonContainer, moveBindingDownButton);

            var toggleBindingExpansionButton = new Button(() => togglePropertyExpansion(Binding));
            toggleBindingExpansionButton.SetEnabled(
                bindingState == ComponentPropertyBindingState.Complete
            );

            toggleBindingExpansionButton.text = renderCondensed ? "…" : "↸";
            AddHeaderButton(buttonContainer, toggleBindingExpansionButton);

            var removeBindingButton = new Button(() => removeBinding(Binding));
            removeBindingButton.text = "✕";
            AddHeaderButton(buttonContainer, removeBindingButton);

            headerElement.Add(buttonContainer);

            headerElement.Add(MakeBindingStateLabel(bindingState));

            return headerElement;
        }

        private void AddHeaderButton(VisualElement headerElement, Button button)
        {
            button.AddToClassList(DataBindingEditorStyles.bindingActionButton);
            headerElement.Add(button);
        }

        private VisualElement MakeBindingStateLabel(ComponentPropertyBindingState bindingState)
        {
            if (bindingState == ComponentPropertyBindingState.SourceUnbound)
            {
                var errorLabel = new Label(displayText.BindingSourceUnboundMessageText);
                errorLabel.AddToClassList(DataBindingEditorStyles.ErrorText);
                return errorLabel;
            }

            if (bindingState == ComponentPropertyBindingState.TargetUnbound)
            {
                var errorLabel = new Label(displayText.BindingTargetUnboundMessageText);
                errorLabel.AddToClassList(DataBindingEditorStyles.ErrorText);
                return errorLabel;
            }

            if (bindingState == ComponentPropertyBindingState.TargetUnbound)
            {
                var errorLabel = new Label(displayText.BindingUnassignableMessageText);
                errorLabel.AddToClassList(DataBindingEditorStyles.ErrorText);
                return errorLabel;
            }

            var condensedLabel = new Label(MakeCondensedLabelText(Binding));
            condensedLabel.AddToClassList(DataBindingEditorStyles.condensedBindingLabel);

            return condensedLabel;
        }

        private string MakeCondensedLabelText(ComponentPropertyBinding binding)
        {
            var sourceProperty = bindableDataSourceProperties.Single(
                x => x.Name == Binding.SourcePath
            );
            var friendlySourceTypeName = sourceProperty.PropertyType
                .GetTypeInfo()
                .GetFriendlyName();

            return String.Format(
                displayText.ComponentPropertyBindingCondensedLabelFormat_Type_Source_Target_Component,
                friendlySourceTypeName,
                Binding.SourcePath,
                $"{Binding.TargetComponent.GetType().Name}.{binding.TargetPath}",
                Binding.TargetComponent.name
            );
        }

        private ComponentPropertyBindingState DetermineBindingState(
            ComponentPropertyBinding binding
        )
        {
            var sourceProperty = bindableDataSourceProperties.FirstOrDefault(
                x => x.Name == binding.SourcePath
            );

            if (sourceProperty == null)
            {
                return ComponentPropertyBindingState.SourceUnbound;
            }

            var targetProperty = Binding.TargetComponent
                ?.GetType()
                .GetProperties()
                .FirstOrDefault(x => x.Name == binding.TargetPath);

            if (targetProperty == null)
            {
                return ComponentPropertyBindingState.TargetUnbound;
            }

            if (!targetProperty.PropertyType.IsAssignableFrom(sourceProperty.PropertyType))
            {
                return ComponentPropertyBindingState.Unassignable;
            }

            return ComponentPropertyBindingState.Complete;
        }

        private void HandleSourcePathChanged(ChangeEvent<string> change)
        {
            Binding.SourcePath = change.newValue;

            MarkDirtyRepaint();
            bindingChanged();
        }

        private void HandleTargetObjectSelectionChanged(ChangeEvent<UnityEngine.Object> changeEvent)
        {
            Binding.TargetGameObject = changeEvent.newValue as GameObject;

            UpdateTargetComponentChoices();

            MarkDirtyRepaint();
            bindingChanged();
        }

        private void HandleTargetComponentChanged(ChangeEvent<string> changeEvent)
        {
            Binding.TargetComponent = Binding.TargetGameObject
                ?.GetComponentsInChildren<Component>()
                .FirstOrDefault((x) => MakeTargetComponentDisplayValue(x) == changeEvent.newValue);

            UpdateTargetPathChoises(targetComponentSelectionDropdownField);

            MarkDirtyRepaint();
            bindingChanged();
        }

        private void HandleTargetPathChanged(ChangeEvent<string> changeEvent)
        {
            Binding.TargetPath = changeEvent.newValue;

            MarkDirtyRepaint();
            bindingChanged();
        }

        private void UpdateTargetComponentChoices()
        {
            var componentsOfGameObject = Array.Empty<Component>();

            if (Binding.TargetGameObject != null)
            {
                componentsOfGameObject =
                    Binding.TargetGameObject.GetComponentsInChildren<Component>(true);

                targetComponentSelectionDropdownField.choices = componentsOfGameObject
                    .Select(MakeTargetComponentDisplayValue)
                    .ToList();
                targetComponentSelectionDropdownField.value = MakeTargetComponentDisplayValue(
                    Binding.TargetComponent
                );
            }
        }

        private void UpdateTargetPathChoises(DropdownField targetPathDropdownField)
        {
            var sourceProperty = Array.Find(
                dataSourceType.GetProperties(),
                x => x.Name == Binding.SourcePath
            );

            if ((sourceProperty != null) && (Binding.TargetComponent != null))
            {
                var targetProperties = Binding.TargetComponent.GetType().GetProperties();
                var targetProperty = Array.Find(
                    targetProperties,
                    (x) => x.Name == Binding.TargetPath
                );

                targetPathDropdownField.choices = targetProperties
                    .Where(x => x.PropertyType.IsAssignableFrom(sourceProperty.PropertyType))
                    .Select(x => x.Name)
                    .ToList();

                targetPathDropdownField.value = targetPathDropdownField.choices.Contains(
                    Binding.TargetPath
                )
                    ? Binding.TargetPath
                    : null;
            }
        }

        private string MakeTargetComponentDisplayValue(Component component)
        {
            if (component != null)
            {
                return $"{component.gameObject.name}::{component.GetType().Name}";
            }

            return null;
        }
    }
}
