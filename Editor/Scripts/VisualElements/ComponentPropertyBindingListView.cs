using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace JH.DataBinding.Editor
{
    class ComponentPropertyBindingListView : ListView
    {
        private SerializedObject serializedObject;

        public ComponentPropertyBindingListView(
            SerializedObject serializedObject,
            Action invalideEditor
        )
        {
            this.serializedObject = serializedObject;

            // TODO: think abou how to apply styles in general
            this.ApplyBindingContainerStyle();

            virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
            bindingPath = nameof(View.componentPropertyBindings);

            headerTitle = DataBindingCommonData.EditorDisplayText.ComponentPropertyBindingsText;
            showFoldoutHeader = true;
            bindingSourceSelectionMode = BindingSourceSelectionMode.AutoAssign;
            reorderable = true;
            reorderMode = ListViewReorderMode.Animated;
            selectionType = SelectionType.Single;
            showBoundCollectionSize = true;
            showAddRemoveFooter = true;
            showBorder = true;
            allowAdd = true;
            allowRemove = true;
            //  makeItem = MakeComponentPropertyBinding;
            // bindItem = BindComponentPropertyBinding;
        }

        // private VisualElement MakeComponentPropertyBinding() { }

        private void BindComponentPropertyBinding(VisualElement element, int offset)
        {
            // serializedObject.var propertyField = new PropertyField(
            //     property,
            //     MakeLabelHeaderText(property)
            // );

            // element.Add(propertyField);
            // root.MarkDirtyRepaint();
            // var componentPropertyBinding = view.componentPropertyBindings[offset];
        }
    }
}
