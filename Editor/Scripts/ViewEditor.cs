using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace JH.DataBinding.Editor
{
    [CustomEditor(typeof(View), true)]
    public class ViewEditor : UnityEditor.Editor
    {
        private View view;
        private VisualElement editorRootElement;

        public virtual void OnEnable()
        {
            view = target as View;

            if (view.dataSourceType?.Type == null)
            {
                var guessedDataSourceType = DataBindingCommonData.GuessDataSourceTypeName(
                    view.name
                );

                if (guessedDataSourceType != null)
                {
                    Debug.Log(
                        $"Guessing {guessedDataSourceType.Name} as data source type for view {view.name}."
                    );

                    HandleDataSourceTypeChanged(guessedDataSourceType);
                }
            }
        }

        public override VisualElement CreateInspectorGUI()
        {
            editorRootElement = new VisualElement();
            editorRootElement.styleSheets.Add(DataBindingEditorStyle.StyleSheet);

            FillInRoot(editorRootElement);

            editorRootElement.TrackSerializedObjectValue(serializedObject, HandleObjectChanged);

            return editorRootElement;
        }

        private void FillInRoot(VisualElement root)
        {
            try
            {
                root.Clear();

                var dataSourceSelection = new VisualElement();
                FillInDataSourceSelection(dataSourceSelection);
                root.Add(dataSourceSelection);

                var componentPropertyBindings = new ComponentPropertyBindingListView();
                componentPropertyBindings.Bind(serializedObject);
                root.Add(componentPropertyBindings);

                var containerPropertyBindings = new ContainerPropertyBindingListView();
                containerPropertyBindings.Bind(serializedObject);
                root.Add(containerPropertyBindings);
            }
            catch (Exception exception)
            {
                Debug.LogError(exception.Message);
                root.Add(new InternalErrorReport(exception));
            }
        }

        private void FillInDataSourceSelection(VisualElement dataSourceSelectionRoot)
        {
            var validDataSources = DataBindingCommonData.GetValidDataSourceTypes();

            var filteredDataSources = validDataSources
                .Where(x =>
                    x.GetFriendlyName()
                        .IndexOf(
                            DataBindingCommonData.dataSourceTypeInspectorFilter,
                            StringComparison.OrdinalIgnoreCase
                        ) != -1
                )
                .ToArray();

            if (validDataSources.Length == 0)
            {
                FillInMissingDataSource(dataSourceSelectionRoot);
                return;
            }

            FillInDataSourceSelectionFilterSection(dataSourceSelectionRoot);

            FillInDataSourceSelectionSelectionSection(dataSourceSelectionRoot, filteredDataSources);
        }

        private void FillInDataSourceSelectionFilterSection(VisualElement dataSourceSelectionRoot)
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

            filterInput.RegisterCallback<FocusOutEvent>(_ =>
            {
                if (DataBindingCommonData.dataSourceTypeInspectorFilter != filterInput.value)
                {
                    DataBindingCommonData.dataSourceTypeInspectorFilter = filterInput.value;
                    FillInRoot(editorRootElement);
                }
            });

            filterInput.RegisterCallback<KeyDownEvent>(keyDown =>
            {
                if (keyDown.keyCode == KeyCode.Return || keyDown.keyCode == KeyCode.KeypadEnter)
                {
                    DataBindingCommonData.dataSourceTypeInspectorFilter = filterInput.value;
                    FillInRoot(editorRootElement);
                }
            });

            filterInput.AddToClassList(DataBindingEditorStyle.dataSourceFilterInput);
            filterSection.Add(filterInput);

            var buttonGroup = new VisualElement();
            buttonGroup.AddToClassList(DataBindingEditorStyle.dataSourceSelectionButtonGroup);

            var newButton = new Button(
                () => DataBindingEditorOperations.CreateNewDataSource(view.name)
            )
            {
                text = DataBindingCommonData.EditorDisplayText.NewDataSourceText,
                tooltip = DataBindingCommonData.EditorDisplayText.NewDataSourceTooltip,
            };

            newButton.AddToClassList(DataBindingEditorStyle.dataSourceSelectionButton);

            buttonGroup.Add(newButton);

            filterSection.Add(buttonGroup);

            dataSourceSelectionRoot.Add(filterSection);
        }

        private void FillInDataSourceSelectionSelectionSection(
            VisualElement dataSourceSelectionRoot,
            Type[] validDataSources
        )
        {
            var selectionSection = new VisualElement();
            selectionSection.AddToClassList(
                DataBindingEditorStyle.dataSourceSelectionSelectionSection
            );

            var choises = validDataSources.Select(x => x.GetFriendlyName()).ToList();
            var currentValue = view.dataSourceType.Type.GetFriendlyName();
            var filterConditionedDefault = choises.Contains(currentValue)
                ? currentValue
                : choises.First();

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
                HandleDataSourceTypeChanged(
                    validDataSources.FirstOrDefault(x => x.Name == changeEvent.newValue)
                );
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

        private void FillInMissingDataSource(VisualElement dataSourceSelectionRoot)
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
                DataBindingEditorOperations.CreateNewDataSource(view.name);
                HandleDataSourceTypeChanged(null);
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

        private void HandleObjectChanged(SerializedObject serializedObject)
        {
            serializedObject.ApplyModifiedProperties();
            FillInRoot(editorRootElement);
        }

        private void HandleDataSourceTypeChanged(Type newType)
        {
            if (view.dataSourceType.Type != newType)
            {
                EditorUtility.SetDirty(view);
                view.dataSourceType.Type = newType;
                serializedObject.ApplyModifiedProperties();

                FillInRoot(editorRootElement);
            }
        }
    }
}
