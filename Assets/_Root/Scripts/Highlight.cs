using EditorAttributes;
using GlueTrap.Utilities;
using UnityEngine;

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
	[SerializeField, ReadOnly]
	private Material _Material;
	[SerializeField, ReadOnly, ColorUsage(true, true)]
	private Color _DefaultOutlineColour;
	[SerializeField, ReadOnly]
	private float _DefaultOutlineThickness;

	[Header("Debug Options"), SerializeField,
	 ButtonField(nameof(GetRef), "Get References")]
	private Void _ButtonHolder;
	[SerializeField, ButtonField(nameof(Hide))]
	private Void _ButtonHolder2;
	[SerializeField, ButtonField(nameof(Show))]
	private Void _ButtonHolder3;

	private GameManager _GameManager;


	private void Awake()
	{
		_GameManager = Utils.GetGameManager();
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
		_DefaultOutlineColour = _Material.GetColor(s_OutlineColour);
		_DefaultOutlineThickness = _Material.GetFloat(s_OutlineThickness);
	}

	private void Start()
	{
		Hide();
	}

	private void Update()
	{
		// Ignore if used by composite.
		if (m_UsedByComposite) return;

		Vector2 mousePos =
			_GameManager.m_Camera.ScreenToWorldPoint(Input.mousePosition);
		RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero,
			Mathf.Infinity, LayerMask.GetMask("Highlighter"));

		if (hit.collider && hit.collider == GetComponent<Collider2D>())
			Show();
		else
			Hide();
	}


	private void GetRef()
	{
		Awake();
	}

	private void Hide()
	{
		_Material.SetColor(s_OutlineColour, Color.black);
		_Material.SetFloat(s_OutlineThickness, 0f);
	}

	private void Show()
	{
		_Material.SetColor(s_OutlineColour, _DefaultOutlineColour);
		_Material.SetFloat(s_OutlineThickness, _DefaultOutlineThickness);
	}
}
}