using System;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;

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
            editorRootElement.styleSheets.Add(DataBindingEditorStyles.StyleSheet);

            FillInRoot(editorRootElement);

            editorRootElement.TrackSerializedObjectValue(serializedObject, HandleObjectChanged);

            return editorRootElement;
        }

        private void FillInRoot(VisualElement root)
        {
            try
            {
                root.Clear();

                root.Add(new DataSourceSelection(target as View, HandleDataSourceTypeChanged));

                var componentPropertyBindings = new ComponentPropertyBindingListView();
                componentPropertyBindings.Bind(serializedObject);
                root.Add(componentPropertyBindings);

                var containerPropertyBindings = new ContainerPropertyBindingListView();
                containerPropertyBindings.Bind(serializedObject);
                root.Add(containerPropertyBindings);
            }
            catch (Exception exception)
            {
                root.Add(new InternalErrorReport(exception));
            }
        }

        private void HandleObjectChanged(SerializedObject _)
        {
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
