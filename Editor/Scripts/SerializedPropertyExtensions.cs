using UnityEditor;

public static class SerializedPropertyExtensions
{
    public static SerializedProperty GetParentProperty(this SerializedProperty property)
    {
        var path = property.propertyPath.Replace(".Array.data[", "[");
        var elements = path.Split('.');

        SerializedProperty parent = property.serializedObject.FindProperty(elements[0]);

        for (int i = 1; i < elements.Length - 1; i++)
        {
            var element = elements[i];
            if (element.Contains("["))
            {
                var index = System.Convert.ToInt32(
                    element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", "")
                );
                parent = parent.GetArrayElementAtIndex(index);
            }
            else
            {
                parent = parent.FindPropertyRelative(element);
            }
        }

        return parent;
    }
}
