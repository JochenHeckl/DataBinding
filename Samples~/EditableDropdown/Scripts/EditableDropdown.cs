using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace JH.DataBinding.Example.EditableDropdown
{
    public class EditableDropdown : MonoBehaviour
    {
        public TMP_Dropdown dropdown;
        public TMP_InputField inputField;
        public Action<string> ValueChanged
        {
            set { inputField.onValueChanged.AddListener(new UnityAction<string>(value)); }
        }

        public string Value
        {
            set { inputField.text = value; }
        }

        public string[] Options
        {
            get { return dropdown.options.Select(x => x.text).ToArray(); }
            set
            {
                if (value != null)
                {
                    dropdown.options = value.Select(x => new TMP_Dropdown.OptionData(x)).ToList();
                }
                else
                {
                    dropdown.options.Clear();
                }
            }
        }

        private Action<string> valueChangedHandler;

        protected void Awake()
        {
            if (dropdown == null)
            {
                throw new ArgumentException("dropdown must be assigned a TMP_Dropdown.");
            }

            if (inputField == null)
            {
                throw new ArgumentException("inputField must be assigned a TMP_InputField.");
            }

            dropdown.onValueChanged.AddListener(new UnityAction<int>(HandleDropdownValueChanged));
            HandleDropdownValueChanged(dropdown.value);
        }

        public void HandleDropdownValueChanged(int selectedDropdownIndex)
        {
            if (dropdown.options.Count > selectedDropdownIndex)
            {
                inputField.text = dropdown.options[selectedDropdownIndex].text;
            }
        }
    }
}
