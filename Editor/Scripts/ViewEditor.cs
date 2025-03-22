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

                    view.dataSourceType.Type = guessedDataSourceType;
                    serializedObject.ApplyModifiedProperties();
                    serializedObject.Update();
                    EditorUtility.SetDirty(view);
                }
            }
        }

        public override VisualElement CreateInspectorGUI()
        {
            editorRootElement = new VisualElement();
            editorRootElement.styleSheets.Add(DataBindingEditorStyle.StyleSheet);

            InspectorElement.FillDefaultInspector(editorRootElement, serializedObject, this);

            var dataSourceTypeProperty = serializedObject.FindProperty(nameof(View.dataSourceType));

            editorRootElement.TrackSerializedObjectValue(
                serializedObject,
                x =>
                {
                    editorRootElement.Bind(x);
                }
            );

            return editorRootElement;
        }
    }
}
