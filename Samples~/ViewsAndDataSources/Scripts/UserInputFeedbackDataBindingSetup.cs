using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

namespace de.JochenHeckl.Unity.DataBinding.Example
{
    public class UserInputFeedbackDataBindingSetup : MonoBehaviour
    {
        public View view;
        public Sprite OneSprite;
        public Sprite OtherSprite;

        private UserInputFeedbackDataBindingDataSource viewModel;

        public void Start()
        {
            viewModel = new UserInputFeedbackDataBindingDataSource();

            viewModel.TypedText = "No Text was typed.";
            viewModel.SelectedDropDownText = "No selection was made.";

            viewModel.DropDownOptions = new List<Dropdown.OptionData>()
            {
                    new Dropdown.OptionData() { image = OneSprite, text = "One Option" },
                    new Dropdown.OptionData() { image = OtherSprite, text = "Other Option" },
            };

            viewModel.HandleTypedTextChanged = HandleTypedTextChanged;
            viewModel.HandleDropDownSelectionChanged = HandleDropDownSelectionChanged;

            viewModel.DynamicElements = Enumerable.Range(1, 20)
                .Select(x => new ElementDataSource() { ElementValue = x.ToString() })
                .ToList();

            view.DataSource = viewModel;
        }

        private void HandleDropDownSelectionChanged(int selectedOptionIndex)
        {
            viewModel.NotifyChanges((x) => x.SelectedDropDownText = x.DropDownOptions[selectedOptionIndex].text);
        }

        private void HandleTypedTextChanged(string newText)
        {
            viewModel.NotifyChanges((x) =>x.TypedText = newText);
        }
    }
}