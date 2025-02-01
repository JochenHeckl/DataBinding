using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace JH.DataBinding.Editor
{
    public static class DataBindingEditorStyle
    {
        private static readonly string viewEditorStyleSheetFile =
            "Packages/de.jochenheckl.unity.databinding/Editor/DataBinding.uss";

        private static StyleSheet sharedStyleSheetAsset;

        public const string errorMessageContainer = nameof(errorMessageContainer);
        public const string errorMessageText = nameof(errorMessageText);

        public const string createDefaultDataSourceButton = nameof(createDefaultDataSourceButton);

        public const string dataSourceSelection = nameof(dataSourceSelection);
        public const string dataSourceSelectionButtonGroup = nameof(dataSourceSelectionButtonGroup);

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

        internal static void DataSourceStyle(VisualElement visualElement)
        {
            visualElement.style.marginTop = 12;
        }

        internal static void ErrorMessageStyle(VisualElement visualElement)
        {
            visualElement.style.marginTop = 12;
            visualElement.style.fontSize = 16;
            visualElement.style.color = Color.red;
        }

        internal static void ErrorButtonStyle(VisualElement visualElement)
        {
            visualElement.style.alignSelf = Align.Center;
        }
    }
}
