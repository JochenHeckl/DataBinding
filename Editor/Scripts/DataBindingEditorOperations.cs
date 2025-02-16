using System.IO;
using UnityEditor;
using UnityEngine;

namespace JH.DataBinding.Editor
{
    internal static class DataBindingEditorOperations
    {
        internal static void CreateNewDataSource(string currentViewName)
        {
            string selectedPath = EditorUtility.OpenFilePanelWithFilters(
                "Select Asset Location",
                Path.Combine("Assets", $"{currentViewName}DataSource.cs"),
                new string[] { "CSharp Script", "cs", "All Files", "*" }
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
