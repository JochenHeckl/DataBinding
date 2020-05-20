using UnityEngine;
using System;

namespace de.JochenHeckl.Unity.DataBinding.Example
{
    public class SimpleDataBindingViewModel : ViewModelBase
    {
        public string SomeBindableText { get; set; }
        public Color SomeBindableTextColor { get; set; }
        public Sprite SomeBindableImage { get; set; }

        public PrefabViewModel SomeBindableView { get;set;}
    }
}