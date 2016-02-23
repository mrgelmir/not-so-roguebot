using UnityEngine;

namespace Utils
{
	public static class ColorUtils
	{
		public static string ToHex(this Color32 color)
		{
			return ColorToHex(color);
		}

		public static string ToHex(this Color color)
		{
			return ColorToHex(color);
		}

		public static string ColorToHex(Color32 color)
		{
			string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
			return hex;
		}

		public static Color HexToColor(this string hex)
		{
			byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
			byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
			byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
			return new Color32(r, g, b, 255);
		}
	}
}
