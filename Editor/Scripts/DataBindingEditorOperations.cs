using System.IO;
using System.Linq;
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

            var viewNameWithoutSpaces = new string(
                currentViewName.ToCharArray().Where(x => !char.IsWhiteSpace(x)).ToArray()
            );

            string selectedPath = EditorUtility.SaveFilePanelInProject(
                "Select Asset Location",
                $"{viewNameWithoutSpaces}DataSource.cs",
                "cs",
                pathToOpen
            );

            if (!string.IsNullOrEmpty(selectedPath))
            {
                var dataSourceCode = DataBindingCommonData.DefaultDataSourceTemplate.Replace(
                    DataBindingCommonData.DefaultDataSourceTemplateNamePlaceHolder,
                    viewNameWithoutSpaces
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
