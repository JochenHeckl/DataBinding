namespace de.JochenHeckl.Unity.DataBinding.Editor
{
    internal class ScriptlessEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawPropertiesExcluding( serializedObject, "m_Script" );

            serializedObject.ApplyModifiedProperties();
        }
    }
}