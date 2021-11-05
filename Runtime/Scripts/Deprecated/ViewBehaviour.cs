using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace de.JochenHeckl.Unity.DataBinding
{
    [DebuggerDisplay( "ViewBehaviour({name}) : {DataSource}" )]
    public class ViewBehaviour : MonoBehaviour
    {

#if UNITY_EDITOR

        [TypeFilter( typeof( IDataSource ) )]
        public SerializableType dataSourceType;

        public bool condenseValidBuilders;

#endif // UNITY_EDITOR

        private static IEnumerable<IBindingBuilder> GatherBindingBuilders( GameObject gameobject )
        {
            var localBindingBuilders = gameobject
                .GetComponents<MonoBehaviour>()
                .Where( x => x.GetType().InheritsOrImplements( typeof( IBindingBuilder ) ) )
               .Cast<IBindingBuilder>()
               .ToArray();

            foreach ( var localbindingBuilder in localBindingBuilders )
            {
                yield return localbindingBuilder;
            }

            // cascade down into all MonoBeheviours that are not views themselves
            // these child views will handle the bindings deeper down the chain themselves

            var childBindingBuilders = Enumerable.Range( 0, gameobject.transform.childCount )
                .Select( x => gameobject.transform.GetChild( x ) )
                .Where( x => x.GetComponent<ViewBehaviour>() == null )
                .SelectMany( x => GatherBindingBuilders( x.gameObject ) )
                .ToArray();

            foreach ( var childBindingBuilder in childBindingBuilders )
            {
                yield return childBindingBuilder;
            }
        }

        public bool ShowView
        {
            get { return gameObject.activeSelf; }
            set { gameObject.SetActive( value ); }
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
                BindBindings( dataSource );

                var newDataSource = DataSource as IDataSource;

                if ( newDataSource != null )
                {
                    newDataSource.ViewModelChanged += OnDataSourceChanged;
                }

                UpdateBindings();
            }
        }

        public IBinding[] Bindings
        {
            get;
            private set;
        }

        private void OnDataSourceChanged()
        {
            UpdateBindings();
        }

        private void BindBindings( object dataSource )
        {
            if (dataSource == null)
            {
                Bindings = null;
                return;
            }

            if ( Bindings == null )
            {
                var noViewChildrenBindingBuilders = GatherBindingBuilders( gameObject );
                var bindings = noViewChildrenBindingBuilders.Select( x => x.BuildBinding() ).ToArray();

                Bindings = bindings.Where( x => x != null ).ToArray();
            }

            foreach ( var binding in Bindings )
            {
                binding.DataSource = dataSource;
            }
        }

        private void UpdateBindings()
        {
            if ( Bindings == null )
            {
                return;
            }

            foreach ( var binding in Bindings )
            {
                binding.UpdateBinding();
            }
        }

        private object dataSource;
    }
}
