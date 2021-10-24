using System;
using System.Linq;
using System.Reflection;

using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.UIElements;

namespace de.JochenHeckl.Unity.DataBinding.Editor
{
	public class ComponentPropertyBindingVisualElement : VisualElement
	{
		private VisualElement headerElement;
		private DropdownField sourcePathElement;
		private ObjectField targetObjectSelectionElement;
		private DropdownField targetComponentSelectionElement;
		private DropdownField targetPathElement;
		private Button removeBindingButton;

		public ComponentPropertyBinding _binding;

		private Type _dataSourceType;
		private PropertyInfo[] _bindableDataSourceProperties;
		private Action _bindingChanged;

		public ComponentPropertyBindingVisualElement(
			Type dataSourceTypeIn,
			ComponentPropertyBinding bindingIn,
			Action bindingChangedIn,
			Action removeBindingIn )
		{
			_dataSourceType = dataSourceTypeIn;
			_binding = bindingIn;
			_bindingChanged = bindingChangedIn;

			_bindableDataSourceProperties =
				_dataSourceType.GetProperties()
				.Where( x => x.CanRead )
				.ToArray();

			AddToClassList( "componentPropertyBinding" );

			headerElement = new VisualElement();
			headerElement.AddToClassList( "componentPropertyBindingHeader" );

			removeBindingButton = new Button( removeBindingIn );
			removeBindingButton.text = "✕";
			headerElement.Add( removeBindingButton );

			Add( headerElement );

			sourcePathElement = new DropdownField( "Source Path" );
			sourcePathElement.choices = _bindableDataSourceProperties.Select( x => x.Name ).ToList();
			sourcePathElement.value = _binding.SourcePath;
			sourcePathElement.RegisterValueChangedCallback( HandleSourcePathChanged );

			Add( sourcePathElement );

			targetObjectSelectionElement = new ObjectField( "Target GameObject" );
			targetObjectSelectionElement.allowSceneObjects = true;
			targetObjectSelectionElement.objectType = typeof( UnityEngine.GameObject );
			targetObjectSelectionElement.value = _binding.TargetGameObject;

			targetObjectSelectionElement.RegisterValueChangedCallback( HandleTargetObjectSelectionChanged );

			Add( targetObjectSelectionElement );

			targetComponentSelectionElement = new DropdownField( "Target Component" );
			targetComponentSelectionElement.RegisterValueChangedCallback( HandleTargetComponentChanged );

			Add( targetComponentSelectionElement );

			targetPathElement = new DropdownField( "Target Path" );
			targetPathElement.RegisterValueChangedCallback( HandleTargetPathChanged );

			Add( targetPathElement );

			UpdateTargetComponentChoices();
			UpdateTargetPathChoises();
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
