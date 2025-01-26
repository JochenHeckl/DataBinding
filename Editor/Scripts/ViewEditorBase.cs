using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace JH.DataBinding.Editor
{
    public class ViewEditorBase : UnityEditor.Editor
    {
        protected Type[] ValidDataSources { get; private set; }

        public virtual void OnEnable()
        {
            ValidDataSources ??= DataBindingCommonData.GetValidDataSourceTypes();
        }

        protected static VisualElement MakeErrorReport(Exception exception)
        {
            var errorReport = new VisualElement();

            var errorLabel = new TextElement();
            errorLabel.text = DataBindingCommonData.EditorDisplayText.EditorErrorMessageText;
            DataBindingEditorStyles.ErrorMessageStyle(errorLabel);
            errorReport.Add(errorLabel);

            var message = new TextElement();
            message.text = exception.Message;
            DataBindingEditorStyles.ErrorMessageStyle(message);
            errorReport.Add(message);

            var reportBugLink = new Button(() => HandleReportError(exception));
            reportBugLink.text = DataBindingCommonData.EditorDisplayText.ReportErrorButtonText;
            DataBindingEditorStyles.ErrorButtonStyle(reportBugLink);
            errorReport.Add(reportBugLink);

            return errorReport;
        }

        private static void HandleReportError(Exception exception)
        {
            // TODO: Check for comiler errors before reporting to github.

            var link =
                $"https://github.com/JochenHeckl/DataBinding/issues/new?assignees=&labels=&template=bug_report.md&title={exception.Message}";

            Application.OpenURL(link);
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

            // var addBindingButton = new Button(handleAddBinding) { text = "Add Binding" };
            // addBindingButton.AddToClassList(DataBindingEditorStyles.addBindingActionButton);
            // header.Add(addBindingButton);

            sectionRoot.Add(header);

            var listView = new ListView(
                bindings.ToArray(),
                -1,
                MakePropertyBindingEditor,
                (visualElement, index) =>
                {
                    visualElement.Clear();
                    visualElement.Add(makeEditorVisualElement(bindings.ElementAt(index)));
                }
            );

            listView.allowAdd = true;
            listView.allowRemove = true;

            listView.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
            listView.showFoldoutHeader = false;
            listView.showAddRemoveFooter = true;
            listView.reorderMode = ListViewReorderMode.Animated;
            listView.pickingMode = PickingMode.Position;
            listView.reorderable = true;

            listView.onAdd = x =>
            {
                handleAddBinding();
                x.RefreshItems();
            };

            sectionRoot.Add(listView);

            // var bindingsGroup = new VisualElement();
            // bindingsGroup.AddToClassList(DataBindingEditorStyles.bindingGroupList);

            // foreach (var binding in bindings)
            // {
            //     bindingsGroup.Add(makeEditorVisualElement(binding));
            // }

            // if (!bindingsGroup.Children().Any())
            // {
            //     bindingsGroup.Add(new Label($"There are no bindings in this section."));
            // }

            // sectionRoot.Add(bindingsGroup);

            return sectionRoot;
        }

        private static VisualElement MakePropertyBindingEditor()
        {
            return new VisualElement();
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

        private VisualElement MakeDataSourceDropDown(
            SerializableType dataSourceType,
            Action<Type> handleDataSourceTypeChanged
        )
        {
            var dataSourceDropDown = new DropdownField(
                label: DataBindingCommonData.EditorDisplayText.DataSourceTypeText,
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
    }
}
