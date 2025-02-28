using UnityEngine;

namespace JH.DataBinding.Example.EditableDropdown
{
    public class EditableDropdownSetup : MonoBehaviour
    {
        public View view;

        private EditableDropdownDataSource dataSource;

        void Start()
        {
            dataSource = new EditableDropdownDataSource()
            {
                InputValueChanged = HandleInputValueChanged,
                DropdownOptions = new string[] { "Apple", "Pear", "Orange", "Plum" },
                InputValue = "Pear",
            };

            view.DataSource = dataSource;
        }

        private void HandleInputValueChanged(string newValue)
        {
            if (dataSource.InputValue != newValue)
            {
                dataSource.NotifyChanges(x => x.InputValue = newValue);
            }
        }
    }
}
