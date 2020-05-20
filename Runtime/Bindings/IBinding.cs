namespace de.JochenHeckl.Unity.DataBinding
{
    public interface IBinding
    {
        object DataSource
        {
            get;
            set;
        }

        string SourcePath
        {
            get;
            set;
        }

        void UpdateBinding();
        void ReverseUpdateBinding();
    }

    public interface IComponentPropertyBinding : IBinding
    {
        object Target
        {
            get;
            set;
        }

        string TargetPath
        {
            get;
            set;
        }
    }


    public interface IAdapterBinding<ValueType> : IBinding
    {
        IBindingAdapter<ValueType> TargetAdapter
        {
            get;
        }
    }

    public interface ISignalingAdapterBinding<ValueType> : IBinding
    {
        ISignalingBindingAdapter<ValueType> TargetAdapter
        {
            get;
        }
    }
}
