using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using EditorAttributes.Editor.Utility;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace EditorAttributes.Editor
{
public class ButtonDrawer
{
	internal const string PARAMS_DATA_LOCATION =
		"ProjectSettings/EditorAttributes";

	#region MAIN ENTRY

	internal static VisualElement DrawButton(MethodInfo function,
		ButtonAttribute buttonAttribute, Dictionary<MethodInfo, bool> foldouts,
		Dictionary<MethodInfo, object[]> parameterValues, object target)
	{
		var root = new VisualElement();
		var parameters = function.GetParameters();

		if (parameters.Length > 0)
		{
			PropertyDrawerBase.ApplyBoxStyle(root);

			if (!parameterValues.ContainsKey(function))
				parameterValues[function] =
					parameters.Select(p => p.DefaultValue).ToArray();

			foldouts.TryAdd(function, true);

			VisualElement button = MakeButton(function, buttonAttribute, () =>
			{
				var paramList = parameters
					.Select((p, i) => ConvertParameterValue(p.ParameterType,
						parameterValues[function][i]))
					.ToArray();

				function.Invoke(target, paramList);
			});

			var foldout = new Foldout
			{
				text = "Parameters",
				value = foldouts[function],
				style =
				{
					unityFontStyleAndWeight = FontStyle.Bold,
					paddingLeft = 15f
				}
			};

			PropertyDrawerBase.ApplyBoxStyle(foldout);
			ApplyGlobalColour(button);
			ApplyGlobalColour(foldout);

			foldout.RegisterValueChangedCallback(evt =>
				foldouts[function] = evt.newValue);

			for (var i = 0; i < parameters.Length; i++)
			{
				ParameterInfo param = parameters[i];
				VisualElement field = DrawParameterField(param.ParameterType,
					param.Name, parameterValues[function][i]);
				ApplyGlobalColour(field);

				var index = i;
				RegisterParameterFieldValueChangedCallback(field,
					param.ParameterType,
					val => parameterValues[function][index] = val);

				field.style.unityFontStyleAndWeight = FontStyle.Normal;
				foldout.Add(field);
			}

			root.Add(button);
			root.Add(foldout);
		}
		else
		{
			VisualElement button = MakeButton(function, buttonAttribute,
				() => function.Invoke(target, null));
			root.Add(button);
		}

		return root;
	}

	#endregion

	#region TYPE CONVERSION

	private static object ConvertParameterValue(Type type, object value)
	{
		var isDbNull = Convert.IsDBNull(value);

		return type switch
		{
			_ when type == typeof(string) => value?.ToString(),
			_ when type == typeof(int) => isDbNull ? 0 : Convert.ToInt32(value),
			_ when type == typeof(uint) => isDbNull
				? 0u
				: Convert.ToUInt32(value),
			_ when type == typeof(long) => isDbNull
				? 0L
				: Convert.ToInt64(value),
			_ when type == typeof(ulong) => isDbNull
				? 0UL
				: Convert.ToUInt64(value),
			_ when type == typeof(float) => isDbNull
				? 0f
				: Convert.ToSingle(value),
			_ when type == typeof(double) => isDbNull
				? 0.0
				: Convert.ToDouble(value),
			_ when type == typeof(bool) => !isDbNull && (bool)value,
			_ when type.IsEnum => Enum.ToObject(type, value ?? 0),
			_ when type == typeof(Vector2) => isDbNull
				? Vector2.zero
				: ParseFromJson<Vector2>(value),
			_ when type == typeof(Vector2Int) => isDbNull
				? Vector2Int.zero
				: ParseFromJson<Vector2Int>(value),
			_ when type == typeof(Vector3) => isDbNull
				? Vector3.zero
				: ParseFromJson<Vector3>(value),
			_ when type == typeof(Vector3Int) => isDbNull
				? Vector3Int.zero
				: ParseFromJson<Vector3Int>(value),
			_ when type == typeof(Vector4) => isDbNull
				? Vector4.zero
				: ParseFromJson<Vector4>(value),
			_ when type == typeof(Color) => isDbNull
				? Color.black
				: ParseFromJson<Color>(value),
			_ when type == typeof(Gradient) => new Gradient(),
			_ when type == typeof(AnimationCurve) => AnimationCurve.Linear(0f,
				0f, 1f, 1f),
			_ when type == typeof(LayerMask) => (LayerMask)(isDbNull
				? 0
				: Convert.ToInt32(value)),
			_ when type == typeof(Rect) => new Rect(0, 0, 0, 0),
			_ when type == typeof(RectInt) => new RectInt(0, 0, 0, 0),
			_ when type == typeof(Bounds) => new Bounds(Vector3.zero,
				Vector3.zero),
			_ when type == typeof(BoundsInt) => new BoundsInt(Vector3Int.zero,
				Vector3Int.zero),
			_ => null
		};
	}

	#endregion

	#region INTERNAL DATA STRUCTURE

	[Serializable]
	private class FunctionParamData
	{
		public Dictionary<string, bool> foldouts = new();
		public Dictionary<string, object[]> parameterValues = new();
	}

	#endregion

	#region BUTTON CREATION

	private static VisualElement MakeButton(MethodInfo function,
		ButtonAttribute buttonAttribute, Action onClick)
	{
		var label = string.IsNullOrWhiteSpace(buttonAttribute.ButtonLabel)
			? function.Name
			: buttonAttribute.ButtonLabel;
		var tooltip =
			function.GetCustomAttribute<TooltipAttribute>()?.tooltip ??
			string.Empty;

		if (buttonAttribute.IsRepetable)
		{
			var repeatButton = new RepeatButton(onClick,
				buttonAttribute.PressDelay, buttonAttribute.RepetitionInterval)
			{
				text = label,
				tooltip = tooltip,
				style = { height = buttonAttribute.ButtonHeight }
			};

			repeatButton.AddToClassList(Button.ussClassName);
			return repeatButton;
		}

		return new Button(onClick)
		{
			text = label,
			tooltip = tooltip,
			style = { height = buttonAttribute.ButtonHeight }
		};
	}

	private static void ApplyGlobalColour(VisualElement element)
	{
		if (EditorExtension.GLOBAL_COLOR !=
		    EditorExtension.DEFAULT_GLOBAL_COLOR)
			element.style.color = EditorExtension.GLOBAL_COLOR;
	}

	#endregion

	#region SERIALISATION

	internal static void SaveParamsData(MethodInfo[] functions, object target,
		Dictionary<MethodInfo, bool> foldouts,
		Dictionary<MethodInfo, object[]> parameterValues)
	{
		var data = new FunctionParamData();

		foreach (MethodInfo function in functions)
		{
			if (!IsButtonFunction(function, out var serialisable) ||
			    !serialisable)
				continue;

			var id = GetFunctionID(function, target);
			if (foldouts.TryGetValue(function, out var foldoutVal))
				data.foldouts[id] = foldoutVal;
			if (parameterValues.TryGetValue(function, out var paramVal))
				data.parameterValues[id] = paramVal;
		}

		if (data.foldouts.Count == 0 && data.parameterValues.Count == 0)
			return;

		JsonConvert.DefaultSettings = () => new JsonSerializerSettings
		{
			Converters = { new UnityTypeConverter() }
		};

		var json = JsonConvert.SerializeObject(data, Formatting.Indented);
		File.WriteAllTextAsync(
			Path.Combine(PARAMS_DATA_LOCATION, $"{target}ParamsData.json"),
			json);
	}

	internal static void LoadParamsData(MethodInfo[] functions, object target,
		ref Dictionary<MethodInfo, bool> foldouts,
		ref Dictionary<MethodInfo, object[]> parameterValues)
	{
		if (!Directory.Exists(PARAMS_DATA_LOCATION))
			Directory.CreateDirectory(PARAMS_DATA_LOCATION);

		var filePath =
			Path.Combine(PARAMS_DATA_LOCATION, $"{target}ParamsData.json");
		if (!File.Exists(filePath)) return;

		try
		{
			var json = File.ReadAllText(filePath);
			var data = JsonConvert.DeserializeObject<FunctionParamData>(json);

			foreach (MethodInfo function in functions)
			{
				if (!IsButtonFunction(function, out var serialisable) ||
				    !serialisable) continue;

				var id = GetFunctionID(function, target);
				if (!data.foldouts.ContainsKey(id)) continue;

				foldouts[function] = data.foldouts[id];
				parameterValues[function] = data.parameterValues[id];
			}
		}
		catch (ArgumentException)
		{
			// Silently ignore malformed data.
		}
	}

	internal static void DeleteParamsData(string path)
	{
		if (File.Exists(path))
			File.Delete(path);
	}

	internal static string GetFunctionID(MethodInfo function, object target)
	{
		return
			$"{target}_{function.Name}_{string.Join("_", function.GetParameters().Select(p => p.ParameterType.Name))}";
	}

	internal static bool IsButtonFunction(MethodInfo function,
		out bool serialiseParams)
	{
		var attr = function.GetCustomAttribute<ButtonAttribute>();
		serialiseParams = attr?.SerializeParameters ?? false;
		return attr != null;
	}

	public static T ParseFromJson<T>(object value)
	{
		if (value == null) return default;

		try
		{
			return (T)value;
		}
		catch (InvalidCastException)
		{
			try
			{
				return JsonConvert.DeserializeObject<T>(value.ToString());
			}
			catch
			{
				return default;
			}
		}
	}

	#endregion

	#region PARAMETER FIELD DRAWING

	internal static VisualElement DrawParameterField(Type fieldType,
		string name, object value)
	{
		name = ObjectNames.NicifyVariableName(name);
		var val = ConvertParameterValue(fieldType, value);

		return fieldType switch
		{
			_ when fieldType == typeof(string) => new TextField(name)
				{ value = (string)val },
			_ when fieldType == typeof(int) => new IntegerField(name)
				{ value = (int)val },
			_ when fieldType == typeof(uint) => new UnsignedIntegerField(name)
				{ value = (uint)val },
			_ when fieldType == typeof(long) => new LongField(name)
				{ value = (long)val },
			_ when fieldType == typeof(ulong) => new UnsignedLongField(name)
				{ value = (ulong)val },
			_ when fieldType == typeof(float) => new FloatField(name)
				{ value = (float)val },
			_ when fieldType == typeof(double) => new DoubleField(name)
				{ value = (double)val },
			_ when fieldType == typeof(bool) => new Toggle(name)
				{ value = (bool)val },
			_ when fieldType.IsEnum => new EnumField(name, (Enum)val),
			_ when fieldType == typeof(Vector2) => new Vector2Field(name)
				{ value = (Vector2)val },
			_ when fieldType == typeof(Vector2Int) => new Vector2IntField(name)
				{ value = (Vector2Int)val },
			_ when fieldType == typeof(Vector3) => new Vector3Field(name)
				{ value = (Vector3)val },
			_ when fieldType == typeof(Vector3Int) => new Vector3IntField(name)
				{ value = (Vector3Int)val },
			_ when fieldType == typeof(Vector4) => new Vector4Field(name)
				{ value = (Vector4)val },
			_ when fieldType == typeof(Color) => new ColorField(name)
				{ value = (Color)val },
			_ when fieldType == typeof(Gradient) => new GradientField(name)
				{ value = (Gradient)val },
			_ when fieldType == typeof(AnimationCurve) => new CurveField(name)
				{ value = (AnimationCurve)val },
			_ when fieldType == typeof(LayerMask) => new LayerMaskField(name,
				(LayerMask)val),
			_ when fieldType == typeof(Rect) => new RectField(name)
				{ value = (Rect)val },
			_ when fieldType == typeof(RectInt) => new RectIntField(name)
				{ value = (RectInt)val },
			_ when fieldType == typeof(Bounds) => new BoundsField(name)
				{ value = (Bounds)val },
			_ when fieldType == typeof(BoundsInt) => new BoundsIntField(name)
				{ value = (BoundsInt)val },
			_ => new HelpBox($"Type {fieldType.Name} is not supported.",
				HelpBoxMessageType.Error)
		};
	}

	private static void RegisterParameterFieldValueChangedCallback(
		VisualElement field, Type type, Action<object> onChange)
	{
		MethodInfo method = typeof(ButtonDrawer)
			.GetMethod("RegisterTypedCallback",
				BindingFlags.NonPublic | BindingFlags.Static)
			?.MakeGenericMethod(type);

		method?.Invoke(null, new object[] { field, onChange });
	}

	private static void RegisterTypedCallback<T>(VisualElement field,
		Action<object> callback)
	{
		field.RegisterCallback<ChangeEvent<T>>(e => callback(e.newValue));
	}

	#endregion
}
}