using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace JH.DataBinding.Editor
{
    public class ViewEditorBase : UnityEditor.Editor
    {
        internal static readonly IDataBindingEditorDisplayText EditorDisplayText =
            new DataBindingEditorDisplayText();

        protected VisualElement EditorRootElement { get; private set; }

        protected Type[] ValidDataSources { get; private set; }

        public virtual void OnEnable()
        {
            ValidDataSources ??= GetValidDataSourceTypes();
        }

        protected void InitDatabindingEditorRootElement()
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

            if (target.GetType() != typeof(View))
            {
                var defaultInspector = new VisualElement();

                InspectorElement.FillDefaultInspector(defaultInspector, serializedObject, this);
                EditorRootElement.Add(defaultInspector);
            }
        }

        protected static VisualElement MakeErrorReport(Exception exception)
        {
            var errorReport = new VisualElement();

            var errorLabel = new TextElement();
            errorLabel.text = EditorDisplayText.EditorErrorMessageText;
            DataBindingEditorStyles.ErrorMessageStyle(errorLabel);
            errorReport.Add(errorLabel);

            var message = new TextElement();
            message.text = exception.Message;
            DataBindingEditorStyles.ErrorMessageStyle(message);
            errorReport.Add(message);

            var reportBugLink = new Button(() => HandleReportError(exception));
            reportBugLink.text = EditorDisplayText.ReportErrorButtonText;
            DataBindingEditorStyles.ErrorButtonStyle(reportBugLink);
            errorReport.Add(reportBugLink);

            return errorReport;
        }

        private static void HandleReportError(Exception exception)
        {
            var link =
                $"https://github.com/JochenHeckl/DataBinding/issues/new?assignees=&labels=&template=bug_report.md&title={exception.Message}";

            Application.OpenURL(link);
        }

        protected VisualElement MakeDataSourceSection(
            SerializableType currentDataSourceType,
            Action<Type> handleDataSourceTypeChanged
        )
        {
            if (ValidDataSources.Length == 0)
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

                DataBindingEditorStyles.DataSourceStyle(dataSource);

                return dataSource;
            }
        }

        protected VisualElement MakeSubViews(
            GameObject subVies, Action<GameObject> onValueChanged
        )
        {
            var view = new VisualElement();
            view.userData = subVies;
            view.AddToClassList(DataBindingEditorStyles.GenericRow);
            
            var visualElement = new ObjectField("Sub Views")
            {
                objectType = typeof(GameObject),
                value = subVies
            };
            visualElement.RegisterValueChangedCallback(_ =>
            {
                onValueChanged(visualElement.value as GameObject);
            });

            view.Add(
                visualElement
            );

            return view;
        }

        protected static VisualElement MakeBindingSection<BindingType>(
            string sectionHeaderText,
            Action handleAddBinding,
            IEnumerable<BindingType> bindings,
            Func<BindingType, VisualElement> makeEditorVisualElement
        )
        {
            var sectionRoot = new VisualElement();
            sectionRoot.AddToClassList(DataBindingEditorStyles.bindingGroup);

            var header = new VisualElement();
            header.AddToClassList(DataBindingEditorStyles.bindingGroupHeader);

            var headerLabel = new Label(sectionHeaderText);
            headerLabel.AddToClassList(DataBindingEditorStyles.bindingGroupLabel);
            header.Add(headerLabel);

            var addBindingButton = new Button(handleAddBinding) { text = "Add Binding" };

            addBindingButton.AddToClassList(DataBindingEditorStyles.addBindingActionButton);
            header.Add(addBindingButton);

            sectionRoot.Add(header);

            var bindingsGroup = new VisualElement();
            bindingsGroup.AddToClassList(DataBindingEditorStyles.bindingGroupList);

            foreach (var binding in bindings)
            {
                bindingsGroup.Add(makeEditorVisualElement(binding));
            }

            if (!bindingsGroup.Children().Any())
            {
                bindingsGroup.Add(new Label($"There are no bindings in this section."));
            }

            sectionRoot.Add(bindingsGroup);

            return sectionRoot;
        }

        protected static IEnumerable<ElementType> MoveElementUp<ElementType>(
            IEnumerable<ElementType> sequence,
            ElementType element
        )
            where ElementType : class
        {
            return MoveElementDown(sequence.Reverse(), element).Reverse();
        }

        protected static IEnumerable<ElementType> MoveElementDown<ElementType>(
            IEnumerable<ElementType> sequence,
            ElementType element
        )
            where ElementType : class
        {
            if (sequence == null)
            {
                throw new ArgumentNullException(nameof(sequence));
            }

            var rearrangedSequence = sequence as ElementType[] ?? sequence.ToArray();

            for (var elementIndex = 0; elementIndex < rearrangedSequence.Length; ++elementIndex)
            {
                if (rearrangedSequence[elementIndex] == element)
                {
                    if (elementIndex < (rearrangedSequence.Length - 1))
                    {
                        (rearrangedSequence[elementIndex], rearrangedSequence[elementIndex + 1]) = (
                            rearrangedSequence[elementIndex + 1],
                            rearrangedSequence[elementIndex]
                        );

                        break;
                    }
                }
            }

            return rearrangedSequence;
        }

        private static Type[] GetValidDataSourceTypes()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            return assemblies
                .Where(x => !x.IsDynamic)
                .SelectMany(x => x.ExportedTypes)
                .Where(DataSourceFilterFunc)
                .ToArray();

            bool DataSourceFilterFunc(Type x) =>
                !x.IsAbstract
                && !x.IsGenericType
                && !x.IsInterface
                && x.InheritsOrImplements(typeof(INotifyDataSourceChanged));
        }

        private VisualElement MakeDataSourceDropDown(
            SerializableType dataSourceType,
            Action<Type> handleDataSourceTypeChanged
        )
        {
            var dataSourceDropDown = new DropdownField(
                label: EditorDisplayText.DataSourceTypeText,
                choices: ValidDataSources.Select(x => x.GetFriendlyName()).ToList(),
                defaultValue: ValidDataSources
                    .FirstOrDefault(x => x == dataSourceType.Type)
                    ?.GetFriendlyName()
            );

            dataSourceDropDown.AddToClassList(DataBindingEditorStyles.bindingDataSourceTypeLabel);
            dataSourceDropDown.RegisterValueChangedCallback(
                (changeEvent) =>
                {
                    var newDataSourceType = ValidDataSources.FirstOrDefault(x =>
                        x.GetFriendlyName() == changeEvent.newValue
                    );
                    handleDataSourceTypeChanged(newDataSourceType);
                }
            );

            return dataSourceDropDown;
        }

        protected static Type GuessDataSourceTypeName(string viewName, Type[] validDataSources)
        {
            return validDataSources
                .Select(x => new { x, distance = viewName.DamerauLevenshteinDistance(x.Name) })
                .OrderBy(x => x.distance)
                .Select(x => x.x)
                .FirstOrDefault();
        }

        public static string GuessBinding(string match, string[] pool)
        {
            return pool.Select(x => new { x, distance = match.DamerauLevenshteinDistance(x) })
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
            var dataSourceSourceFile = FindDataSourceSourceFile(dataSourceType.Type);

            var button = new Button(() => OpenDataSourceEditor(dataSourceSourceFile))
            {
                text = EditorDisplayText.EditSourceText,
                tooltip = EditorDisplayText.NoSourceCodeAvailableToolTip
            };

            button.SetEnabled(dataSourceSourceFile != null);
            return button;
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

        private static string FindDataSourceSourceFile(Type type)
        {
            var assetGuid = AssetDatabase.FindAssets(type.Name).FirstOrDefault();

            if (assetGuid == null)
            {
                var allSourceFiles = Directory.GetFiles(
                    Application.dataPath,
                    "*.cs",
                    SearchOption.AllDirectories
                );

                var sourceFile = allSourceFiles.FirstOrDefault(x =>
                    File.ReadAllText(x).Contains($"class {type.Name}")
                );

                var assetName = Path.GetFileNameWithoutExtension(sourceFile);

                assetGuid = AssetDatabase.FindAssets(assetName).FirstOrDefault();
            }

            if (assetGuid != null)
            {
                return AssetDatabase.GUIDToAssetPath(assetGuid);
            }

            return null;
        }
    }
}