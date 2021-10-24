using System;
using System.Linq;

using UnityEngine.UI;


namespace de.JochenHeckl.Unity.DataBinding
{
    [Obsolete( "User ComponentPropertyBindingBuilders for options and value instead. Will be gone in 2.0" )]
    public class DropdownAdapterBindingBuilder : PropertyBindingBuilder, IBindingBuilder
    {
        public Dropdown dropdown;

        public IBinding BuildBinding()
        {
            var boundPropertyType = GetSourcePropertyType();
            var bindingType = typeof( TwoWayPropertyBinding<> ).MakeGenericType( boundPropertyType );

            var binding = Activator.CreateInstance( bindingType ) as IBinding;
            binding.SourcePath = sourcePath;
            
            var adapter = Activator.CreateInstance( typeof( DropdownValueAdapter ), new object[] { dropdown } );

            dropdown.onValueChanged.RemoveAllListeners();
            dropdown.onValueChanged.AddListener( (x) => binding.ReverseUpdateBinding() );

            var targetAdapterPropertyInfos = bindingType.GetProperties();
            var targetAdapterPropertyInfo = targetAdapterPropertyInfos.Single( x => x.Name == "TargetAdapter" );

            targetAdapterPropertyInfo.SetValue( binding, adapter );

            return binding;
        }
    }
}
