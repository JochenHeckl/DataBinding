using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine.UIElements;

namespace de.JochenHeckl.Unity.DataBinding.Editor
{
    internal class BindingEditor<BindingType> : VisualElement
    {
        public BindingType Binding { get; private set; }

        public BindingEditor(BindingType binding)
        {
            Binding = binding;
        }

        protected void MakeErrorUI(Exception exception, Action<BindingType> removeBinding)
        {
            Clear();
            ClearClassList();

            AddToClassList(DataBindingEditorStyles.invalidBindingClassName);

            Add(new Label("Failed to setup UI for VisualElementPropertyBinding."));
            Add(new Label(exception.Message));

            var removeBindingButton = new Button(() => removeBinding(Binding));
            removeBindingButton.text = "Remove Binding";
            Add(removeBindingButton);
        }
    }
}
