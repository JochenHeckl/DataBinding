using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace JH.DataBinding.Editor
{
    [CustomPropertyDrawer(typeof(DataSourceTypeAttribute))]
    public class DataSourceTypePropertyDrawer : PropertyDrawer
    {
        internal static readonly IDataBindingEditorDisplayText EditorDisplayText =
            new DataBindingEditorDisplayText();

        private VisualElement rootVisualElement;
        private VisualElement dataSourceSelectionRoot;
        private PropertyField propertyField;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            rootVisualElement = new();
            FillInRoot(rootVisualElement, property);
            return rootVisualElement;
        }

        private void FillInRoot(VisualElement root, SerializedProperty property)
        {
            root.Clear();

            var validDataSources = DataBindingCommonData.GetValidDataSourceTypes();

            if (validDataSources.Length == 0)
            {
                FillInMissingDataSource(root, property);
                return;
            }

            var filteredDataSources = validDataSources
                .Where(x =>
                    x.GetFriendlyName()
                        .IndexOf(
                            DataBindingCommonData.dataSourceTypeInspectorFilter,
                            StringComparison.OrdinalIgnoreCase
                        ) != -1
                )
                .ToArray();

            FillInDataSourceSelectionFilterSection(property);

            dataSourceSelectionRoot = new VisualElement();
            root.Add(dataSourceSelectionRoot);

            FillInDataSourceSelectionSelectionSection(filteredDataSources, property);
        }

        private void FillInMissingDataSource(
            VisualElement dataSourceSelectionRoot,
            SerializedProperty property
        )
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
                var dataSourceName = property.serializedObject.targetObject.name;
                DataBindingEditorOperations.CreateNewDataSource(dataSourceName);

                EditorUtility.SetDirty(property.serializedObject.targetObject);
            });

            createDataSourceButton.text = DataBindingCommonData
                .EditorDisplayText
                .CreateDefaultDataSourceText;

            createDataSourceButton.AddToClassList(
                DataBindingEditorStyle.createDefaultDataSourceButton
            );

            labelContainer.Add(createDataSourceButton);

            dataSourceSelectionRoot.Add(labelContainer);
        }

        private void FillInDataSourceSelectionFilterSection(SerializedProperty property)
        {
            var filterSection = new VisualElement();
            filterSection.AddToClassList(DataBindingEditorStyle.dataSourceSelectionFilterSection);

            var filterInput = new TextField(
                DataBindingCommonData.EditorDisplayText.DataSourceTypeInspectorFilterLabel
            )
            {
                value = DataBindingCommonData.dataSourceTypeInspectorFilter,
            };

            filterInput.AddToClassList("unity-base-field__aligned");

            // filterInput.RegisterCallback<FocusOutEvent>(_ =>
            filterInput.RegisterValueChangedCallback(_ =>
            {
                if (DataBindingCommonData.dataSourceTypeInspectorFilter != filterInput.value)
                {
                    DataBindingCommonData.dataSourceTypeInspectorFilter = filterInput.value;

                    var filteredDataSources = DataBindingCommonData
                        .GetValidDataSourceTypes()
                        .Where(x =>
                            x.GetFriendlyName()
                                .IndexOf(
                                    DataBindingCommonData.dataSourceTypeInspectorFilter,
                                    StringComparison.OrdinalIgnoreCase
                                ) != -1
                        )
                        .ToArray();

                    FillInDataSourceSelectionSelectionSection(filteredDataSources, property);
                }
            });

            filterInput.AddToClassList(DataBindingEditorStyle.dataSourceFilterInput);
            filterSection.Add(filterInput);

            var buttonGroup = new VisualElement();
            buttonGroup.AddToClassList(DataBindingEditorStyle.dataSourceSelectionButtonGroup);

            var newButton = new Button(() =>
            {
                var dataSourceName = property.serializedObject.targetObject.name;
                DataBindingEditorOperations.CreateNewDataSource(dataSourceName);
            })
            {
                text = DataBindingCommonData.EditorDisplayText.NewDataSourceText,
                tooltip = DataBindingCommonData.EditorDisplayText.NewDataSourceTooltip,
            };

            newButton.AddToClassList(DataBindingEditorStyle.dataSourceSelectionButton);

            buttonGroup.Add(newButton);

            filterSection.Add(buttonGroup);

            rootVisualElement.Add(filterSection);
        }

        private void FillInDataSourceSelectionSelectionSection(
            Type[] validDataSources,
            SerializedProperty property
        )
        {
            dataSourceSelectionRoot.Clear();

            if (validDataSources.Length == 0)
            {
                var label = new Label(DataBindingCommonData.EditorDisplayText.NoDataSourcesText);
                label.AddToClassList(DataBindingEditorStyle.errorMessageContainer);

                dataSourceSelectionRoot.Add(label);
                return;
            }

            var selectionSection = new VisualElement();
            selectionSection.AddToClassList(
                DataBindingEditorStyle.dataSourceSelectionSelectionSection
            );

            var choises = validDataSources.Select(x => x.GetFriendlyName()).ToList();

            var view = property.serializedObject.targetObject as View;
            var currentValue = view.dataSourceType.Type.GetFriendlyName();
            var filterConditionedDefault = choises.Contains(currentValue)
                ? currentValue
                : choises.FirstOrDefault();

            var dataSourceDropDown = new DropdownField(
                label: DataBindingCommonData.EditorDisplayText.DataSourceTypeText,
                choices: choises,
                defaultValue: filterConditionedDefault
            );

            // dataSourceDropDown.ClearClassList();
            dataSourceDropDown.AddToClassList("unity-base-field__aligned");
            dataSourceDropDown.AddToClassList(
                DataBindingEditorStyle.dataSourceSelectionDataSourceDropDown
            );

            dataSourceDropDown.RegisterValueChangedCallback(changeEvent =>
            {
                var newDataSourceType = validDataSources.FirstOrDefault(x =>
                    x.Name == changeEvent.newValue
                );

                if (newDataSourceType != null)
                {
                    var view = property.serializedObject.targetObject as View;

                    if (view.dataSourceType.Type != newDataSourceType)
                    {
                        view.dataSourceType.Type = newDataSourceType;
                        property.serializedObject.ApplyModifiedProperties();
                        property.serializedObject.Update();
                        EditorUtility.SetDirty(view);
                    }
                }
            });

            selectionSection.Add(dataSourceDropDown);

            var buttonGroup = new VisualElement();
            buttonGroup.AddToClassList(DataBindingEditorStyle.dataSourceSelectionButtonGroup);

            var dataSourceSourceFile = DataBindingCommonData.FindDataSourceSourceFile(
                view.dataSourceType.Type
            );

            var editButton = new Button(
                () => DataBindingEditorOperations.OpenDataSourceEditor(dataSourceSourceFile)
            )
            {
                text = DataBindingCommonData.EditorDisplayText.EditDataSourceText,
                tooltip = DataBindingCommonData.EditorDisplayText.EditDataSourceTooltip,
            };

            editButton.AddToClassList(DataBindingEditorStyle.dataSourceSelectionButton);

            editButton.SetEnabled(File.Exists(dataSourceSourceFile));

            buttonGroup.Add(editButton);

            selectionSection.Add(buttonGroup);

            dataSourceSelectionRoot.Add(selectionSection);
        }
    }
}
