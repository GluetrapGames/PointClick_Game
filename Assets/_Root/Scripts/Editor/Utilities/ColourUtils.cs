using System;
using UnityEngine;

namespace GlueTrap.Editor.Utilities
{
// Modified version of the "ColorUtils" class from the EditorAttributes plugin.
public static class ColourUtils
{
	/// <summary>
	///     Converts the GUIColor value to a color.
	/// </summary>
	/// <param name="colour">The GUIColor</param>
	/// <param name="alpha">Custom transparency value</param>
	/// <returns>The color value</returns>
	public static Color GUIColourToColour(GUIColour colour,
		float alpha = 1f)
	{
		return colour switch
		{
			GUIColour.White => new Color(Color.white.r, Color.white.g,
				Color.white.b, alpha),
			GUIColour.Black => new Color(Color.black.r, Color.black.g,
				Color.black.b, alpha),
			GUIColour.Gray => new Color(Color.gray.r, Color.gray.g,
				Color.gray.b, alpha),
			GUIColour.Red => new Color(Color.red.r, Color.red.g,
				Color.red.b, alpha),
			GUIColour.Green => new Color(Color.green.r, Color.green.g,
				Color.green.b, alpha),
			GUIColour.Blue => new Color(Color.blue.r, Color.blue.g,
				Color.blue.b, alpha),
			GUIColour.Cyan => new Color(Color.cyan.r, Color.cyan.g,
				Color.cyan.b, alpha),
			GUIColour.Magenta => new Color(Color.magenta.r, Color.magenta.g,
				Color.magenta.b, alpha),
			GUIColour.Yellow => new Color(Color.yellow.r, Color.yellow.g,
				Color.yellow.b, alpha),
			GUIColour.Orange => new Color(1f, 149f / 255f, 0f, alpha),
			GUIColour.Brown => new Color(161f / 255f, 62f / 255f, 0f,
				alpha),
			GUIColour.Purple => new Color(158f / 255f, 5f / 255f,
				247f / 255f, alpha),
			GUIColour.Pink => new Color(247f / 255f, 5f / 255f, 171f / 255f,
				alpha),
			GUIColour.Lime => new Color(145f / 255f, 1f, 0f, alpha),
			GUIColour.Default => new Color(0f, 0.6f, 0.8f, alpha),
			_ => throw new ArgumentOutOfRangeException(nameof(colour),
				colour, null)
		};
	}
}
}