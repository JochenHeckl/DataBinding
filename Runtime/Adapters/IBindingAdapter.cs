namespace de.JochenHeckl.Unity.DataBinding
{
    public interface IBindingAdapter<ValueType>
    {
        ValueType Value
        {
            set;
        }
    }
}
