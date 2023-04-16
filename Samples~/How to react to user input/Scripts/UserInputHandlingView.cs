using System;

namespace de.JochenHeckl.Unity.DataBinding.Example
{
    public class UserInputHandlingView : View
    {
        public Action<string> HandleInputChangedAction { get; set; }
        public Action<int> HandleSelectionChangedCallback { get; set; }
        public Action HandleButtonPressed { get; set; }

        public void OnTextInputValueChanged(string newValue) =>
            HandleInputChangedAction?.Invoke(newValue);

        public void OnSelectedOptionValueChanged(int newValue) =>
            HandleSelectionChangedCallback?.Invoke(newValue);

        public void OnCounterButtonPressed() => HandleButtonPressed?.Invoke();
    }
}
