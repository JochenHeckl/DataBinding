using System;
using System.Linq;

using UnityEditor;

using UnityEngine.UIElements;

namespace de.JochenHeckl.Unity.DataBinding.Editor
{
    [CustomEditor(typeof(View), true)]
    public class ViewEditor : ViewEditorBase
    {
        private View view;
        private static Type[] validDataSources;

        public override void OnEnable()
        {
            base.OnEnable();

            view = target as View;

            if (view.dataSourceType.Type == null)
            {
                view.dataSourceType.Type = GuessDataSourceTypeName(view.name, ValidDataSources);
            }
        }

        public override VisualElement CreateInspectorGUI()
        {
            MakeEditorView();

            return EditorRootElement;
        }

        private void MakeEditorView()
        {
            try
            {
                InitDataBindingEditorRootElement();

                EditorRootElement.Add(
                    MakeDataSourceSection(
                        ValidDataSources,
                        view.dataSourceType,
                        HandleDataSourceTypeChanged
                    )
                );

                //editorRootElement.Add(MakeVisualElementPropertyBindings());

                //var dataSourceTypeDropDownField = new DropdownField(
                //    label: "DataSource Type",
                //    choices: validDataSources.Select(x => x.GetFriendlyName()).ToList(),
                //    defaultValue: validDataSources
                //        .FirstOrDefault(x => x == view.dataSourceType.Type)
                //        ?.Name ?? ""
                //);

                //dataSourceTypeDropDownField.AddToClassList(
                //    DataBindingEditorStyles.bindingDataSourceTypeLabel
                //);
                //dataSourceTypeDropDownField.RegisterValueChangedCallback(
                //    HandleDataSourceTypeChanged
                //);
                //editorRootElement.Add(dataSourceTypeDropDownField);

                //editorRootElement.Add(MakeComponentPropertyBindings());
                //editorRootElement.Add(MakeContainerPropertyBindings());
            }
            catch (Exception exception)
            {
                EditorRootElement.Clear();
                EditorRootElement.Add(MakeErrorReport(exception));
            }
        }

        private VisualElement MakeComponentPropertyBindings()
        {
            return MakeBindingSection(
                "Component Property Bindings",
                HandleAddComponentPropertyBinding,
                view.componentPropertyBindings,
                MakeComponentPropertyBindingVisualElement
            );
        }

        private void HandleAddComponentPropertyBinding()
        {
            view.componentPropertyBindings = view.componentPropertyBindings
                .Append(new ComponentPropertyBinding() { DataSource = view.DataSource })
                .ToArray();

            StoreAndUpdateView();
        }

        private VisualElement MakeComponentPropertyBindingVisualElement(
            ComponentPropertyBinding binding
        )
        {
            return new ComponentPropertyBindingEditor(
                view.dataSourceType.Type,
                binding,
                StoreAndUpdateView,
                binding => binding.showExpanded,
                HandleMoveBindingUp,
                HandleMoveBindingDown,
                HandleTogglePropertyExpansion,
                HandleRemoveBinding
            );
        }

        private void HandleMoveBindingUp(ComponentPropertyBinding binding)
        {
            view.componentPropertyBindings = MoveElementUp(view.componentPropertyBindings, binding)
                .ToArray();

            StoreAndUpdateView();
        }

        private void HandleMoveBindingDown(ComponentPropertyBinding binding)
        {
            view.componentPropertyBindings = MoveElementDown(
                    view.componentPropertyBindings,
                    binding
                )
                .ToArray();

            StoreAndUpdateView();
        }

        private void HandleTogglePropertyExpansion(ComponentPropertyBinding binding)
        {
            binding.showExpanded = !binding.showExpanded;
            StoreAndUpdateView();
        }

        private void HandleRemoveBinding(ComponentPropertyBinding binding)
        {
            view.componentPropertyBindings = view.componentPropertyBindings
                .Where(x => x != binding)
                .ToArray();

            StoreAndUpdateView();
        }

        private VisualElement MakeContainerPropertyBindings()
        {
            return MakeBindingSection(
                "Container Property Bindings",
                HandleAddContainerPropertyBinding,
                view.containerPropertyBindings,
                MakeContainerPropertyBindingVisualElement
            );
        }

        private void HandleAddContainerPropertyBinding()
        {
            view.containerPropertyBindings = view.containerPropertyBindings
                .Append(new ContainerPropertyBinding() { DataSource = view.DataSource })
                .ToArray();

            StoreAndUpdateView();
        }

        private VisualElement MakeContainerPropertyBindingVisualElement(
            ContainerPropertyBinding binding
        )
        {
            return new ContainerPropertyBindingEditor(
                view.dataSourceType.Type,
                binding,
                StoreAndUpdateView,
                binding => binding.showExpanded,
                HandleMoveBindingUp,
                HandleMoveBindingDown,
                HandleTogglePropertyExpansion,
                HandleRemoveBinding
            );
        }

        private void HandleMoveBindingUp(ContainerPropertyBinding binding)
        {
            MoveElementUp(view.containerPropertyBindings, binding);

            StoreAndUpdateView();
        }

        private void HandleMoveBindingDown(ContainerPropertyBinding binding)
        {
            MoveElementDown(view.containerPropertyBindings, binding);

            StoreAndUpdateView();
        }

        private void HandleTogglePropertyExpansion(ContainerPropertyBinding binding)
        {
            binding.showExpanded = !binding.showExpanded;
            StoreAndUpdateView();
        }

        private void HandleRemoveBinding(ContainerPropertyBinding binding)
        {
            view.containerPropertyBindings = view.containerPropertyBindings
                .Where(x => x != binding)
                .ToArray();

            StoreAndUpdateView();
        }

        private void HandleDataSourceTypeChanged(Type newDataSourceType)
        {
            if ((view != null) && (view.dataSourceType.Type != newDataSourceType))
            {
                view.dataSourceType.Type = newDataSourceType;

                StoreAndUpdateView();
            }
        }

        private void StoreAndUpdateView()
        {
            EditorUtility.SetDirty(view);
            AssetDatabase.SaveAssetIfDirty(view);

            MakeEditorView();

            EditorRootElement.MarkDirtyRepaint();
        }
    }
}
