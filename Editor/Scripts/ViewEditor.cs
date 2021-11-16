using System;
using System.Linq;

using UnityEditor;

using UnityEngine.UIElements;

namespace de.JochenHeckl.Unity.DataBinding.Editor
{
    [CustomEditor( typeof( View ), true )]
    public class ViewEditor : UnityEditor.Editor
    {
        private View _view;
        private static Type[] _validDataSources;        
        private VisualElement _editorRootElement;

        public void OnEnable()
        {
            _view = target as View;

            if ( _validDataSources == null )
            {
                _validDataSources = ViewEditorCommon.GetValidDataSourceTypes();
            }
        }

        public override VisualElement CreateInspectorGUI()
        {
            _editorRootElement = new VisualElement();

            MakeEditorView();

            return _editorRootElement;
        }

        private void MakeEditorView()
        {
            _editorRootElement.Clear();
            _editorRootElement.styleSheets.Add( DataBindingEditorStyles.StyleSheet );
            _editorRootElement.AddToClassList( DataBindingEditorStyles.viewEditorClassName );

            var dataSourceTypeDropDownField = new DropdownField(
               label: "DataSource Type",
               choices: _validDataSources.Select( x => x.GetFriendlyName() ).ToList(),
               defaultValue: _validDataSources.FirstOrDefault( x => x == _view.dataSourceType.Type )?.Name ?? "" );

            dataSourceTypeDropDownField.AddToClassList( DataBindingEditorStyles.bindingDataSourceTypeLabelClassName );
            dataSourceTypeDropDownField.RegisterValueChangedCallback( HandleDataSourceTypeChanged );
            _editorRootElement.Add( dataSourceTypeDropDownField );

            _editorRootElement.Add( MakeComponentPropertyBindings() );
            _editorRootElement.Add( MakeComponentPropertyBindings() );
        }

        private VisualElement MakeComponentPropertyBindings()
        {
            return ViewEditorCommon.MakeBindingSection(
                "Component Property Bindings",
                HandleAddComponentPropertyBinding,
                _view.componentPropertyBindings,
                MakeComponentPropertyBindingVisualElement );
        }

        private void HandleAddComponentPropertyBinding()
        {
            _view.componentPropertyBindings = _view.componentPropertyBindings
                .Append( new ComponentPropertyBinding() { DataSource = _view.DataSource } )
                .ToArray();

            StoreAndUpdateView();
        }

        private VisualElement MakeComponentPropertyBindingVisualElement( ComponentPropertyBinding binding )
        {
            return new ComponentPropertyBindingVisualElement(
                _view.dataSourceType.Type,
                binding,
                StoreAndUpdateView,
                binding => binding.showExpanded,
                HandleMoveBindingUp,
                HandleMoveBindingDown,
                HandleTogglePropertyExpansion,
                HandleRemoveBinding );
        }

        private void HandleMoveBindingUp( ComponentPropertyBinding binding )
        {
            ViewEditorCommon.MoveElementUp( _view.componentPropertyBindings, binding );

            StoreAndUpdateView();
        }

        private void HandleMoveBindingDown( ComponentPropertyBinding binding )
        {
            ViewEditorCommon.MoveElementDown( _view.componentPropertyBindings, binding );

            StoreAndUpdateView();
        }

        private void HandleTogglePropertyExpansion( ComponentPropertyBinding binding )
        {
            binding.showExpanded = !binding.showExpanded;
            StoreAndUpdateView();
        }
        private void HandleRemoveBinding( ComponentPropertyBinding binding )
        {
            _view.componentPropertyBindings =
                _view.componentPropertyBindings.Where( x => x != binding )
                .ToArray();

            StoreAndUpdateView();
        }

        private VisualElement MakeContainerPropertyBindings()
        {
            return ViewEditorCommon.MakeBindingSection(
                "Container Property Bindings",
                HandleAddContainerPropertyBinding,
                _view.containerPropertyBindings,
                MakeContainerPropertyBindingVisualElement);
        }

        private void HandleAddContainerPropertyBinding()
        {
            _view.containerPropertyBindings = _view.containerPropertyBindings
                .Append( new ContainerPropertyBinding() { DataSource = _view.DataSource } )
                .ToArray();

            StoreAndUpdateView();
        }

        private VisualElement MakeContainerPropertyBindingVisualElement( ContainerPropertyBinding binding )
        {
            return new ContainerPropertyBindingVisualElement(
                _view.dataSourceType.Type,
                binding,
                StoreAndUpdateView,
                binding => binding.showExpanded,
                HandleMoveBindingUp,
                HandleMoveBindingDown,
                HandleTogglePropertyExpansion,
                HandleRemoveBinding );
        }

        private void HandleMoveBindingUp( ContainerPropertyBinding binding )
        {
            ViewEditorCommon.MoveElementUp( _view.containerPropertyBindings, binding );

            StoreAndUpdateView();
        }
        
        private void HandleMoveBindingDown( ContainerPropertyBinding binding )
        {
            ViewEditorCommon.MoveElementDown( _view.containerPropertyBindings, binding );

            StoreAndUpdateView();
        }

        
        private void HandleTogglePropertyExpansion( ContainerPropertyBinding binding )
        {
            binding.showExpanded = !binding.showExpanded;
            StoreAndUpdateView();
        }

        private void HandleRemoveBinding( ContainerPropertyBinding binding )
        {
            _view.containerPropertyBindings =
                _view.containerPropertyBindings.Where( x => x != binding )
                .ToArray();

            StoreAndUpdateView();
        }

        private void HandleDataSourceTypeChanged( ChangeEvent<string> changeEvent )
        {
            if ( _view != null )
            {
                _view.dataSourceType.Type = _validDataSources.FirstOrDefault( x => x.Name == changeEvent.newValue );

                StoreAndUpdateView();
            }
        }

        private void StoreAndUpdateView()
        {
            EditorUtility.SetDirty( _view );
            AssetDatabase.SaveAssetIfDirty( _view );

            MakeEditorView();

            _editorRootElement.MarkDirtyRepaint();
        }
    }
}