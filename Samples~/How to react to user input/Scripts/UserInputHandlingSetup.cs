using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

namespace de.JochenHeckl.Unity.DataBinding.Example
{
    public class UserInputHandlingSetup : MonoBehaviour
    {
        public View view;

        private UserInputHandlingDataSource viewModel;
        private int pressCounter = 0;

        public void Start()
        {
            var dropdownOptions = new List<Dropdown.OptionData>()
            {
                new Dropdown.OptionData() { text = "One Option" },
                new Dropdown.OptionData() { text = "Other Option" },
            };

            viewModel = new UserInputHandlingDataSource();

            viewModel.TypedText = "No Text was typed.";
            viewModel.SelectedDropDownText = dropdownOptions[0].text;
            viewModel.PressCounterText = MakePressCounterText(pressCounter);

            viewModel.DropDownOptions = dropdownOptions;
            viewModel.HandleTypedTextChanged = HandleTypedTextChanged;
            viewModel.HandleDropDownSelectionChanged = HandleDropDownSelectionChanged;
            viewModel.HandleButtonPressed = HandleButtonPressed;

            view.DataSource = viewModel;
        }

        private void HandleDropDownSelectionChanged(int selectedOptionIndex)
        {
            viewModel.NotifyChanges(
                (x) => x.SelectedDropDownText = x.DropDownOptions[selectedOptionIndex].text
            );
        }

        private void HandleTypedTextChanged(string newText)
        {
            viewModel.NotifyChanges((x) => x.TypedText = newText);
        }

        private void HandleButtonPressed()
        {
            pressCounter++;
            viewModel.NotifyChanges((x) => x.PressCounterText = MakePressCounterText(pressCounter));
        }

        private string MakePressCounterText(int pressCounter)
        {
            return $"You pressed a button {pressCounter} times";
        }
    }
}
