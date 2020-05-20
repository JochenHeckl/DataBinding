using System;
using UnityEngine.UI;

namespace de.JochenHeckl.Unity.DataBinding
{
    public class ButtonCommandAdapterBindingBuilder : PropertyBindingBuilder, IBindingBuilder
    {
        [BindableComponent]
        public Button targetComponent;

        public IBinding BuildBinding()
        {
            var boundPropertyType = GetSourcePropertyType();

            if ( !boundPropertyType.InheritsOrImplements( typeof( ICommand ) ) )
            {
                throw new InvalidOperationException( "Button Commands must be bound to properties that implement ICommand" );
            }

            var adapter = new ButtonCommandAdapter( targetComponent );
            var bindignType = typeof( OneWayPropertyAdapterBinding<> ).MakeGenericType( boundPropertyType );
            var binding = Activator.CreateInstance( bindignType, adapter, sourcePath ) as IBinding;

            return binding;
        }
    }
}
