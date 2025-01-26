using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;

namespace JH.DataBinding.Editor
{
    internal class DataSourceSelection : VisualElement
    {
        internal DataSourceSelection(View view, Action<Type> updateDataSourceType)
        {
            this.ApplyDataSourceSelectionStyle();

            var validDataSources = DataBindingCommonData.GetValidDataSourceTypes();

            if (validDataSources.Length == 0)
            {
                var labelContainer = new VisualElement();

                var textErrorContent = new Label(
                    DataBindingCommonData.EditorDisplayText.MissingDataSourcesErrorText
                );

                textErrorContent.ApplyErrorTextStyle();
                labelContainer.Add(textErrorContent);

                Add(labelContainer);
                return;
            }

            var dataSourceDropDown = new DropdownField(
                label: DataBindingCommonData.EditorDisplayText.DataSourceTypeText,
                choices: validDataSources.Select(x => x.GetFriendlyName()).ToList(),
                defaultValue: view.dataSourceType.Type.Name
            );

            dataSourceDropDown.RegisterValueChangedCallback(changeEvent =>
            {
                updateDataSourceType(
                    validDataSources.FirstOrDefault(x => x.Name == changeEvent.newValue)
                );
            });

            Add(dataSourceDropDown);

            var dataSourceSourceFile = DataBindingCommonData.FindDataSourceSourceFile(
                view.dataSourceType.Type
            );

            var button = new Button(() => OpenDataSourceEditor(dataSourceSourceFile))
            {
                text = DataBindingCommonData.EditorDisplayText.EditSourceText,
                tooltip = DataBindingCommonData.EditorDisplayText.NoSourceCodeAvailableToolTip,
            };

            button.SetEnabled(dataSourceSourceFile != null);

            Add(button);
        }

        private static void OpenDataSourceEditor(string dataSourceSourceFile)
        {
            if (dataSourceSourceFile != null)
            {
                var scriptAsset = AssetDatabase.LoadAssetAtPath<MonoScript>(
                    dataSourceSourceFile.Replace('/', Path.DirectorySeparatorChar)
                );

                if (scriptAsset != null)
                {
                    AssetDatabase.OpenAsset(scriptAsset);
                }
            }
        }
    }
}
