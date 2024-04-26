using System;
using Unity.Collections;
using UnityEngine;

namespace JH.DataBinding.Example.EditableDropdown
{
    public class EditableDropdownSetup : MonoBehaviour
    {
        public View view;

        private ExampleDataSource dataSource;

        void Start()
        {
            dataSource = new ExampleDataSource()
            {
                InputValueChanged = HandleInputValueChanged,
                DropdownOptions = new string[] { "Apple", "Pear", "Orange", "Plum" },
                InputValue = "Pear"
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
