using System;

namespace JH.DataBinding.Example.EditableDropdown
{
    public class ExampleDataSource : DataSourceBase<ExampleDataSource>
    {
        public Action<string> InputValueChanged { get; set; }
        public string[] DropdownOptions { get; set; }
        public string InputValue { get; set; }
    }
}
