using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace JH.DataBinding.Editor
{
    internal class DataSourceSelection : VisualElement
    {
        internal DataSourceSelection(View view, Action<Type> updateDataSourceType)
        {
            var validDataSources = DataBindingCommonData.GetValidDataSourceTypes();

            if (validDataSources.Length == 0)
            {
                var labelContainer = new VisualElement();
                labelContainer.AddToClassList(DataBindingEditorStyle.errorMessageContainer);

                var textErrorContent = new Label(
                    DataBindingCommonData.EditorDisplayText.MissingDataSourcesErrorText
                );

                textErrorContent.AddToClassList(DataBindingEditorStyle.errorMessageText);

                labelContainer.Add(textErrorContent);

                var createDataSourceButton = new Button(() =>
                {
                    CreateDefaultDataSource(view.name);
                    updateDataSourceType(null);
                });

                createDataSourceButton.text = DataBindingCommonData
                    .EditorDisplayText
                    .CreateDefaultDataSourceText;

                createDataSourceButton.AddToClassList(
                    DataBindingEditorStyle.createDefaultDataSourceButton
                );

                labelContainer.Add(createDataSourceButton);

                Add(labelContainer);

                return;
            }

            var dataSourceDropDown = new DropdownField(
                label: DataBindingCommonData.EditorDisplayText.DataSourceTypeText,
                choices: validDataSources.Select(x => x.GetFriendlyName()).ToList(),
                defaultValue: view.dataSourceType.Type.GetFriendlyName()
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

            var buttonGroup = new VisualElement();
            buttonGroup.AddToClassList(DataBindingEditorStyle.dataSourceSelectionButtonGroup);

            var newButton = new Button(() => CreateNewDataSource(view.name))
            {
                text = DataBindingCommonData.EditorDisplayText.NewDataSourceText,
                tooltip = DataBindingCommonData.EditorDisplayText.NewDataSourceTooltip,
            };

            buttonGroup.Add(newButton);

            var editButton = new Button(() => OpenDataSourceEditor(dataSourceSourceFile))
            {
                text = DataBindingCommonData.EditorDisplayText.EditDataSourceText,
                tooltip = DataBindingCommonData.EditorDisplayText.EditDataSourceTooltip,
            };

            editButton.SetEnabled(dataSourceSourceFile != null);

            buttonGroup.Add(editButton);

            Add(buttonGroup);
        }

        private void CreateNewDataSource(string name) { }

        private void CreateDefaultDataSource(string name)
        {
            var directory = GetSelectedDirectoryOrFallback();

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var codeTemplate = DataBindingCommonData.EditorDisplayText.DefaultDataSourceTemplate;
            var code = codeTemplate.Replace("{name}", name);

            var dataSourceSourceFilename = $"{name}DataSource.cs";
            var path = Path.Combine(directory, dataSourceSourceFilename);

            if (!File.Exists(path))
            {
                File.WriteAllText(path, code);
                OpenDataSourceEditor(path);
            }
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

        private static string GetSelectedDirectoryOrFallback()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);

            if (string.IsNullOrEmpty(path))
            {
                return Path.Combine(Application.dataPath, "DataSource");
            }

            if (Path.HasExtension(path))
            {
                path = Path.GetDirectoryName(path);
            }

            return path;
        }
    }
}
