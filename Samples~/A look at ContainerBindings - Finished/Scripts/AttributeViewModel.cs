namespace JH.DataBinding.Examples.ContainerBindings
{
    public class AttributeViewModel : DataSourceBase<AttributeViewModel>
    {
        public string Name { get; set; }
        public int Value { get; set; }
        public string ValueString => $"{Value}";
    }
}
