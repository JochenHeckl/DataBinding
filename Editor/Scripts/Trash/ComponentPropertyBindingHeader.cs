// using System;
// using UnityEditor;
// using UnityEngine.UIElements;

// namespace JH.DataBinding.Editor
// {
//     internal class ComponentPropertyBindingHeader : VisualElement
//     {
//         public bool ShowExpanded
//         {
//             set
//             {
//                 toggleBindingExpansionButton.text = value
//                     ? DataBindingCommonData.EditorDisplayText.CondenseButtonText
//                     : DataBindingCommonData.EditorDisplayText.ExpandButtonText;
//             }
//         }
//         private ComponentPropertyBindingStateLabel bindingStateLabel;
//         private Button toggleBindingExpansionButton;

//         public ComponentPropertyBindingHeader(
//             SerializedProperty property,
//             Action handleTogglePropertyExpansion
//         )
//         {
//             this.ApplBindingHeaderStyle();
//             AddToClassList(DataBindingEditorStyles.bindingHeaderRow);

//             var (bindingState, bindableDataSourceProperties) =
//                 DataBindingCommonData.DetermineComponentPropertyBindingState(property);

//             bindingStateLabel = new ComponentPropertyBindingStateLabel(property);
//             Add(bindingStateLabel);

//             toggleBindingExpansionButton = new Button(handleTogglePropertyExpansion);
//             toggleBindingExpansionButton.SetEnabled(
//                 bindingState == ComponentPropertyBindingState.Complete
//             );
//             Add(toggleBindingExpansionButton);

//             // var bindingShowExpanded =
//             //     (property.boxedValue as ComponentPropertyBinding)?.showExpanded ?? false;

//             // ShowExpanded =
//             //     (bindingState != ComponentPropertyBindingState.Complete) || bindingShowExpanded;
//         }
//     }
// }
