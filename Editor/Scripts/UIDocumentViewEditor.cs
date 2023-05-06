using System;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEngine.UIElements;

using de.JochenHeckl.Unity.DataBinding.Editor;

namespace de.JochenHeckl.Unity.DataBinding.Experimental.Editor
{
    [CustomEditor(typeof(UIDocumentView), true)]
    public class UIDocumentViewEditor : ViewEditorBase
    {
        private UIDocumentView view;
        private Dictionary<string, bool> expansionState = new();

        public override void OnEnable()
        {
            base.OnEnable();

            view = target as UIDocumentView;

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

                EditorRootElement.Add(MakeVisualElementPropertyBindings());
            }
            catch (Exception exception)
            {
                EditorRootElement.Clear();
                EditorRootElement.Add(MakeErrorReport(exception));
            }
        }

        private VisualElement MakeVisualElementPropertyBindings()
        {
            return MakeBindingSection(
                "VisualElement Property Bindings",
                HandleAddVisualElementPropertyBinding,
                view.visualElementPropertyBindings,
                MakeVisualElementPropertyBindingVisualElement
            );
        }

        private void HandleAddVisualElementPropertyBinding()
        {
            view.visualElementPropertyBindings = view.visualElementPropertyBindings
                .Append(new VisualElementPropertyBinding())
                .ToArray();

            StoreAndUpdateView();
        }

        private VisualElement MakeVisualElementPropertyBindingVisualElement(
            VisualElementPropertyBinding binding
        )
        {
            return new VisualElementPropertyBindingEditor(
                EditorDisplayText,
                view.dataSourceType.Type,
                view.RootVisualElement,
                binding,
                StoreAndUpdateView,
                binding => GetExpansionState(binding),
                HandleMoveBindingUp,
                HandleMoveBindingDown,
                HandleTogglePropertyExpansion,
                HandleRemoveBinding
            );
        }

        private void HandleMoveBindingUp(VisualElementPropertyBinding binding)
        {
            view.visualElementPropertyBindings = MoveElementUp(
                    view.visualElementPropertyBindings,
                    binding
                )
                .ToArray();

            StoreAndUpdateView();
        }

        private void HandleMoveBindingDown(VisualElementPropertyBinding binding)
        {
            view.visualElementPropertyBindings = MoveElementDown(
                    view.visualElementPropertyBindings,
                    binding
                )
                .ToArray();

            StoreAndUpdateView();
        }

        private void HandleTogglePropertyExpansion(VisualElementPropertyBinding binding)
        {
            ToggleExpansionState(binding);
            StoreAndUpdateView();
        }

        private void HandleRemoveBinding(VisualElementPropertyBinding binding)
        {
            view.visualElementPropertyBindings = view.visualElementPropertyBindings
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

        private bool GetExpansionState(VisualElementPropertyBinding binding)
        {
            bool expandBinding = false;

            expansionState.TryGetValue(binding.SourcePath, out expandBinding);

            return expandBinding;
        }

        private void ToggleExpansionState(VisualElementPropertyBinding binding)
        {
            if (binding.SourcePath != null)
            {
                expansionState[binding.SourcePath] = !GetExpansionState(binding);
            }
        }
    }
}
