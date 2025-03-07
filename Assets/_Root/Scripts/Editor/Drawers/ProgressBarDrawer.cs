using System.Reflection;
using EditorAttributes.Editor;
using GlueTrap.Editor.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GlueTrap.Editor
{
// Modified version of the "ProgressBarDrawer" class from the
// EditorAttributes plugin. Made so that it can support dynamic MaxValue instancing.
[CustomPropertyDrawer(typeof(ProgressBarAttribute))]
public class ProgressBarDrawer : PropertyDrawerBase
{
	public override VisualElement CreatePropertyGUI(
		SerializedProperty property)
	{
		var root = new VisualElement();

		if (property.propertyType is SerializedPropertyType.Integer
		    or SerializedPropertyType.Float)
		{
			if (attribute is not ProgressBarAttribute progressBarAttribute)
				return root;

			var progressBar = new ProgressBar
			{
				tooltip = property.tooltip,
				style =
				{
					height = progressBarAttribute.m_BarHeight
				}
			};

			// Proper UI colours.
			Color barColour;
			Color labelColour;
			if (progressBarAttribute.m_UsingBarRGB)
				barColour = progressBarAttribute.m_BarColourRGB;
			else
			{
				barColour = ColourUtils.GUIColourToColour(
					progressBarAttribute.m_BarColour);
			}

			if (progressBarAttribute.m_UsingLabelRGB)
				labelColour = progressBarAttribute.m_LabelColourRGB;
			else
			{
				labelColour = ColourUtils.GUIColourToColour(
					progressBarAttribute.m_LabelColour);
			}

			progressBar
				.Q(className: AbstractProgressBar.progressUssClassName)
				.style.backgroundColor = new StyleColor(barColour);

			// Get the progress bar label and change its style.
			var label = progressBar.Q<Label>();
			label.style.unityFontStyleAndWeight = FontStyle.Bold;
			label.style.color = new StyleColor(labelColour);

			root.Add(progressBar);

			UpdateVisualElement(progressBar, () =>
			{
				var propertyValue = GetPropertyValue(property);
				var maxValue = GetMaxValue(property,
					progressBarAttribute.m_MaxFieldName);

				progressBar.highValue = maxValue;
				progressBar.value = propertyValue;
				progressBar.title =
					$"{property.displayName}: {propertyValue}/{maxValue}";
			}, 30);
		}
		else
		{
			root.Add(new HelpBox(
				"The ProgressBar Attribute can only be attached to an int or float",
				HelpBoxMessageType.Error));
		}

		return root;
	}

	private float GetPropertyValue(SerializedProperty property)
	{
		return property.propertyType switch
		{
			SerializedPropertyType.Integer => property.intValue,
			SerializedPropertyType.Float => property.floatValue,
			_ => 0f
		};
	}

	private float GetMaxValue(SerializedProperty property,
		string maxFieldName)
	{
		if (string.IsNullOrEmpty(maxFieldName)) return 100f; // Default max

		Object targetObject = property.serializedObject.targetObject;
		FieldInfo info = targetObject.GetType().GetField(maxFieldName,
			BindingFlags.Instance | BindingFlags.NonPublic |
			BindingFlags.Public);

		if (info == null)
			return 100f; // Default max if field is not found.

		return info.FieldType == typeof(int)
			? (int)info.GetValue(targetObject)
			: (float)info.GetValue(targetObject);
	}
}
}