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
        private readonly Type _dataSourceType;
        private readonly VisualElement _rootVisualElement;
        private readonly VisualElementPropertyBinding _binding;
        private readonly Action _bindingChanged;
        private PropertyInfo[] _bindableDataSourceProperties;

        public VisualElementPropertyBindingEditor(
            Type dataSourceType,
            VisualElement rootVisualElement,
            VisualElementPropertyBinding binding,
            Action bindingChanged,
            Func<VisualElementPropertyBinding, bool> showExpanded,
            Action<VisualElementPropertyBinding> moveBindingUp,
            Action<VisualElementPropertyBinding> moveBindingDown,
            Action<VisualElementPropertyBinding> togglePropertyExpansion,
            Action<VisualElementPropertyBinding> removeBinding )
        {
            _dataSourceType = dataSourceType;
            _rootVisualElement = rootVisualElement;
            _binding = binding;
            _bindingChanged = bindingChanged;

            if ( _dataSourceType == null )
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
                    removeBinding );
            }
            catch ( Exception e )
            {
                Clear();

                AddToClassList(DataBindingEditorStyles.invalidBindingClassName);

                Add( new Label( "Failed to setup UI for VisualElementPropertyBinding." ) );
                Add( new Label( e.Message ) );

                var removeBindingButton = new Button( () => removeBinding( binding ) );
                removeBindingButton.text = "Remove Binding";
                Add( removeBindingButton ); ;
            }
        }

        private void MakeEditorUI(
            Func<VisualElementPropertyBinding, bool> showExpanded,
            Action<VisualElementPropertyBinding> moveBindingUp,
            Action<VisualElementPropertyBinding> moveBindingDown,
            Action<VisualElementPropertyBinding> togglePropertyExpansion,
            Action<VisualElementPropertyBinding> removeBinding )
        {
            Clear();

            _bindableDataSourceProperties =
                _dataSourceType.GetProperties()
                .Where( x => x.CanRead )
                .ToArray();

            AddToClassList( DataBindingEditorStyles.bindingDefinitionClassName );

            var headerElement = new VisualElement();
            headerElement.AddToClassList( DataBindingEditorStyles.bindingDefinitionHeaderClassName );

            var isBindingValid = IsBindingValid( _binding );
            var renderCondensed = isBindingValid && !showExpanded(_binding);

            var removeBindingButton = new Button( () => removeBinding( _binding ) );
            removeBindingButton.text = "✕";
            AddHeaderButton( headerElement, removeBindingButton );

            var toggleBindingExpansionButton = new Button( () => togglePropertyExpansion( _binding ) );
            toggleBindingExpansionButton.text = renderCondensed ? "…" : "↸";
            AddHeaderButton( headerElement, toggleBindingExpansionButton );

            var moveBindingDownButton = new Button( moveBindingDown != null ? () => moveBindingDown( _binding ) : (Action)null );
            moveBindingDownButton.text = "▼";
            AddHeaderButton( headerElement, moveBindingDownButton );

            var moveBindingUpButton = new Button( moveBindingUp != null ? () => moveBindingUp( _binding ) : (Action)null );
            moveBindingUpButton.text = "▲";
            AddHeaderButton( headerElement, moveBindingUpButton );

            Add( headerElement );

            if ( renderCondensed )
            {
                var condensedLabel = new Label( MakeCondensedLabelText( _binding ) );
                condensedLabel.AddToClassList( DataBindingEditorStyles.condensedBindingLabelClassName );

                Add( condensedLabel );
            }
            else
            {
                var sourcePathElement = new DropdownField( "Source Path" );
                sourcePathElement.choices = _bindableDataSourceProperties.Select( x => x.Name ).ToList();
                sourcePathElement.value = _binding.SourcePath;
                sourcePathElement.RegisterValueChangedCallback( HandleSourcePathChanged );

                Add( sourcePathElement );

                var namedVisualElements = EnumerateRecursive( _rootVisualElement ).ToArray();
                var targetVisualElementQueryElement = new DropdownField( "Target Visual Element" );
                targetVisualElementQueryElement.choices = namedVisualElements.Select( x => x.name ).ToList();
                targetVisualElementQueryElement.value = _binding.TargetVisualElementQuery;
                targetVisualElementQueryElement.RegisterValueChangedCallback( HandleTargetVisualElementQueryChanged );

                Add( targetVisualElementQueryElement );

                var sourceProperty = _dataSourceType.GetProperties().FirstOrDefault( x => x.Name == _binding.SourcePath );
                var targetVisualElement = namedVisualElements.FirstOrDefault( x => x.name == _binding.TargetVisualElementQuery );

                if ( targetVisualElement != null )
                {
                    var bindableTargetVisualElementProperties = targetVisualElement
                        .GetType()
                        .GetProperties()
                        .Where( x => x.PropertyType.IsAssignableFrom( sourceProperty.PropertyType ) )
                        .Select( x => x.Name )
                        .ToArray();

                    var bindableStyleProperties = typeof( IStyle )
                    .GetProperties()
                    .Where( x => x.PropertyType.IsAssignableFrom( sourceProperty.PropertyType ) )
                    .Select( x => $"{nameof( targetVisualElement.style )}.{x.Name}" )
                    .ToArray();

                    var selectableTargetPaths = bindableTargetVisualElementProperties
                        .Union( bindableStyleProperties )
                        .ToList();

                    var targetPathElement = new DropdownField( "Target Path" );
                    targetPathElement.choices = selectableTargetPaths;
                    targetPathElement.value = _binding.TargetPath;
                    targetPathElement.RegisterValueChangedCallback( HandleTargetPathChanged );

                    Add( targetPathElement );
                }
            }
        }

        private void HandleTargetPathChanged( ChangeEvent<string> changeEvent )
        {
            _binding.TargetPath = changeEvent.newValue;
            _bindingChanged();
        }

        private void HandleTargetVisualElementQueryChanged( ChangeEvent<string> changeEvent )
        {
            _binding.TargetVisualElementQuery = changeEvent.newValue;
            _bindingChanged();
        }

        private void HandleSourcePathChanged( ChangeEvent<string> changeEvent )
        {
            _binding.SourcePath = changeEvent.newValue;
            _bindingChanged();
        }

        private string MakeCondensedLabelText( VisualElementPropertyBinding binding )
        {
            var sourceProperty = _bindableDataSourceProperties.Single( x => x.Name == _binding.SourcePath );
            var friendlySourceTypeName = sourceProperty.PropertyType.GetTypeInfo().GetFriendlyName();

            var targetVisualElement = _rootVisualElement.Q( _binding.TargetVisualElementQuery );
            var targetVisualElementTypeName = targetVisualElement.GetType().GetFriendlyName();


            return $"<color=blue>{friendlySourceTypeName}</color> <b>{_binding.SourcePath}</b> binds to <color=blue>{targetVisualElementTypeName}</color>::<b>{_binding.TargetPath}</b> ({_binding.TargetVisualElementQuery})";
        }

        private void AddHeaderButton( VisualElement headerElement, Button button )
        {
            button.AddToClassList( DataBindingEditorStyles.bindingActionButtonClassName );
            headerElement.Add( button );
        }

        private bool IsBindingValid( VisualElementPropertyBinding binding )
        {
            var sourceProperty = _bindableDataSourceProperties
                .FirstOrDefault( x => x.Name == binding.SourcePath );

            var targetVisualElement = _rootVisualElement?.Q( binding.TargetVisualElementQuery );
            var targetPropertyAccess = targetVisualElement.ResolvePublicPropertyPath( PathResolveOperation.SetValue, binding.TargetPath );

            return
                (sourceProperty != null)
                && !string.IsNullOrEmpty( binding.TargetVisualElementQuery )
                && (targetVisualElement != null)
                && (targetPropertyAccess.Any());
        }

        private IEnumerable<VisualElement> EnumerateRecursive( VisualElement visualElement )
        {
            if ( !string.IsNullOrEmpty( visualElement.name ) )
            {
                yield return visualElement;
            }

            foreach ( var child in visualElement.Children() )
            {
                foreach ( var childResult in EnumerateRecursive( child ) )
                {
                    yield return childResult;
                }
            }
        }
    }
}