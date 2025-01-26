using System;
using System.Diagnostics;
using UnityEngine;

namespace JH.DataBinding
{
    [DebuggerDisplay("View({name}) : {DataSource}")]
    public class View : MonoBehaviour
    {
#if UNITY_EDITOR
        [HideInInspector]
        public SerializableType dataSourceType;
#endif // UNITY_EDITOR

        // [HideInInspector]
        public ComponentPropertyBinding[] componentPropertyBindings =
            Array.Empty<ComponentPropertyBinding>();

        // [HideInInspector]
        public ContainerPropertyBinding[] containerPropertyBindings =
            Array.Empty<ContainerPropertyBinding>();

        private INotifyDataSourceChanged dataSource;

        public void OnEnable()
        {
            BindBindingDataSources(dataSource);
            UpdateBindings();
        }

        public void OnDestroy()
        {
            if (DataSource != null)
            {
                DataSource = null;
            }
        }

        public INotifyDataSourceChanged DataSource
        {
            get
            {
                // return local data source if available

                if (dataSource != null)
                {
                    return dataSource;
                }

                // walk up the hierarchy if no local data source is available

                var parent = transform.parent;

                if (parent != null)
                {
                    var parentViewBehaviour = parent.GetComponent<View>();

                    if (parentViewBehaviour != null)
                    {
                        return parentViewBehaviour.DataSource;
                    }
                }

                return null;
            }
            set
            {
                if (dataSource == value)
                {
                    return;
                }

                var oldDataSource = DataSource as INotifyDataSourceChanged;

                if (oldDataSource != null)
                {
                    oldDataSource.DataSourceChanged -= OnDataSourceChanged;
                }

                dataSource = value;
                BindBindingDataSources(dataSource);

                var newDataSource = DataSource as INotifyDataSourceChanged;

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

        private void BindBindingDataSources(object bindingDataSource)
        {
            foreach (var binding in componentPropertyBindings)
            {
                binding.DataSource = bindingDataSource;
            }

            foreach (var binding in containerPropertyBindings)
            {
                binding.DataSource = bindingDataSource;
            }
        }

        private void UpdateBindings()
        {
            foreach (var binding in componentPropertyBindings)
            {
                binding.UpdateBinding();
            }

            foreach (var binding in containerPropertyBindings)
            {
                binding.UpdateBinding();
            }
        }
    }
}
