namespace JH.DataBinding.Examples.ContainerBindingTest
{

public class ContainerViewDataSource : DataSourceBase<ContainerViewDataSource>
{
    public ItemDataSource[] Items { get; set; }
}
}