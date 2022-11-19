using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace de.JochenHeckl.Unity.DataBinding.Editor
{
    internal class VisualElementPropertyBindingEditor : VisualElement
    {
        private readonly IDataBindingEditorDisplayText displayText;
        private readonly Type dataSourceType;
        private readonly VisualElement rootVisualElement;
        private readonly VisualElementPropertyBinding binding;
        private readonly Action bindingChanged;
        private PropertyInfo[] bindableDataSourceProperties;

        public VisualElementPropertyBindingEditor(
            IDataBindingEditorDisplayText displayText,
            Type dataSourceType,
            VisualElement rootVisualElement,
            VisualElementPropertyBinding binding,
            Action bindingChanged,
            Func<VisualElementPropertyBinding, bool> showExpanded,
            Action<VisualElementPropertyBinding> moveBindingUp,
            Action<VisualElementPropertyBinding> moveBindingDown,
            Action<VisualElementPropertyBinding> togglePropertyExpansion,
            Action<VisualElementPropertyBinding> removeBinding
        )
        {
            this.displayText = displayText;
            this.dataSourceType = dataSourceType;
            this.rootVisualElement = rootVisualElement;
            this.binding = binding;
            this.bindingChanged = bindingChanged;

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
            Func<VisualElementPropertyBinding, bool> showExpanded,
            Action<VisualElementPropertyBinding> moveBindingUp,
            Action<VisualElementPropertyBinding> moveBindingDown,
            Action<VisualElementPropertyBinding> togglePropertyExpansion,
            Action<VisualElementPropertyBinding> removeBinding
        )
        {
            Clear();
            ClearClassList();

            AddToClassList(DataBindingEditorStyles.bindingContainer);

            var bindingState = TestBindingState(binding);
            var isBindingComplete = bindingState == VisualElementPropertyBindingState.Complete;

            bool renderCondensed = isBindingComplete && !showExpanded(binding);

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

            if (renderCondensed)
            {
                var condensedLabel = new Label(MakeCondensedLabelText(binding));
                condensedLabel.AddToClassList(DataBindingEditorStyles.condensedBindingLabel);

                Add(condensedLabel);
            }
            else
            {
                var sourcePathElement = new DropdownField(displayText.SourcePathText);
                sourcePathElement.choices = bindableDataSourceProperties
                    .Select(x => x.Name)
                    .ToList();
                sourcePathElement.value = binding.SourcePath;
                sourcePathElement.RegisterValueChangedCallback(HandleSourcePathChanged);

                Add(sourcePathElement);

                var namedVisualElements = EnumerateRecursive(rootVisualElement).ToArray();
                var targetVisualElementQueryElement = new DropdownField("Target Visual Element");
                targetVisualElementQueryElement.choices = namedVisualElements
                    .Select(x => x.name)
                    .ToList();
                targetVisualElementQueryElement.value = binding.TargetVisualElementQuery;
                targetVisualElementQueryElement.RegisterValueChangedCallback(
                    HandleTargetVisualElementQueryChanged
                );

                Add(targetVisualElementQueryElement);

                var sourceProperty = dataSourceType
                    .GetProperties()
                    .FirstOrDefault(x => x.Name == binding.SourcePath);
                var targetVisualElement = namedVisualElements.FirstOrDefault(
                    x => x.name == binding.TargetVisualElementQuery
                );

                if (targetVisualElement != null)
                {
                    var bindableTargetVisualElementProperties = targetVisualElement
                        .GetType()
                        .GetProperties()
                        .Where(x => x.PropertyType.IsAssignableFrom(sourceProperty.PropertyType))
                        .Select(x => x.Name)
                        .ToArray();

                    var bindableStyleProperties = typeof(IStyle)
                        .GetProperties()
                        .Where(x => x.PropertyType.IsAssignableFrom(sourceProperty.PropertyType))
                        .Select(x => $"{nameof(targetVisualElement.style)}.{x.Name}")
                        .ToArray();

                    var selectableTargetPaths = bindableTargetVisualElementProperties
                        .Union(bindableStyleProperties)
                        .ToList();

                    var targetPathElement = new DropdownField("Target Path");
                    targetPathElement.choices = selectableTargetPaths;
                    targetPathElement.value = binding.TargetPath;
                    targetPathElement.RegisterValueChangedCallback(HandleTargetPathChanged);

                    Add(targetPathElement);
                }
            }
        }

        private void MakeErrorUI(
            Exception exception,
            Action<VisualElementPropertyBinding> removeBinding
        )
        {
            Clear();
            ClearClassList();

            AddToClassList(DataBindingEditorStyles.invalidBindingClassName);

            Add(new Label("Failed to setup UI for VisualElementPropertyBinding."));
            Add(new Label(exception.Message));

            var removeBindingButton = new Button(() => removeBinding(binding));
            removeBindingButton.text = "Remove Binding";
            Add(removeBindingButton);
        }

        private VisualElement MakeBindingHeader(
            VisualElementPropertyBindingState bindingState,
            Action<VisualElementPropertyBinding> moveBindingUp,
            Action<VisualElementPropertyBinding> moveBindingDown,
            Action<VisualElementPropertyBinding> togglePropertyExpansion,
            Action<VisualElementPropertyBinding> removeBinding,
            bool renderCondensed
        )
        {
            VisualElement headerElement = new VisualElement();
            headerElement.AddToClassList(DataBindingEditorStyles.bindingHeaderRow);

            headerElement.Add(MakeBindingStateLabel(bindingState));

            VisualElement buttonContainer = new VisualElement();

            bindableDataSourceProperties = dataSourceType
                .GetProperties()
                .Where(x => x.CanRead)
                .ToArray();

            buttonContainer.AddToClassList(
                DataBindingEditorStyles.bindingInteractionButtonContainer
            );

            var moveBindingUpButton = new Button(
                moveBindingUp != null ? () => moveBindingUp(binding) : (Action)null
            );
            moveBindingUpButton.text = "▲";
            AddHeaderButton(buttonContainer, moveBindingUpButton);

            var moveBindingDownButton = new Button(
                moveBindingDown != null ? () => moveBindingDown(binding) : (Action)null
            );
            moveBindingDownButton.text = "▼";
            AddHeaderButton(buttonContainer, moveBindingDownButton);

            var toggleBindingExpansionButton = new Button(() => togglePropertyExpansion(binding));
            toggleBindingExpansionButton.SetEnabled(
                bindingState == VisualElementPropertyBindingState.Complete
            );

            toggleBindingExpansionButton.text = renderCondensed ? "…" : "↸";
            AddHeaderButton(buttonContainer, toggleBindingExpansionButton);

            var removeBindingButton = new Button(() => removeBinding(binding));
            removeBindingButton.text = "✕";
            AddHeaderButton(buttonContainer, removeBindingButton);

            headerElement.Add(buttonContainer);

            return headerElement;
        }

        private VisualElement MakeBindingStateLabel(VisualElementPropertyBindingState bindingState)
        {
            if (bindingState == VisualElementPropertyBindingState.SourceUnbound)
            {
                var errorLabel = new Label("Select the source path");
                errorLabel.AddToClassList(DataBindingEditorStyles.ErrorText);
                return errorLabel;
            }

            var label = new Label("✓");
            label.AddToClassList(DataBindingEditorStyles.SuccessText);
            return label;
        }

        private void HandleSourcePathChanged(ChangeEvent<string> changeEvent)
        {
            binding.SourcePath = changeEvent.newValue;
            bindingChanged();
        }

        private void HandleTargetPathChanged(ChangeEvent<string> changeEvent)
        {
            binding.TargetPath = changeEvent.newValue;
            bindingChanged();
        }

        private void HandleTargetVisualElementQueryChanged(ChangeEvent<string> changeEvent)
        {
            binding.TargetVisualElementQuery = changeEvent.newValue;
            bindingChanged();
        }

        private string MakeCondensedLabelText(VisualElementPropertyBinding binding)
        {
            var sourceProperty = bindableDataSourceProperties.Single(
                x => x.Name == this.binding.SourcePath
            );
            var friendlySourceTypeName = sourceProperty.PropertyType
                .GetTypeInfo()
                .GetFriendlyName();

            var targetVisualElement = rootVisualElement.Q(this.binding.TargetVisualElementQuery);
            var targetVisualElementTypeName = targetVisualElement.GetType().GetFriendlyName();

            return $"<color=blue>{friendlySourceTypeName}</color> <b>{this.binding.SourcePath}</b> binds to <color=blue>{targetVisualElementTypeName}</color>::<b>{this.binding.TargetPath}</b> ({this.binding.TargetVisualElementQuery})";
        }

        private void AddHeaderButton(VisualElement headerElement, Button button)
        {
            button.AddToClassList(DataBindingEditorStyles.bindingActionButton);
            headerElement.Add(button);
        }

        private VisualElementPropertyBindingState TestBindingState(
            VisualElementPropertyBinding binding
        )
        {
            if (rootVisualElement == null)
            {
                return VisualElementPropertyBindingState.RootVisualElementUnboud;
            }

            var sourceProperty = bindableDataSourceProperties?.FirstOrDefault(
                x => x.Name == binding.SourcePath
            );

            if (sourceProperty == null)
            {
                return VisualElementPropertyBindingState.SourceUnbound;
            }

            if (string.IsNullOrEmpty(binding.TargetVisualElementQuery))
            {
                return VisualElementPropertyBindingState.TargetElementUnbound;
            }

            var targetVisualElement = rootVisualElement.Q(binding.TargetVisualElementQuery);

            if (targetVisualElement == null)
            {
                return VisualElementPropertyBindingState.TargetElementUnbound;
            }

            var targetPropertyAccess = targetVisualElement.ResolvePublicPropertyPath(
                PathResolveOperation.SetValue,
                binding.TargetPath
            );

            if (!targetPropertyAccess.Any())
            {
                return VisualElementPropertyBindingState.TargetPropertyUnbound;
            }

            return VisualElementPropertyBindingState.Complete;
        }

        private IEnumerable<VisualElement> EnumerateRecursive(VisualElement visualElement)
        {
            if (!string.IsNullOrEmpty(visualElement.name))
            {
                yield return visualElement;
            }

            foreach (var child in visualElement.Children())
            {
                foreach (var childResult in EnumerateRecursive(child))
                {
                    yield return childResult;
                }
            }
        }
    }
}
