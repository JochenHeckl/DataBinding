using System;
using System.IO;
using UnityEditor;

namespace JH.DataBinding.Editor
{
    internal static class DataBindingEditorOperations
    {
        internal static void CreateNewDataSource(string currentViewName) { }

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
