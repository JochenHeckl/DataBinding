using System.IO;
using UnityEditor;
using UnityEngine;

namespace JH.DataBinding.Editor
{
    internal static class DataBindingEditorOperations
    {
        internal static void CreateNewDataSource(string currentViewName)
        {
            var pathToOpen = Application.dataPath;
            Object selectedObject = Selection.activeObject;

            if (selectedObject != null)
            {
                var selectedObjectPath = AssetDatabase.GetAssetPath(selectedObject);

                if (!string.IsNullOrEmpty(selectedObjectPath))
                {
                    pathToOpen = Path.GetDirectoryName(selectedObjectPath);
                }
            }

            string selectedPath = EditorUtility.OpenFilePanel(
                "Select Asset Location",
                Path.Combine(pathToOpen, $"{currentViewName}DataSource.cs"),
                "cs"
            );

            if (!string.IsNullOrEmpty(selectedPath))
            {
                if (!selectedPath.StartsWith(Application.dataPath))
                {
                    EditorUtility.DisplayDialog(
                        DataBindingCommonData.EditorDisplayText.InvalidPathTitle,
                        DataBindingCommonData.EditorDisplayText.InvalidPathContent,
                        DataBindingCommonData.EditorDisplayText.OKLabel
                    );

                    return;
                }

                var dedicatedClassName = Path.GetFileNameWithoutExtension(selectedPath);

                var dataSourceCode = DataBindingCommonData.DefaultDataSourceTemplate.Replace(
                    DataBindingCommonData.DefaultDataSourceTemplateNamePlaceHolder,
                    dedicatedClassName
                );

                File.WriteAllText(selectedPath, dataSourceCode);
                AssetDatabase.ImportAsset(selectedPath);

                OpenDataSourceEditor(selectedPath);
            }
        }

        internal static void OpenDataSourceEditor(string dataSourceSourceFile)
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
    }
}
