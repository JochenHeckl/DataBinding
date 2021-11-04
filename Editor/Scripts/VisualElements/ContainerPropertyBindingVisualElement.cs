using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.UIElements;

namespace de.JochenHeckl.Unity.DataBinding.Editor
{
	public class ContainerPropertyBindingVisualElement : VisualElement
	{
		private readonly VisualElement headerElement;
		private readonly DropdownField sourcePathElement;
		private readonly ObjectField targetContainerSelectionElement;
		private readonly ObjectField elementTemplateSelection;
        private readonly Button togglePropertyExpansionButton;
        private readonly Button removeBindingButton;

		public ContainerPropertyBinding _binding;

		private readonly Type _dataSourceType;
		private readonly PropertyInfo[] _bindableDataSourceProperties;
		private readonly Action _bindingChanged;

		public ContainerPropertyBindingVisualElement(
			Type dataSourceTypeIn,
			ContainerPropertyBinding bindingIn,
			Action bindingChangedIn,
            bool forcedShowExpanded,
            Action<ContainerPropertyBinding> moveBindingUp,
            Action<ContainerPropertyBinding> moveBindingDown,
            Action togglePropertyExpansion,
            Action removeBindingIn )
		{
            _dataSourceType = dataSourceTypeIn;
            _binding = bindingIn;
            _bindingChanged = bindingChangedIn;

            if ( _dataSourceType == null )
            {
                return;
            }

            _bindableDataSourceProperties =
                _dataSourceType.GetProperties()
                .Where(x => 
                    x.CanRead
                    && typeof(IEnumerable<object>).IsAssignableFrom(x.PropertyType))
                .ToArray();

            AddToClassList("bindingDefinition");

            headerElement = new VisualElement();
            headerElement.AddToClassList("bindingDefinitionHeader");

            var isBindingValid = IsBindingValid( _binding );
            var renderCondensed = isBindingValid && !forcedShowExpanded;

            removeBindingButton = new Button(removeBindingIn);
            removeBindingButton.text = "✕";

            AddHeaderButton(removeBindingButton);            

            togglePropertyExpansionButton = new Button(isBindingValid ? togglePropertyExpansion : null);
            togglePropertyExpansionButton.text = renderCondensed ? "…" : "↸";
            
            AddHeaderButton(togglePropertyExpansionButton);

            var moveBindingDownButton = new Button(moveBindingDown != null ? () => moveBindingDown(_binding) : (Action)null);
            moveBindingDownButton.text = "▼";
            
            AddHeaderButton(moveBindingDownButton);

            var moveBindingUpButton = new Button(moveBindingUp != null ? () => moveBindingUp(_binding) : (Action)null);
            moveBindingUpButton.text = "▲";

            AddHeaderButton(moveBindingUpButton);

            Add( headerElement );

            if ( renderCondensed )
            {
                var condensedLabel = new Label(MakeCondensedLabel(_binding));
                condensedLabel.AddToClassList("unity-text-element");
                condensedLabel.AddToClassList("unity-label");
                condensedLabel.AddToClassList("condensedBindingLabel");

                Add(condensedLabel);
            }
            else
            {
                sourcePathElement = new DropdownField("Source Path");
                sourcePathElement.choices = _bindableDataSourceProperties.Select(x => x.Name).ToList();
                sourcePathElement.value = _binding.SourcePath;
                sourcePathElement.RegisterValueChangedCallback(HandleSourcePathChanged);

                Add( sourcePathElement );

                targetContainerSelectionElement = new ObjectField("Target Container");
                targetContainerSelectionElement.allowSceneObjects = true;
                targetContainerSelectionElement.objectType = typeof(Transform);
                targetContainerSelectionElement.value = _binding.TargetContainer;
                targetContainerSelectionElement.RegisterValueChangedCallback( HandleTargetContainerSelectionChanged );

                Add( targetContainerSelectionElement );

                elementTemplateSelection = new ObjectField("Element Template");
                elementTemplateSelection.allowSceneObjects = false;
                elementTemplateSelection.objectType = typeof(View);
                elementTemplateSelection.value = _binding.ElementTemplate;
                elementTemplateSelection.RegisterValueChangedCallback(HandleElementTemplateChanged);

                Add(elementTemplateSelection);
            }
        }

        private void AddHeaderButton(Button button)
        {
            button.AddToClassList("bindingActionButton");
            headerElement.Add(button);
        }

        private string MakeCondensedLabel(ContainerPropertyBinding binding)
        {
            var sourceProperty = _bindableDataSourceProperties.Single(x => x.Name == _binding.SourcePath);

            var friendlySourceTypeName = sourceProperty.PropertyType.GetTypeInfo().GetFriendlyName();

            return $"<color=blue>{friendlySourceTypeName}</color> <b>{_binding.SourcePath}</b> expands to <b>{_binding.TargetContainer.name}</b> ({_binding.ElementTemplate.name})";
        }

        private bool IsBindingValid(ContainerPropertyBinding binding)
        {
            var sourceProperty = _bindableDataSourceProperties
                .FirstOrDefault(x => x.Name == binding.SourcePath);

            return
                sourceProperty != null
                && binding.TargetContainer != null
                && binding.ElementTemplate != null;
        }

        private void HandleSourcePathChanged(ChangeEvent<string> change)
        {
            _binding.SourcePath = change.newValue;

            MarkDirtyRepaint();
            _bindingChanged();
        }


        private void HandleTargetContainerSelectionChanged(ChangeEvent<UnityEngine.Object> changeEvent)
        {
            _binding.TargetContainer = changeEvent.newValue as Transform;

            MarkDirtyRepaint();
            _bindingChanged();
        }

        private void HandleElementTemplateChanged(ChangeEvent<UnityEngine.Object> changeEvent)
        {
            _binding.ElementTemplate = changeEvent.newValue as View;

            MarkDirtyRepaint();
            _bindingChanged();
        }
    }
}
