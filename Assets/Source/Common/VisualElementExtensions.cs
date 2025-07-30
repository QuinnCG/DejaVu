using UnityEngine.UIElements;

namespace Quinn
{
	public static class VisualElementExtensions
	{
		/// <summary>
		/// Sets all padding values to the same value.
		/// </summary>
		public static void SetPadding(this IStyle style, StyleLength length)
		{
			style.paddingBottom = style.paddingLeft = style.paddingRight = style.paddingTop = length;
		}
		/// <summary>
		/// Sets all margin values to the same value.
		/// </summary>
		public static void SetMargin(this IStyle style, StyleLength length)
		{
			style.marginBottom = style.marginLeft = style.marginRight = style.marginTop = length;
		}

		/// <summary>
		/// Sets all border radii to the same value.
		/// </summary>
		public static void SetBorderRadius(this IStyle style, StyleLength radius)
		{
			style.borderBottomLeftRadius = style.borderBottomRightRadius = style.borderTopLeftRadius = style.borderTopRightRadius = radius;
		}
		/// <summary>
		/// Sets all border colors to the same value.
		/// </summary>
		public static void SetBorderColor(this IStyle style, StyleColor color)
		{
			style.borderBottomColor = style.borderLeftColor = style.borderTopColor = style.borderRightColor = color;
		}
		/// <summary>
		/// Sets all border widths to the same value.
		/// </summary>
		public static void SetBorderWidth(this IStyle style, StyleFloat width)
		{
			style.borderLeftWidth = style.borderRightWidth = style.borderTopWidth = style.borderBottomWidth = width;
		}
	}
}
