using System.Diagnostics;
using System.Reflection;

namespace de.JochenHeckl.Unity.DataBinding
{
    [DebuggerDisplay("{dataSource.GetType().Name}.{Path} => {BindingAdapter.GetType().Name}")]
    internal class OneWayPropertyAdapterBinding<ValueType> : IAdapterBinding<ValueType>
    {
        public OneWayPropertyAdapterBinding()
        {

        }

        public OneWayPropertyAdapterBinding( IBindingAdapter<ValueType> adapter, string sourcePathIn )
        {
            TargetAdapter = adapter;
            SourcePath = sourcePathIn;
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

        public IBindingAdapter<ValueType> TargetAdapter
        {
            get;
            set;
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
            boundPropertyGetter = propertyInfo?.GetGetMethod();
        }

        public void UpdateBinding()
        {
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
        }

        public void ReverseUpdateBinding()
        {
            throw new System.InvalidOperationException("OneWayBindings must never reverse update.");
        }

        private object dataSource;
        private MethodInfo boundPropertyGetter;
    }
}