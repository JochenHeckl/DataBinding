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
                InitDatabindingEditorRootElement();

                EditorRootElement.Add(
                    MakeDataSourceSection(view.dataSourceType, HandleDataSourceTypeChanged)
                );

                EditorRootElement.Add(MakeComponentPropertyBindings());
                EditorRootElement.Add(MakeContainerPropertyBindings());
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
                EditorDisplayText.ComponentPropertyBindingsText,
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
                EditorDisplayText,
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
                EditorDisplayText.ContainerPropertyBindingsText,
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
                EditorDisplayText,
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
