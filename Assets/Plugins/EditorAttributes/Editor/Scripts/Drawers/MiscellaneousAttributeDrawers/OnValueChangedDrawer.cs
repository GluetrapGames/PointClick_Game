using System.Reflection;
using EditorAttributes.Editor.Utility;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace EditorAttributes.Editor
{
[CustomPropertyDrawer(typeof(OnValueChangedAttribute))]
public class OnValueChangedDrawer : PropertyDrawerBase
{
	public override VisualElement CreatePropertyGUI(SerializedProperty property)
	{
		var onValueChangedAttribute = attribute as OnValueChangedAttribute;

		// Get both the method and the correct target instance.
		Object target = property.serializedObject.targetObject;
		ReflectionUtility.GetNestedObjectType(property, out var nestedTarget);

		if (nestedTarget is Object unityObject)
			target = unityObject;

		var root = new VisualElement();
		var propertyField = new PropertyField(property);

		MethodInfo function =
			ReflectionUtility.FindFunction(onValueChangedAttribute.FunctionName,
				target);

		if (function != null && function.GetParameters().Length == 0)
			propertyField.RegisterValueChangeCallback(_ =>
				function.Invoke(target, null));
		else
			root.Add(new HelpBox("Function must exist and have no parameters.",
				HelpBoxMessageType.Error));

		root.Add(propertyField);
		return root;
	}
}
}