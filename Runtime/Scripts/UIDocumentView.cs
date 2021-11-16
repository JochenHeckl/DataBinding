using System;
using System.Diagnostics;

using UnityEngine;
using UnityEngine.UIElements;

namespace de.JochenHeckl.Unity.DataBinding
{
    [DebuggerDisplay( "UIDocumentView ({name}) : {DataSource}" )]

    [RequireComponent( typeof( UIDocument ) )]
    public class UIDocumentView : MonoBehaviour
    {

#if UNITY_EDITOR

        public SerializableType dataSourceType;
        public UIDocument UIDocument
        {
            get
            {
                if ( _uIDocument == null )
                { 
                    _uIDocument = GetComponent<UIDocument>();
                }

                return _uIDocument;
            }
        }
        

#endif // UNITY_EDITOR

        public VisualElementPropertyBinding[] visualElementPropertyBindings = Array.Empty<VisualElementPropertyBinding>();
        private INotifyDataSourceChanged _dataSource;
        private UIDocument _uIDocument;

        public void OnEnable()
        {
            _uIDocument = GetComponent<UIDocument>();
            BindBindingDataSources( _dataSource );
            BindBindingVisualElements( _uIDocument );
            UpdateBindings();
        }

        public INotifyDataSourceChanged DataSource
        {
            get
            {
                return _dataSource;
            }

            set
            {
                var oldDataSource = _dataSource;

                if ( oldDataSource != null )
                {
                    oldDataSource.DataSourceChanged -= OnDataSourceChanged;
                }

                _dataSource = value;
                BindBindingDataSources( _dataSource );

                var newDataSource = value;

                if ( newDataSource != null )
                {
                    newDataSource.DataSourceChanged += OnDataSourceChanged;
                }

                UpdateBindings();
            }
        }

        private void OnDataSourceChanged()
        {
            UpdateBindings();
        }

        private void BindBindingDataSources( object dataSource )
        {
            if ( dataSource == null )
            {
                return;
            }

            foreach ( var binding in visualElementPropertyBindings )
            {
                binding.DataSource = _dataSource;
            }
        }

        private void BindBindingVisualElements( UIDocument uiDocument )
        {
            foreach ( var binding in visualElementPropertyBindings )
            {
                binding.RootVisualElement = uiDocument.rootVisualElement;
            }
        }

        private void UpdateBindings()
        {
            foreach ( var binding in visualElementPropertyBindings )
            {
                binding.UpdateBinding();
            }

            _uIDocument.rootVisualElement.MarkDirtyRepaint();
        }
    }
}
