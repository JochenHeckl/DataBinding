using System;

namespace de.JochenHeckl.Unity.DataBinding.Example
{
    public class UserInputFeedbackDataBindingView : View
    {
        public Action<string> HandleInputChangedAction { get; set; }
        public Action<int> HandleSelectionChangedCallback { get; set; }

        public void OnTextInputValueChanged( string newValue ) => HandleInputChangedAction?.Invoke( newValue );
        public void OnSelectedOptionValueChanged( int newValue ) => HandleSelectionChangedCallback?.Invoke( newValue );
    }
}