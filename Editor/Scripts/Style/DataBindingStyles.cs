using UnityEngine;
using UnityEngine.UIElements;

namespace JH.DataBinding.Editor
{
    public static class DataBindingStyles
    {
        public static void ApplyBindingContainerStyle(this VisualElement visualElement)
        {
            visualElement.style.flexDirection = FlexDirection.Column;
            visualElement.style.marginTop = 4;
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
