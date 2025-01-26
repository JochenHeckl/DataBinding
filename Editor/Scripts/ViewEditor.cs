using System;
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

            if (view.dataSourceType.Type == null)
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
            editorRootElement.styleSheets.Add(DataBindingEditorStyles.StyleSheet);

            FillInRoot(editorRootElement);

            editorRootElement.TrackSerializedObjectValue(serializedObject, HandleObjectChanged);

            return editorRootElement;
        }

        private void HandleObjectChanged(SerializedObject _)
        {
            InvalidateEditor();
        }

        private void FillInRoot(VisualElement root)
        {
            try
            {
                root.Clear();

                root.Add(new DataSourceSelection(target as View, HandleDataSourceTypeChanged));

                var componentPropertyBindings = new ComponentPropertyBindingListView(
                    serializedObject,
                    InvalidateEditor
                );
                componentPropertyBindings.Bind(serializedObject);
                root.Add(componentPropertyBindings);
            }
            catch (Exception exception)
            {
                root.Add(new InternalErrorReport(exception));
            }
        }

        private void InvalidateEditor()
        {
            FillInRoot(editorRootElement);
        }

        private void BindItem(VisualElement element, int itemIndex)
        {
            element.Clear();
        }

        private void BindComponentPropertyListViewItem(
            VisualElement visualElement,
            int itemIndex
        ) { }

        private VisualElement MakeComponentPropertyListViewItem()
        {
            var container = new VisualElement();
            container.style.flexShrink = 1;
            container.style.flexGrow = 1;
            container.style.whiteSpace = WhiteSpace.Normal;
            container.name = "XXX";
            return container;
        }

        private void HandleAddComponentPropertyBinding()
        {
            view.componentPropertyBindings = view
                .componentPropertyBindings.Append(
                    new ComponentPropertyBinding() { DataSource = view.DataSource }
                )
                .ToArray();

            StoreAndUpdateView();
        }

        // private void HandleDataSourceTypeChanged(Type newDataSourceType)
        // {
        //     if ((view != null) && (view.dataSourceType.Type != newDataSourceType))
        //     {
        //         view.dataSourceType.Type = newDataSourceType;

        //         StoreAndUpdateView();
        //     }

        //     serializedObject.ApplyModifiedProperties();
        // }

        private void StoreAndUpdateView()
        {
            EditorUtility.SetDirty(view);
            AssetDatabase.SaveAssetIfDirty(view);

            // MakeEditorView();

            // EditorRootElement.MarkDirtyRepaint();
        }

        private void HandleDataSourceTypeChanged(Type newType)
        {
            if (view.dataSourceType.Type != newType)
            {
                EditorUtility.SetDirty(view);
                view.dataSourceType.Type = newType;
                serializedObject.ApplyModifiedProperties();

                InvalidateEditor();
            }
        }
    }
}
