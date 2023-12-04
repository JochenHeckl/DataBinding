using UnityEditor;

namespace de.JochenHeckl.Unity.DataBinding.Editor
{
    public static class DatabindingEditorMenu
    {
        [MenuItem("Data Binding/About")]
        public static void About()
        {
            EditorUtility.DisplayDialog("Data Binding", $"Thank you for using DataBinding.", "OK");
        }
    }
}
