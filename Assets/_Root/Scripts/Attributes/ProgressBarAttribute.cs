using UnityEngine;

namespace GlueTrap
{
	// Modified version of the "GUIColor" enum from the EditorAttributes plugin.
	public enum GUIColour
	{
		Default,
		White,
		Black,
		Gray,
		Red,
		Green,
		Lime,
		Blue,
		Cyan,
		Yellow,
		Orange,
		Brown,
		Magenta,
		Purple,
		Pink
	}

	// Modified version of the "ProgressBarAttribute" class from the
	// EditorAttributes plugin.
	public class ProgressBarAttribute : PropertyAttribute
	{
		/// <summary>
		///     Attribute to draw a progress bar.
		/// </summary>
		/// <param name="maxFieldName">
		///     The name of the field to use as a maximum value of the progress bar
		/// </param>
		/// <param name="barHeight">The height of the progress bar in pixels</param>
		public ProgressBarAttribute(string maxFieldName, float barHeight = 20f)
		{
			m_MaxFieldName = maxFieldName;
			m_BarHeight = barHeight;
			m_BarColour = GUIColour.Default;
			m_LabelColour = GUIColour.White;
		}

		/// <summary>
		///     Attribute to draw a progress bar.
		/// </summary>
		/// <param name="maxFieldName">
		///     The name of the field to use as a maximum value of the progress bar
		/// </param>
		/// <param name="barColour">The colour of the progress bar</param>
		/// <param name="barHeight">The height of the progress bar in pixels</param>
		public ProgressBarAttribute(string maxFieldName, GUIColour barColour,
			float barHeight = 20f)
		{
			m_MaxFieldName = maxFieldName;
			m_BarHeight = barHeight;
			m_BarColour = barColour;
			m_LabelColour = GUIColour.White;
		}

		/// <summary>
		///     Attribute to draw a progress bar.
		/// </summary>
		/// <param name="maxFieldName">
		///     The name of the field to use as a maximum value of the progress bar
		/// </param>
		/// <param name="r">Red amount</param>
		/// <param name="g">Green amount</param>
		/// <param name="b">Blue amount</param>
		/// <param name="barHeight">The height of the progress bar in pixels</param>
		public ProgressBarAttribute(string maxFieldName, float r, float g,
			float b, float barHeight = 20f)
		{
			m_UsingBarRGB = true;
			m_MaxFieldName = maxFieldName;
			m_BarHeight = barHeight;
			m_BarColourRGB = new Color(r, g, b);
			m_LabelColour = GUIColour.White;
		}

		/// <summary>
		///     Attribute to draw a progress bar.
		/// </summary>
		/// <param name="maxFieldName">
		///     The name of the field to use as a maximum value of the progress bar
		/// </param>
		/// <param name="barColour">The colour of the progress bar</param>
		/// <param name="labelColour">The colour of the progress bar label</param>
		/// <param name="barHeight">The height of the progress bar in pixels</param>
		public ProgressBarAttribute(string maxFieldName, GUIColour barColour,
			GUIColour labelColour, float barHeight = 20f)
		{
			m_MaxFieldName = maxFieldName;
			m_BarHeight = barHeight;
			m_BarColour = barColour;
			m_LabelColour = labelColour;
		}

		/// <summary>
		///     Attribute to draw a progress bar.
		/// </summary>
		/// <param name="maxFieldName">
		///     The name of the field to use as a maximum value of the progress bar
		/// </param>
		/// <param name="rB">Red amount of the bar colour</param>
		/// <param name="gB">Green amount of the bar colour</param>
		/// <param name="bB">Blue amount of the bar colour</param>
		/// <param name="rL">Red amount of the label colour</param>
		/// <param name="gL">Green amount of the label colour</param>
		/// <param name="bL">Blue amount of the label colour</param>
		/// <param name="barHeight">The height of the progress bar in pixels</param>
		public ProgressBarAttribute(string maxFieldName, float rB, float gB,
			float bB, float rL, float gL, float bL, float barHeight = 20f)
		{
			m_UsingBarRGB = true;
			m_UsingLabelRGB = true;
			m_MaxFieldName = maxFieldName;
			m_BarHeight = barHeight;
			m_BarColourRGB = new Color(rB, gB, bB);
			m_LabelColourRGB = new Color(rL, gL, bL);
		}

		/// <summary>
		///     Attribute to draw a progress bar.
		/// </summary>
		/// <param name="maxFieldName">
		///     The name of the field to use as a maximum value of the progress bar
		/// </param>
		/// <param name="rB">Red amount of the bar colour</param>
		/// <param name="gB">Green amount of the bar colour</param>
		/// <param name="bB">Blue amount of the bar colour</param>
		/// <param name="labelColour">The colour of the progress bar label</param>
		/// <param name="barHeight">The height of the progress bar in pixels</param>
		public ProgressBarAttribute(string maxFieldName, float rB, float gB,
			float bB, GUIColour labelColour, float barHeight = 20f)
		{
			m_UsingBarRGB = true;
			m_MaxFieldName = maxFieldName;
			m_BarHeight = barHeight;
			m_BarColourRGB = new Color(rB, gB, bB);
			m_LabelColour = labelColour;
		}

		/// <summary>
		///     Attribute to draw a progress bar.
		/// </summary>
		/// <param name="maxFieldName">
		///     The name of the field to use as a maximum value of the progress bar
		/// </param>
		/// <param name="barColour">The colour of the progress bar</param>
		/// <param name="rL">Red amount of the label colour</param>
		/// <param name="gL">Green amount of the label colour</param>
		/// <param name="bL">Blue amount of the label colour</param>
		/// <param name="barHeight">The height of the progress bar in pixels</param>
		public ProgressBarAttribute(string maxFieldName, GUIColour barColour,
			float rL, float gL, float bL, float barHeight = 20f)
		{
			m_UsingLabelRGB = true;
			m_MaxFieldName = maxFieldName;
			m_BarHeight = barHeight;
			m_BarColour = barColour;
			m_LabelColourRGB = new Color(rL, gL, bL);
		}


		public string m_MaxFieldName { get; private set; }
		public float m_BarHeight { get; private set; }
		public GUIColour m_BarColour { get; private set; }
		public Color m_BarColourRGB { get; private set; }
		public Color m_LabelColourRGB { get; private set; }
		public bool m_UsingBarRGB { get; private set; }
		public bool m_UsingLabelRGB { get; private set; }
		public GUIColour m_LabelColour { get; private set; }
	}
}