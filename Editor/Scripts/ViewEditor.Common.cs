using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace de.JochenHeckl.Unity.DataBinding.Editor
{
    internal static class ViewEditorCommon
    {
        public static VisualElement MakeBindingSection<BindingType>(
            string sectionHeaderText,
            Action handleAddBinding,
            IEnumerable<BindingType> bindings,
            Func<BindingType, VisualElement> MakeEditorVisualElement)
        {
            var sectionRoot = new VisualElement();
            sectionRoot.AddToClassList(DataBindingEditorStyles.bindingGroupClassName);

            var header = new VisualElement();
            header.AddToClassList(DataBindingEditorStyles.bindingGroupHeaderClassName);

            var headerLabel = new Label(sectionHeaderText);
            headerLabel.AddToClassList(DataBindingEditorStyles.bindingGroupLabelClassName);
            header.Add(headerLabel);

            var addBindingButton = new Button(handleAddBinding);
            addBindingButton.text = "Add Binding";
            addBindingButton.AddToClassList(DataBindingEditorStyles.bindingActionButtonClassName);
            header.Add(addBindingButton);

            sectionRoot.Add(header);

            var bindingsGroup = new VisualElement();
            bindingsGroup.AddToClassList(DataBindingEditorStyles.bindingGroupListClassName);

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
           ElementType element)
           where ElementType : class
        {
            return MoveElementDown(sequence.Reverse(), element).Reverse();
        }

        public static IEnumerable<ElementType> MoveElementDown<ElementType>(
            IEnumerable<ElementType> sequence,
            ElementType element)
            where ElementType : class
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
    }
}
