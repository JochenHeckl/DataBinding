using System;
using System.Diagnostics;
using System.Reflection;

namespace de.JochenHeckl.Unity.DataBinding
{
    [DebuggerDisplay("{dataSource.GetType().Name}.{Path} => {BindingAdapter.GetType().Name}")]

    [Obsolete( "This will be removed in version 2.0" )]
    internal class TwoWayPropertyBinding<ValueType> : ISignalingAdapterBinding<ValueType>
    {
        public ISignalingBindingAdapter<ValueType> TargetAdapter
        {
            get;
            set;
        }

        public object DataSource
        {
            get
            {
                return dataSource;
            }
            set
            {
                Bind(value);
            }
        }

        public string SourcePath
        {
            get;
            set;
        }

        public void Bind( object dataSourceIn )
        {
            dataSource = dataSourceIn;

            var propertyInfo = dataSource.ResolvePublicPropertyPath(SourcePath);
            boundPropertyGetter = ( propertyInfo != null ) ? propertyInfo.GetGetMethod() : null;
            boundPropertySetter = ( propertyInfo != null ) ? propertyInfo.GetSetMethod() : null;
        }

        public void UpdateBinding()
        {
            if( isUpdateing )
            {
                return;
            }

            isUpdateing = true;

            if ( dataSource != null )
            {
                if ( string.IsNullOrEmpty(SourcePath) )
                {
                    TargetAdapter.Value = (ValueType) dataSource;
                }
                else
                {
                    var value = boundPropertyGetter.Invoke(dataSource, null);
                    TargetAdapter.Value = (ValueType) value;
                }
            }
            else
            {
                TargetAdapter.Value = default;
            }

            isUpdateing = false;
        }

        public void ReverseUpdateBinding()
        {
            if ( !isUpdateing  && dataSource != null )
            {
                isUpdateing = true;

                boundPropertySetter.Invoke(dataSource, new object[] { TargetAdapter.Value });

                if ( dataSource is IDataSource )
                {
                    ( (IDataSource) dataSource ).NotifyViewModelChanged();
                }

                isUpdateing = false;
            }
        }

        private bool isUpdateing = false;
        
        private object dataSource;
        private MethodInfo boundPropertyGetter;
        private MethodInfo boundPropertySetter;
    }
}
