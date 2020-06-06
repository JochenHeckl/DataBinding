using UnityEditor;

namespace de.JochenHeckl.Unity.DataBinding.Editor
{
    [CustomEditor( typeof( DropdownAdapterBindingBuilder ) )]
    internal class DropdownAdapterBindingBuilderEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawPropertiesExcluding( serializedObject, "m_Script" );

            serializedObject.ApplyModifiedProperties();
        }
    }
}
