using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;

namespace JH.DataBinding.Editor
{
    [CustomPropertyDrawer(typeof(EnumerableBindingSourcePathAttribute))]
    public class ContainerPropertyBindingSourcePathPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var view = property.serializedObject.targetObject as View;
            var sourceProperties = DataBindingCommonData.GetBindableDataSourceProperties(
                view.dataSourceType.Type
            );

            var options = sourceProperties
                .Where(x =>
                    typeof(IEnumerable<INotifyDataSourceChanged>).IsAssignableFrom(x.PropertyType)
                )
                .Select(x => x.Name)
                .ToList();

            var rootVisualElement = new DropdownField(
                property.displayName,
                options,
                options.IndexOf(property.stringValue)
            );

            rootVisualElement.AddToClassList("unity-base-field__aligned");

            rootVisualElement.RegisterValueChangedCallback(x =>
            {
                property.stringValue = x.newValue;
                property.serializedObject.ApplyModifiedProperties();

                EditorUtility.SetDirty(view);
            });

            return rootVisualElement;
        }
    }
}
