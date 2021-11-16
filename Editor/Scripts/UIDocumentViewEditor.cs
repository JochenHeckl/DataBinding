using System;
using System.Linq;

using UnityEditor;

using UnityEngine.UIElements;

namespace de.JochenHeckl.Unity.DataBinding.Editor
{
    [CustomEditor( typeof( UIDocumentView ), true )]
    public class UIDocumentViewEditor : UnityEditor.Editor
    {
        private UIDocumentView _view;
        private static Type[] _validDataSources;
        private VisualElement _editorRootElement;

        public void OnEnable()
        {
            _view = target as UIDocumentView;
            
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

            _editorRootElement.Add( MakeVisualElementPropertyBindings() );
        }

        private VisualElement MakeVisualElementPropertyBindings()
        {
            return ViewEditorCommon.MakeBindingSection(
                "VisualElement Property Bindings",
                HandleAddVisualElementPropertyBinding,
                _view.visualElementPropertyBindings,
                MakeVisualElementPropertyBindingVisualElement );
        }

        private void HandleAddVisualElementPropertyBinding()
        {
            _view.visualElementPropertyBindings = _view.visualElementPropertyBindings
                .Append( new VisualElementPropertyBinding() )
                .ToArray();

            StoreAndUpdateView();
        }

        private VisualElement MakeVisualElementPropertyBindingVisualElement( VisualElementPropertyBinding binding )
        {
            return new VisualElementPropertyBindingVisualElement(
                        _view.dataSourceType.Type,
                        _view.UIDocument.rootVisualElement,
                        binding,
                        StoreAndUpdateView,
                        binding => binding.showExpanded,
                        HandleMoveBindingUp,
                        HandleMoveBindingDown,
                        HandleTogglePropertyExpansion,
                        HandleRemoveBinding );
        }

        private void HandleMoveBindingUp( VisualElementPropertyBinding binding )
        {
            ViewEditorCommon.MoveElementUp( _view.visualElementPropertyBindings, binding );

            StoreAndUpdateView();
        }
        
        private void HandleMoveBindingDown( VisualElementPropertyBinding binding )
        {
            ViewEditorCommon.MoveElementDown( _view.visualElementPropertyBindings, binding );

            StoreAndUpdateView();
        }

        private void HandleTogglePropertyExpansion( VisualElementPropertyBinding binding )
        {
            binding.showExpanded = !binding.showExpanded;
            StoreAndUpdateView();
        }

        private void HandleRemoveBinding( VisualElementPropertyBinding binding )
        {
            _view.visualElementPropertyBindings = _view.visualElementPropertyBindings
                .Where( x => x != binding )
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