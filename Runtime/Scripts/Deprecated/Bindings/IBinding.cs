using System;

namespace de.JochenHeckl.Unity.DataBinding
{
    public interface IBinding
    {
        // TODO: Change to IDataSource in 2.0
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

        [Obsolete( "This will be removed in version 2.0. Do bind actions to handle user input." )]
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


    [Obsolete( "This will be removed in version 2.0. Just do use code in views." )]
    public interface IAdapterBinding<ValueType> : IBinding
    {
        IBindingAdapter<ValueType> TargetAdapter
        {
            get;
        }
    }

    [Obsolete( "This will be removed in version 2.0. IT was only ever needed for TwoWay bindings which will soon be a thing of the past." )]
    public interface ISignalingAdapterBinding<ValueType> : IBinding
    {
        ISignalingBindingAdapter<ValueType> TargetAdapter
        {
            get;
        }
    }
}
