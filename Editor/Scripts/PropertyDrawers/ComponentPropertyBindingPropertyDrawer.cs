using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace JH.DataBinding.Editor
{
    [CustomPropertyDrawer(typeof(ComponentPropertyBinding))]
    public class ComponentPropertyBindingPropertyDrawer : PropertyDrawer
    {
        internal static readonly IDataBindingEditorDisplayText EditorDisplayText =
            new DataBindingEditorDisplayText();

        private VisualElement rootVisualElement;
        private PropertyField propertyField;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            rootVisualElement = new();
            rootVisualElement.name = $"ComponentPropertyBinding for {property.displayName}";

            FillInRoot(rootVisualElement, property);
            return rootVisualElement;
        }

        private void FillInRoot(VisualElement root, SerializedProperty property)
        {
            root.Clear();

            propertyField = new PropertyField(property, MakeLabelHeaderText(property));
            propertyField.name = nameof(ComponentPropertyBinding);

            root.Add(propertyField);
            root.MarkDirtyRepaint();
        }

        private string MakeLabelHeaderText(SerializedProperty property)
        {
            var (bindingState, bindableDataSourceProperties) =
                DataBindingCommonData.DetermineComponentPropertyBindingState(property);

            return MakeLabelHeaderText(
                property.boxedValue as ComponentPropertyBinding,
                bindingState,
                bindableDataSourceProperties
            );
        }

        private string MakeLabelHeaderText(
            ComponentPropertyBinding binding,
            ComponentPropertyBindingState bindingState,
            PropertyInfo[] bindableDataSourceProperties
        )
        {
            var displayText = DataBindingCommonData.EditorDisplayText;

            switch (bindingState)
            {
                case ComponentPropertyBindingState.MissingDataSourceAssignment:
                    return displayText.BindingMissingDataSourceAssignment;

                case ComponentPropertyBindingState.SourceUnbound:
                    return displayText.BindingSourceUnboundMessageText;

                case ComponentPropertyBindingState.TargetUnbound:
                    return displayText.BindingTargetUnboundMessageText;

                case ComponentPropertyBindingState.Unassignable:
                    return displayText.BindingUnassignableMessageText;

                case ComponentPropertyBindingState.Complete:

                    var sourceProperty = bindableDataSourceProperties.Single(x =>
                        x.Name == binding.SourcePath
                    );

                    var friendlySourceTypeName = sourceProperty
                        .PropertyType.GetTypeInfo()
                        .GetFriendlyName();

                    var condensedLabelString = string.Format(
                        displayText.ComponentPropertyBindingCondensedLabelFormat_Type_Source_Target_Component,
                        friendlySourceTypeName,
                        binding.SourcePath,
                        binding.TargetComponent.name,
                        $"{binding.TargetComponent.GetType().Name}.{binding.TargetPath}"
                    );

                    return condensedLabelString;
            }

            throw new InvalidProgramException(
                "Please upgrade to CoreCLR, so my compiler can deal with this."
            );
        }
    }
}
