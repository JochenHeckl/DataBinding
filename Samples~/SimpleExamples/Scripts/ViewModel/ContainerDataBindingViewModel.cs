using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace de.JochenHeckl.Unity.DataBinding.Example
{
    public class ContainerDataBindingViewModel : ViewModelBase
    {
        public IEnumerable<TextItemViewModel> TextItems { get; set; }

        public ICommand AddItemCommand { get; set; }
    }
}