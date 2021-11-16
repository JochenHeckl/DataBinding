using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine.UIElements;

namespace de.JochenHeckl.Unity.DataBinding.Editor
{
    public static class DataBindingEditorStyles
    {
        private static readonly string _viewEditorStyleSheetFile
            = "Packages/de.jochenheckl.unity.dataBinding/Editor/UI/DataBinding.uss";

        private static StyleSheet _sharedStyleSheetAsset;
        public const string viewEditorClassName = "viewEditor";
        public const string invalidBindingClassName = "invalidBinding";
        
        public const string bindingDataSourceTypeLabelClassName = "bindingDataSourceTypeLabel";
        public const string bindingGroupClassName = "bindingGroup";
        public const string bindingGroupHeaderClassName = "bindingGroupHeader";
        public const string bindingGroupLabelClassName = "bindingGroupLabel";
        public const string bindingGroupListClassName = "bindingGroupList";
        public const string bindingActionButtonClassName = "bindingActionButton";
        public const string bindingDefinitionClassName = "bindingDefinition";
        public const string bindingDefinitionHeaderClassName = "bindingDefinitionHeader";
        public const string condensedBindingLabelClassName = "condensedBindingLabel";



        public static StyleSheet StyleSheet
        {
            get
            {
                // if ( _sharedStyleSheetAsset == null )
                {
                    _sharedStyleSheetAsset = AssetDatabase.LoadAssetAtPath<StyleSheet>( _viewEditorStyleSheetFile );
                }

                return _sharedStyleSheetAsset;
            }
        }
    }
}
