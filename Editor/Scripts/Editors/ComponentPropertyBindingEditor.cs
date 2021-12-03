using System;
using System.Linq;
using System.Reflection;

using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.UIElements;

namespace de.JochenHeckl.Unity.DataBinding.Editor
{
	public class ComponentPropertyBindingEditor : VisualElement
	{
		private readonly VisualElement headerElement;
		private readonly DropdownField sourcePathElement;
		private readonly ObjectField targetObjectSelectionElement;
		private readonly DropdownField targetComponentSelectionElement;
		private readonly DropdownField targetPathElement;
        private readonly Button togglePropertyExpansionButton;
        private readonly Button removeBindingButton;

		public ComponentPropertyBinding _binding;

		private readonly Type _dataSourceType;
		private readonly PropertyInfo[] _bindableDataSourceProperties;
		private readonly Action _bindingChanged;

		public ComponentPropertyBindingEditor(
			Type dataSourceTypeIn,
			ComponentPropertyBinding bindingIn,
			Action bindingChangedIn,
            Func<ComponentPropertyBinding, bool> showExpanded,
            Action<ComponentPropertyBinding> moveBindingUp,
            Action<ComponentPropertyBinding> moveBindingDown,
            Action<ComponentPropertyBinding> togglePropertyExpansion,
            Action<ComponentPropertyBinding> removeBinding )
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
				.Where( x => x.CanRead )
				.ToArray();

            AddToClassList("bindingDefinition");

            headerElement = new VisualElement();
            headerElement.AddToClassList("bindingDefinitionHeader");

            var isBindingValid = IsBindingValid(_binding);
            var renderCondensed = isBindingValid && !showExpanded( _binding );

            removeBindingButton = new Button( () => removeBinding( _binding ) );
            removeBindingButton.text = "✕";
            AddHeaderButton(removeBindingButton);

            togglePropertyExpansionButton = new Button(isBindingValid ? () => togglePropertyExpansion( _binding ) : (Action) null);
            togglePropertyExpansionButton.text = renderCondensed ? "…" : "↸";
            AddHeaderButton(togglePropertyExpansionButton);

            var moveBindingDownButton = new Button(moveBindingDown != null ? () => moveBindingDown(_binding) : (Action)null);
            moveBindingDownButton.text = "▼";
            AddHeaderButton(moveBindingDownButton);

            var moveBindingUpButton = new Button(moveBindingUp != null ? () => moveBindingUp(_binding) : (Action)null);
            moveBindingUpButton.text = "▲";
            AddHeaderButton(moveBindingUpButton);

            Add(headerElement);

            if ( renderCondensed )
            {
                var condensedLabel = new Label(MakeCondensedLabelText(_binding));
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

                Add(sourcePathElement);

                targetObjectSelectionElement = new ObjectField("Target GameObject");
                targetObjectSelectionElement.allowSceneObjects = true;
                targetObjectSelectionElement.objectType = typeof(UnityEngine.GameObject);
                targetObjectSelectionElement.value = _binding.TargetGameObject;

                targetObjectSelectionElement.RegisterValueChangedCallback(HandleTargetObjectSelectionChanged);

                Add(targetObjectSelectionElement);

                targetComponentSelectionElement = new DropdownField("Target Component");
                targetComponentSelectionElement.RegisterValueChangedCallback(HandleTargetComponentChanged);

                Add(targetComponentSelectionElement);

                targetPathElement = new DropdownField("Target Path");
                targetPathElement.RegisterValueChangedCallback(HandleTargetPathChanged);

                Add(targetPathElement);

                UpdateTargetComponentChoices();
                UpdateTargetPathChoises();
            }
        }

        private void AddHeaderButton(Button button)
        {
            button.AddToClassList(DataBindingEditorStyles.bindingActionButtonClassName);
            headerElement.Add(button);
        }

        private string MakeCondensedLabelText( ComponentPropertyBinding binding )
		{
            var sourceProperty = _bindableDataSourceProperties.Single( x => x.Name == _binding.SourcePath );
            var friendlySourceTypeName = sourceProperty.PropertyType.GetTypeInfo().GetFriendlyName();

            return $"<color=blue>{friendlySourceTypeName}</color> <b>{_binding.SourcePath}</b> binds to <b>{_binding.TargetComponent.GetType().Name}.{binding.TargetPath}</b> ({_binding.TargetComponent.name})";
        }

		private bool IsBindingValid( ComponentPropertyBinding binding )
		{
            var sourceProperty = _bindableDataSourceProperties
                .FirstOrDefault( x => x.Name == binding.SourcePath );

            var targetProperty = _binding.TargetComponent?
                .GetType()
                .GetProperties()
                .FirstOrDefault( x => x.Name == binding.TargetPath );

            return
                sourceProperty != null
                && targetProperty != null
                && targetProperty.PropertyType.IsAssignableFrom( sourceProperty.PropertyType );
        }

		private void HandleSourcePathChanged( ChangeEvent<string> change )
		{
			_binding.SourcePath = change.newValue;

			MarkDirtyRepaint();
			_bindingChanged();
		}


		private void HandleTargetObjectSelectionChanged( ChangeEvent<UnityEngine.Object> changeEvent)
		{
			_binding.TargetGameObject = changeEvent.newValue as GameObject;

			UpdateTargetComponentChoices();

			MarkDirtyRepaint();
			_bindingChanged();
		}

		private void HandleTargetComponentChanged( ChangeEvent<string> changeEvent )
		{
			_binding.TargetComponent = _binding.TargetGameObject?.GetComponentsInChildren<Component>()
				.FirstOrDefault( ( x ) => MakeTargetComponentDisplayValue( x ) == changeEvent.newValue );

			UpdateTargetPathChoises();

			MarkDirtyRepaint(); 
			_bindingChanged();
		}

		private void HandleTargetPathChanged( ChangeEvent<string> changeEvent )
		{
			_binding.TargetPath = changeEvent.newValue;

			MarkDirtyRepaint(); 
			_bindingChanged();
		}

		private void UpdateTargetComponentChoices()
		{
			var componentsOfGameObject = Array.Empty<Component>();

			if ( _binding.TargetGameObject != null )
			{
				componentsOfGameObject = _binding.TargetGameObject.GetComponentsInChildren<Component>( true );

				targetComponentSelectionElement.choices = componentsOfGameObject.Select( MakeTargetComponentDisplayValue ).ToList();
				targetComponentSelectionElement.value = MakeTargetComponentDisplayValue( _binding.TargetComponent );

			}
		}

		private void UpdateTargetPathChoises()
		{
			var sourceProperty = Array.Find(
				_dataSourceType.GetProperties(),
				x => x.Name == _binding.SourcePath );

			if ( (sourceProperty != null) && (_binding.TargetComponent != null) )
			{
				var targetProperties = _binding.TargetComponent.GetType().GetProperties();
				var targetProperty = Array.Find( targetProperties, ( x ) => x.Name == _binding.TargetPath );

				targetPathElement.choices = targetProperties
					.Where( x => x.PropertyType.IsAssignableFrom( sourceProperty.PropertyType ) )
							.Select( x => x.Name )
							.ToList();

				targetPathElement.value = targetPathElement.choices.Contains( _binding.TargetPath ) ? _binding.TargetPath : null;
			}
		}

		private string MakeTargetComponentDisplayValue( Component component )
		{
			if ( component != null )
			{
				return $"{component.gameObject.name}::{component.GetType().Name}";
			}

			return null;
		}
	}
}
