using System;
using System.Diagnostics;

using UnityEngine;

namespace de.JochenHeckl.Unity.DataBinding
{
    [DebuggerDisplay("View({name}) : {DataSource}")]
    public class View : MonoBehaviour
    {
#if UNITY_EDITOR

        public SerializableType dataSourceType;
#endif // UNITY_EDITOR

        public ComponentPropertyBinding[] componentPropertyBindings =
            Array.Empty<ComponentPropertyBinding>();
        public ContainerPropertyBinding[] containerPropertyBindings =
            Array.Empty<ContainerPropertyBinding>();

        private INotifyDataSourceChanged dataSource;

        public void OnEnable()
        {
            BindBindingDataSources(dataSource);
            UpdateBindings();
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

        private void BindBindingDataSources(object dataSource)
        {
            if (dataSource == null)
            {
                return;
            }

            foreach (var binding in componentPropertyBindings)
            {
                binding.DataSource = dataSource;
            }

            foreach (var binding in containerPropertyBindings)
            {
                binding.DataSource = dataSource;
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
