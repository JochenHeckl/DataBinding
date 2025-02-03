using UnityEngine;
using UnityEngine.UIElements;

namespace JH.DataBinding.Editor
{
    public static class VisualElementStyleExtensions
    {
        public static StyleLength FromPixels(int pixels)
        {
            return new StyleLength(new Length(pixels, LengthUnit.Pixel));
        }

        public static void Margin(this IStyle style, StyleLength length)
        {
            style.marginTop = style.marginLeft = style.marginRight = style.marginBottom = length;
        }

        public static void Padding(this IStyle style, StyleLength length)
        {
            style.paddingTop =
                style.paddingLeft =
                style.paddingRight =
                style.paddingBottom =
                    length;
        }

        public static void BorderWidth(this IStyle style, StyleFloat length)
        {
            style.borderTopWidth =
                style.borderLeftWidth =
                style.borderRightWidth =
                style.borderBottomWidth =
                    length;
        }

        public static void BorderRadius(this IStyle style, StyleLength length)
        {
            style.borderTopLeftRadius =
                style.borderTopRightRadius =
                style.borderBottomLeftRadius =
                style.borderBottomRightRadius =
                    length;
        }

        public static void BorderColor(this IStyle style, StyleColor color)
        {
            style.borderTopColor =
                style.borderLeftColor =
                style.borderRightColor =
                style.borderBottomColor =
                    color;
        }
    }
}
