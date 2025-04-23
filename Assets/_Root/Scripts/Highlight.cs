using System;
using EditorAttributes;
using GlueTrap.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;
using Void = EditorAttributes.Void;

namespace GlueTrap
{
[RequireComponent(typeof(PolygonCollider2D))]
public class Highlight : MonoBehaviour
{
	private static readonly int s_OutlineColour =
		Shader.PropertyToID("_Outline_Colour");
	private static readonly int s_OutlineThickness =
		Shader.PropertyToID("_Outline_Thickness");

	public bool m_UsedByComposite;

	[SerializeField,
	 Tooltip("If the script should grab references from the child object.")]
	private bool _OnChild;
	[SerializeField,
	 Tooltip("If the script should grab references from the object's parent.")]
	private bool _OnParent;
	[NonSerialized]
	public bool _IsController;
	[NonSerialized]
	public bool _PlayerColliding;
	[SerializeField, ReadOnly]
	private Material _Material;
	[SerializeField, ReadOnly, ColorUsage(true, true)]
	private Color _DefaultOutlineColour;
	[SerializeField, ReadOnly]
	private float _DefaultOutlineThickness;
	[SerializeField]
	private bool _UseCustomValues;
	[SerializeField, EnableField(nameof(_UseCustomValues)),
	 ColorUsage(true, true)]
	private Color _CustomShowOutlineColour;
	[SerializeField, EnableField(nameof(_UseCustomValues))]
	private float _CustomShowOutlineThickness = 1f;

	[Header("Debug Options"), SerializeField,
	 ButtonField(nameof(GetReference), "Get References")]
	private Void _ButtonHolder;
	[SerializeField, ButtonField(nameof(Hide))]
	private Void _ButtonHolder2;
	[SerializeField, ButtonField(nameof(Show))]
	private Void _ButtonHolder3;

	private GameManager _GameManager;


	private void Awake()
	{
		_GameManager = Utils.GetGameManager();
		GetReference();
	}

	private void Start()
	{
		Hide();
	}

	private void Update()
	{
		// Ignore if used by composite or is not moving, ignore.
		if (m_UsedByComposite) return;
		// Only allow highlighting when not talking or in menus.
		if (_GameManager.m_CurrentState != States.Moving)
		{
			Hide();
			return;
		}

		Vector2 mousePos =
			_GameManager.m_Camera.ScreenToWorldPoint(Input.mousePosition);
		var colliderComp = GetComponent<Collider2D>();

		_IsController = Gamepad.current != null;
		if (!_IsController)
		{
			if (colliderComp.OverlapPoint(mousePos))
				Show();
			else
				Hide();
		}
		else
		{
			if (_PlayerColliding)
				Show();
			else
				Hide();
		}
	}
		


	public void GetReference()
	{
		// Obtain the material.
		Renderer rendererComp;
		if (_OnParent)
		{
			rendererComp = GetComponentInParent<Renderer>();
			if (!rendererComp)
			{
				Debug.LogError(
					$"Either, no Renderer component attached to the {name}'s " +
					$"parent, or {name} has no parent!");
				return;
			}
		}
		else if (_OnChild)
		{
			rendererComp = GetComponentInChildren<Renderer>();
			if (!rendererComp)
			{
				Debug.LogError(
					"Either, no Renderer component attached to any of the " +
					$"{name}'s children, or {name} has no children!");
				return;
			}
		}
		else
		{
			rendererComp = GetComponent<Renderer>();
			if (!rendererComp)
			{
				Debug.LogError($"{name} has no Renderer component!");
				return;
			}
		}

		_Material = rendererComp.material;

		// Ensure the material grabbed has the required properties.
		if (rendererComp.material.HasProperty(s_OutlineColour))
			_DefaultOutlineColour = _Material.GetColor(s_OutlineColour);
		if (rendererComp.material.HasProperty(s_OutlineThickness))
			_DefaultOutlineThickness = _Material.GetFloat(s_OutlineThickness);
	}

	public void Hide()
	{
		_Material.SetColor(s_OutlineColour, Color.black);
		_Material.SetFloat(s_OutlineThickness, 0f);
	}

	public void Show()
	{
		_Material.SetColor(s_OutlineColour,
			_UseCustomValues
				? _CustomShowOutlineColour
				: _DefaultOutlineColour);
		_Material.SetFloat(s_OutlineThickness,
			_UseCustomValues
				? _CustomShowOutlineThickness
				: _DefaultOutlineThickness);
	}
}
}