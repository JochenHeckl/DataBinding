using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace JH.DataBinding.Editor
{
    class ComponentPropertyBindingListView : ListView
    {
        public ComponentPropertyBindingListView()
        {
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
        }
    }
}
