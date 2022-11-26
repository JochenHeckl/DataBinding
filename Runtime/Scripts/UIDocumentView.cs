using System;
using System.Diagnostics;

using UnityEngine;
using UnityEngine.UIElements;

namespace de.JochenHeckl.Unity.DataBinding
{
    [DebuggerDisplay("UIDocumentView ({name}) : {DataSource}")]
    [RequireComponent(typeof(UIDocument))]
    public class UIDocumentView : MonoBehaviour
    {
#if UNITY_EDITOR

        public SerializableType dataSourceType;
#endif // UNITY_EDITOR

        public VisualElementPropertyBinding[] visualElementPropertyBindings =
            Array.Empty<VisualElementPropertyBinding>();

        private INotifyDataSourceChanged dataSource;
        private VisualElement _rootVisualElement;

        public VisualElement RootVisualElement
        {
            get
            {
                if (_rootVisualElement == null)
                {
                    var uiDocument = GetComponent<UIDocument>();

                    if (uiDocument != null)
                    {
                        _rootVisualElement = uiDocument.rootVisualElement;
                    }
                }

                return _rootVisualElement;
            }
        }

        public void OnEnable()
        {
            BindBindingDataSources();
            BindBindingVisualElements();
            UpdateBindings();
        }

        public INotifyDataSourceChanged DataSource
        {
            get { return dataSource; }
            set
            {
                var oldDataSource = dataSource;

                if (oldDataSource != null)
                {
                    oldDataSource.DataSourceChanged -= OnDataSourceChanged;
                }

                dataSource = value;
                BindBindingDataSources();

                var newDataSource = value;

                if (newDataSource != null)
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

        private void BindBindingDataSources()
        {
            if (dataSource == null)
            {
                return;
            }

            foreach (var binding in visualElementPropertyBindings)
            {
                binding.DataSource = dataSource;
            }
        }

        private void BindBindingVisualElements()
        {
            foreach (var binding in visualElementPropertyBindings)
            {
                binding.RootVisualElement = RootVisualElement;
            }
        }

        private void UpdateBindings()
        {
            foreach (var binding in visualElementPropertyBindings)
            {
                binding.UpdateBinding();
            }

            RootVisualElement.MarkDirtyRepaint();
        }
    }
}
