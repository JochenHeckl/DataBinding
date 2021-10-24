using System;
using System.Diagnostics;

using UnityEngine;

namespace de.JochenHeckl.Unity.DataBinding
{
	[DebuggerDisplay( "View({name}) : {DataSource}" )]
    public class View : MonoBehaviour
    {

#if UNITY_EDITOR

        public SerializableType dataSourceType;

        public bool condenseValidBuilders;

#endif // UNITY_EDITOR

        public ComponentPropertyBinding[] componentPropertyBindings = Array.Empty<ComponentPropertyBinding>();

		public void OnEnable()
		{
            BindBindingDataSources( dataSource );
            UpdateBindings();
		}

		public object DataSource
        {
            get
            {
                // return local data source if available

                if ( dataSource != null )
                {
                    return dataSource;
                }

                // walk up the hierarchy if no local data source is available

                var parent = transform.parent;

                if ( parent != null )
                {
                    var parentViewBehaviour = parent.GetComponent<ViewBehaviour>();

                    if ( parentViewBehaviour != null )
                    {
                        return parentViewBehaviour.DataSource;
                    }
                }

                return null;
            }

            set
            {
                var oldDataSource = DataSource as IDataSource;

                if ( oldDataSource != null )
                {
                    oldDataSource.ViewModelChanged -= OnDataSourceChanged;
                }

                dataSource = value;
                BindBindingDataSources( dataSource );

                var newDataSource = DataSource as IDataSource;

                if ( newDataSource != null )
                {
                    newDataSource.ViewModelChanged += OnDataSourceChanged;
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
            if (dataSource == null)
            {
                return;
            }

            foreach ( var binding in componentPropertyBindings )
            {
                binding.DataSource = dataSource;
            }
        }

        private void UpdateBindings()
        {
            if ( componentPropertyBindings == null )
            {
                return;
            }

            foreach ( var binding in componentPropertyBindings )
            {
                binding.UpdateBinding();
            }
        }

        private object dataSource;
    }
}
