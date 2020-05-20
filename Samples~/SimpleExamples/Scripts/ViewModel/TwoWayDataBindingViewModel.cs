namespace de.JochenHeckl.Unity.DataBinding.Example
{
    public class TwoWayDataBindingViewModel : ViewModelBase
    {
        public double TwoWayProperty { get; set; }
        public string OneWayTextProperty { get { return TwoWayProperty.ToString(); } }
    }
}