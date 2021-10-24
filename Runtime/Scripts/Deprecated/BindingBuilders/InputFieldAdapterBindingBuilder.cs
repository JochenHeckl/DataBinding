using System;
using System.Linq;

using UnityEngine.UI;


namespace de.JochenHeckl.Unity.DataBinding
{
    public class InputFieldAdapterBindingBuilder : PropertyBindingBuilder, IBindingBuilder
    {
        public InputField inputField;

        public IBinding BuildBinding()
        {
            var boundPropertyType = GetSourcePropertyType();
            var bindingType = typeof( TwoWayPropertyBinding<> ).MakeGenericType( boundPropertyType );

            var binding = Activator.CreateInstance( bindingType ) as IBinding;
            binding.SourcePath = sourcePath;
            
            var adapterType = typeof( InputFieldTextAdapter<> ).MakeGenericType( boundPropertyType );
            var adapter = Activator.CreateInstance( adapterType, new object[] { inputField } );

            inputField.onValueChanged.RemoveAllListeners();
            inputField.onValueChanged.AddListener( (x) => binding.ReverseUpdateBinding() );

            var targetAdapterPropertyInfos = bindingType.GetProperties();
            var targetAdapterPropertyInfo = targetAdapterPropertyInfos.Single( x => x.Name == "TargetAdapter" );

            targetAdapterPropertyInfo.SetValue( binding, adapter );

            return binding;
        }
    }
}
