using UnityEngine;
using UnityEngine.UIElements;

namespace JH.DataBinding.Editor
{
    public static class DataBindingStyles
    {
        public static void ApplyBindingContainerStyle(this VisualElement visualElement)
        {
            visualElement.style.flexDirection = FlexDirection.Column;
        }

        public static void ApplyBindingContainerItemStyle(this VisualElement visualElement) { }

        public static void ApplyErrorTextStyle(this VisualElement visualElement)
        {
            visualElement.style.fontSize = 16;
            visualElement.style.Padding(12);
            visualElement.style.Margin(12);
            visualElement.style.BorderColor(new Color(.6f, .2f, .2f));
            visualElement.style.BorderWidth(2);
            visualElement.style.BorderRadius(8);
            visualElement.style.textOverflow = TextOverflow.Ellipsis;
            visualElement.style.whiteSpace = WhiteSpace.PreWrap;
        }

        public static void ApplyDataSourceSelectionStyle(this VisualElement visualElement)
        {
            visualElement.style.fontSize = 16;
            visualElement.style.Margin(4);
            visualElement.style.flexDirection = FlexDirection.Row;
            visualElement.style.flexWrap = Wrap.NoWrap;
            visualElement.style.justifyContent = Justify.SpaceBetween;
        }

        public static void ApplyInternalErrorSectionStyle(this VisualElement visualElement)
        {
            visualElement.style.fontSize = 14;
            visualElement.style.Margin(4);
        }

        public static void ApplyReportErrorButtonStyle(this VisualElement visualElement)
        {
            visualElement.style.fontSize = 16;
            visualElement.style.Margin(4);
        }

        public static void ApplyComponentPropertyBindingStyle(this VisualElement visualElement)
        {
            visualElement.style.Margin(4);
        }

        public static void ApplBindingHeaderStyle(this VisualElement visualElement)
        {
            visualElement.style.Margin(2);
            visualElement.style.Padding(4);
            visualElement.style.fontSize = 15;
            visualElement.style.height = 20;
        }
    }
}
