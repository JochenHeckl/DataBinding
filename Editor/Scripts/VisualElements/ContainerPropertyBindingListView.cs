using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace JH.DataBinding.Editor
{
    class ContainerPropertyBindingListView : ListView
    {
        public ContainerPropertyBindingListView()
        {
            // TODO: think abou how to apply styles in general
            this.ApplyBindingContainerStyle();

            virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
            bindingPath = nameof(View.containerPropertyBindings);

            headerTitle = DataBindingCommonData.EditorDisplayText.ContainerPropertyBindingsText;
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
