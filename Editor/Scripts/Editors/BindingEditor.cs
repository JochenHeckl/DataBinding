using System;

using UnityEngine.UIElements;

namespace de.JochenHeckl.Unity.DataBinding.Editor
{
    internal class BindingEditor<BindingType> : VisualElement
    {
        public IDataBindingEditorDisplayText DisplayText { get; private set; }

        public BindingType Binding { get; private set; }

        public BindingEditor(IDataBindingEditorDisplayText displayText, BindingType binding)
        {
            DisplayText = displayText;
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

        protected void AddHeaderButton(VisualElement parent, Button button)
        {
            button.AddToClassList(DataBindingEditorStyles.bindingActionButton);
            parent.Add(button);
        }
    }
}
