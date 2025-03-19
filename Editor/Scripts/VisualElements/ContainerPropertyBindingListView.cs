using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace JH.DataBinding.Editor
{
    class ContainerPropertyBindingListView : ListView
    {
        public ContainerPropertyBindingListView()
        {
            // TODO: think about how to apply styles in general
            this.ApplyBindingContainerStyle();

            virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
            bindingPath = nameof(View.containerPropertyBindings);

            headerTitle = DataBindingCommonData.EditorDisplayText.ContainerPropertyBindingsText;
            showFoldoutHeader = true;
            reorderable = true;
            reorderMode = ListViewReorderMode.Animated;
            selectionType = SelectionType.Single;
            showBoundCollectionSize = true;
            showAddRemoveFooter = true;
            showBorder = true;
#if UNITY_2023_3_OR_NEWER
            bindingSourceSelectionMode = BindingSourceSelectionMode.AutoAssign;
            allowAdd = true;
            allowRemove = true;
#endif // UNITY_2023_3_OR_NEWER
        }
    }
}
