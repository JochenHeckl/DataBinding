using UnityEditor;
using UnityEngine.UIElements;

namespace JH.DataBinding.Editor
{
    public static class DataBindingEditorStyles
    {
        private static readonly string viewEditorStyleSheetFile =
            "Packages/de.jochenheckl.unity.databinding/Editor/UI/DataBinding.uss";

        private static StyleSheet sharedStyleSheetAsset;
        public const string viewEditorClassName = "viewEditor";

        public const string GenericRow = "genericRow";

        public const string SuccessText = "successText";
        public const string ErrorText = "errorText";
        public const string InfoText = "infoText";

        public const string addBindingActionButton = "addBindingActionButton";
        public const string bindingActionButton = "bindingActionButton";
        public const string bindingContainer = "bindingContainer";
        public const string bindingDataSourceTypeLabel = "bindingDataSourceTypeLabel";
        public const string bindingGroup = "bindingGroup";
        public const string bindingGroupHeader = "bindingGroupHeader";
        public const string bindingGroupLabel = "bindingGroupLabel";
        public const string bindingGroupList = "bindingGroupList";
        public const string bindingHeaderRow = "bindingHeaderRow";
        public const string bindingInteractionButtonContainer = "bindingInteractionButtonContainer";
        public const string bindingProperty = "bindingProperty";
        public const string condensedBindingLabel = "condensedBindingLabel";
        public const string invalidBindingClassName = "invalidBinding";

        public static StyleSheet StyleSheet
        {
            get
            {
                if (sharedStyleSheetAsset == null)
                {
                    sharedStyleSheetAsset = AssetDatabase.LoadAssetAtPath<StyleSheet>(
                        viewEditorStyleSheetFile
                    );
                }

                return sharedStyleSheetAsset;
            }
        }
    }
}
