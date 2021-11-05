using System;

using UnityEngine;

namespace de.JochenHeckl.Unity.DataBinding
{
    public class ComponentPropertyBindingBuilder : PropertyBindingBuilder, IBindingBuilder
    {
		[BindableComponent]
		public Component targetComponent;

        [ComponentPropertyPath]
        public string targetPath;

        public IBinding BuildBinding()
        {
            var binding = Activator.CreateInstance<OneWayComponentPropertyBinding>();

            binding.DataSource = GetDataSource();
            binding.SourcePath = sourcePath;
            binding.Target = targetComponent;
            binding.TargetPath = targetPath;

            return binding;
        }
    }
}