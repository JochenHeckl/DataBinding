using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using static System.Net.Mime.MediaTypeNames;

namespace JH.DataBinding.Example.UserInputHandling.Tutorial
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
                new Dropdown.OptionData() { text = "Option 01" },
                new Dropdown.OptionData() { text = "Option 02" },
            };

            viewModel = new UserInputHandlingDataSource();

            viewModel.TypedText = MakeDisplayText(null);
            viewModel.SelectedDropDownText = MakeOptionText(dropdownOptions[0]);
            viewModel.PressCounterText = MakePressCounterText(pressCounter);

            viewModel.DropDownOptions = dropdownOptions;
            view.DataSource = viewModel;
        }

        private void HandleDropDownSelectionChanged(int selectedOptionIndex)
        {
            viewModel.NotifyChanges(
                (x) =>
                    x.SelectedDropDownText = MakeOptionText(x.DropDownOptions[selectedOptionIndex])
            );
        }

        private void HandleTypedTextChanged(string newText)
        {
            viewModel.NotifyChanges((x) => x.TypedText = MakeDisplayText(newText));
        }

        private void HandleButtonPressed()
        {
            pressCounter++;
            viewModel.NotifyChanges((x) => x.PressCounterText = MakePressCounterText(pressCounter));
        }

        private string MakeDisplayText(string newText)
        {
            if (string.IsNullOrEmpty(newText))
            {
                return "No text was typed.";
            }
            else
            {
                return $"You typed the text: {newText}.";
            }
        }

        private string MakeOptionText(Dropdown.OptionData optionData)
        {
            return $"Selected Option: {optionData.text}";
        }

        private string MakePressCounterText(int pressCounter)
        {
            return $"You pressed a button {pressCounter} times";
        }
    }
}
