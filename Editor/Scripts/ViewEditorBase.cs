using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;
using UnityEditor;
using static UnityEngine.UI.InputField;

namespace de.JochenHeckl.Unity.DataBinding.Editor
{
    public class ViewEditorBase : UnityEditor.Editor
    {
        internal static IDataBindingEditorDisplayText EditorDisplayText =
            new DataBindingEditorDisplayText();

        public VisualElement EditorRootElement { get; set; }
        public Type[] ValidDataSources { get; set; }

        public ViewEditorBase() { }

        public virtual void OnEnable()
        {
            if (ValidDataSources == null)
            {
                ValidDataSources = GetValidDataSourceTypes();
            }
        }

        public void InitDataBindingEditorRootElement()
        {
            if (EditorRootElement == null)
            {
                EditorRootElement = new();

                EditorRootElement.styleSheets.Add(DataBindingEditorStyles.StyleSheet);
                EditorRootElement.AddToClassList(DataBindingEditorStyles.viewEditorClassName);
            }
            else
            {
                EditorRootElement.Clear();
            }
        }

        public VisualElement MakeErrorReport(Exception exception)
        {
            var errorReport = new VisualElement();

            var errorLabel = new Label(EditorDisplayText.EditorErrorMessageText);
            errorLabel.AddToClassList(DataBindingEditorStyles.ErrorText);
            errorReport.Add(errorLabel);

            var reportBugLink = new Button(() => HandleReportError(exception));
            errorLabel.AddToClassList(DataBindingEditorStyles.ErrorText);
            errorLabel.Add(reportBugLink);

            var message = new Label(exception.Message);
            errorReport.Add(message);

            return errorReport;
        }

        private static void HandleReportError(Exception exception)
        {
            var link =
                $"https://github.com/JochenHeckl/DataBinding/issues/new?assignees=&labels=&template=bug_report.md&title={exception.Message}";

            Application.OpenURL(link);
        }

        public VisualElement MakeDataSourceSection(
            Type[] validDataSources,
            SerializableType currentDataSourceType,
            Action<Type> handleDataSourceTypeChanged
        )
        {
            if (validDataSources.Length == 0)
            {
                return MakeMissingDataSourceLabel();
            }
            else
            {
                var dataSource = new VisualElement();
                dataSource.AddToClassList(DataBindingEditorStyles.GenericRow);

                dataSource.Add(
                    MakeDataSourceDropDown(currentDataSourceType, handleDataSourceTypeChanged)
                );

                if (currentDataSourceType.Type != null)
                {
                    dataSource.Add(MakeOpenDataSourceButton(currentDataSourceType));
                }

                return dataSource;
            }
        }

        public static VisualElement MakeBindingSection<BindingType>(
            string sectionHeaderText,
            Action handleAddBinding,
            IEnumerable<BindingType> bindings,
            Func<BindingType, VisualElement> MakeEditorVisualElement
        )
        {
            var sectionRoot = new VisualElement();
            sectionRoot.AddToClassList(DataBindingEditorStyles.bindingGroup);

            var header = new VisualElement();
            header.AddToClassList(DataBindingEditorStyles.bindingGroupHeader);

            var headerLabel = new Label(sectionHeaderText);
            headerLabel.AddToClassList(DataBindingEditorStyles.bindingGroupLabel);
            header.Add(headerLabel);

            var addBindingButton = new Button(handleAddBinding);
            addBindingButton.text = "Add Binding";
            addBindingButton.AddToClassList(DataBindingEditorStyles.bindingActionButton);
            header.Add(addBindingButton);

            sectionRoot.Add(header);

            var bindingsGroup = new VisualElement();
            bindingsGroup.AddToClassList(DataBindingEditorStyles.bindingGroupList);

            if (!bindings.Any())
            {
                bindingsGroup.Add(new Label($"There are no bindings in this secion."));
            }
            else
            {
                foreach (var binding in bindings)
                {
                    bindingsGroup.Add(MakeEditorVisualElement(binding));
                }
            }

            sectionRoot.Add(bindingsGroup);

            return sectionRoot;
        }

        public static IEnumerable<ElementType> MoveElementUp<ElementType>(
            IEnumerable<ElementType> sequence,
            ElementType element
        ) where ElementType : class
        {
            return MoveElementDown(sequence.Reverse(), element).Reverse();
        }

        public static IEnumerable<ElementType> MoveElementDown<ElementType>(
            IEnumerable<ElementType> sequence,
            ElementType element
        ) where ElementType : class
        {
            for (var elementIndex = 0; elementIndex < sequence.Count(); ++elementIndex)
            {
                var currentElement = sequence.ElementAt(elementIndex);

                if (currentElement == element)
                {
                    if (currentElement != sequence.Last())
                    {
                        yield return sequence.ElementAt(elementIndex + 1);
                        ++elementIndex;
                    }
                }

                yield return currentElement;
            }
        }

        public static Type[] GetValidDataSourceTypes()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            Func<Type, bool> dataSourceFilterFunc = (x) =>
                !x.IsAbstract
                && !x.IsGenericType
                && !x.IsInterface
                && x.InheritsOrImplements(typeof(INotifyDataSourceChanged));

            return assemblies
                .Where(x => !x.IsDynamic)
                .SelectMany(x => x.ExportedTypes)
                .Where(dataSourceFilterFunc)
                .ToArray();
        }

        private VisualElement MakeDataSourceDropDown(
            SerializableType dataSourceType,
            Action<Type> handleDataSourceTypeChanged
        )
        {
            var dataSourceDropDown = new DropdownField(
                label: EditorDisplayText.DataSourceTypeText,
                choices: ValidDataSources.Select(x => x.GetFriendlyName()).ToList(),
                defaultValue: ValidDataSources.FirstOrDefault(x => x == dataSourceType.Type)?.Name
            );

            dataSourceDropDown.AddToClassList(DataBindingEditorStyles.bindingDataSourceTypeLabel);
            dataSourceDropDown.RegisterValueChangedCallback(
                (changeEvent) =>
                {
                    var newDataSourceType = ValidDataSources.FirstOrDefault(
                        x => x.Name == changeEvent.newValue
                    );
                    handleDataSourceTypeChanged(newDataSourceType);
                }
            );

            return dataSourceDropDown;
        }

        public static Type GuessDataSourceTypeName(string viewName, Type[] validDataSources)
        {
            return validDataSources
                .Select(x => new { x, distance = viewName.DamerauLevenshteinDistance(x.Name) })
                .OrderBy(x => x.distance)
                .Select(x => x.x)
                .FirstOrDefault();
        }

        private static VisualElement MakeMissingDataSourceLabel()
        {
            var labelContainer = new Box();

            var textErrorContent = new Label(EditorDisplayText.MissingDataSourcesErrorText);
            textErrorContent.AddToClassList(DataBindingEditorStyles.ErrorText);
            labelContainer.Add(textErrorContent);

            var textInfoContent = new Label(EditorDisplayText.MissingDataSourcesInfoText);
            textInfoContent.AddToClassList(DataBindingEditorStyles.InfoText);
            labelContainer.Add(textInfoContent);

            return labelContainer;
        }

        private static VisualElement MakeOpenDataSourceButton(SerializableType dataSourceType)
        {
            var dataSourceSourceAsset = FindDataSourceSourceAsset(dataSourceType);

            var button = new UnityEngine.UIElements.Button(
                () => OpenDataSourceEditor(dataSourceType)
            );

            button.text = EditorDisplayText.EditSourceText;
            button.tooltip = EditorDisplayText.NoSourceCodeAvailableToolTip;

            button.SetEnabled(dataSourceSourceAsset != null);
            return button;
        }

        private static void OpenDataSourceEditor(SerializableType dataSourceType)
        {
            if (dataSourceType.Type != null)
            {
                var sourceFile = AssetDatabase
                    .FindAssets(dataSourceType.Type.Name)
                    .FirstOrDefault();

                if (sourceFile != null)
                {
                    var scriptAsset = AssetDatabase.LoadAssetAtPath<MonoScript>(
                        AssetDatabase.GUIDToAssetPath(sourceFile)
                    );

                    if (scriptAsset != null)
                    {
                        AssetDatabase.OpenAsset(scriptAsset);
                        return;
                    }
                }
            }
        }

        private static MonoScript FindDataSourceSourceAsset(SerializableType dataSourceType)
        {
            if (dataSourceType.Type != null)
            {
                var sourceFile = AssetDatabase
                    .FindAssets(dataSourceType.Type.Name)
                    .FirstOrDefault();

                if (sourceFile != null)
                {
                    var scriptAsset = AssetDatabase.LoadAssetAtPath<MonoScript>(
                        AssetDatabase.GUIDToAssetPath(sourceFile)
                    );

                    return scriptAsset;
                }
            }

            return null;
        }
    }
}
