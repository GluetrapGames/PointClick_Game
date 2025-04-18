using GlueTrap.Utilities;
using PixelCrushers.DialogueSystem;
using UnityEngine;

namespace GlueTrap
{
public class RecordPlayer : MonoBehaviour
{
	private bool _CanInteract;
	private GameManager _GameManager;
	private bool _HasInteraction;
	private InventoryItem _HeldItem;
	private bool _HideHighlight = true;
	private BreakableItem _RecordPlayerBreakableRef;
	private Highlight _RecordPlayerHighlightRef;
	private InteractDialgoue _RecordPlayerInteractRef;
	private PolygonCollider2D _RecordPlayerPolygonColliderRef;
	private SpriteRenderer _RecordPlayerSpriteRef;
	private bool isPlaying = false;

	private void Awake()
	{
		_GameManager = Utils.GetGameManager();
		_RecordPlayerHighlightRef = GetComponentInChildren<Highlight>();
		_RecordPlayerPolygonColliderRef =
			GetComponentInChildren<PolygonCollider2D>();
		_RecordPlayerBreakableRef = GetComponent<BreakableItem>();
		_RecordPlayerInteractRef = GetComponent<InteractDialgoue>();
		_RecordPlayerSpriteRef = GetComponent<SpriteRenderer>();

		_RecordPlayerInteractRef.enabled = false;
	}

	private void Update()
	{
		if (_HideHighlight) _RecordPlayerHighlightRef.Hide();

		if (_HasInteraction)
		{
			_RecordPlayerHighlightRef.enabled = false;
			_RecordPlayerPolygonColliderRef.enabled = false;
			return;
		}

		;
		_CanInteract = DialogueLua.GetVariable("Has_Record").asBool;

		if (!_CanInteract) return;
		_HideHighlight = false;
		_RecordPlayerInteractRef.enabled = true;

		if (!_RecordPlayerInteractRef.m_Interacting) return;
		Interaction();
	}

	private void Interaction()
	{
		_HeldItem = _GameManager.m_InventoryManager.m_HeldItemSlot
			.GetComponentInChildren<InventoryItem>();

		if (!_HeldItem) return;
		if (!_RecordPlayerBreakableRef || _RecordPlayerBreakableRef._itemHp !=
		    _RecordPlayerBreakableRef._itemMaxHp) return;
		if (_HeldItem.itemData.m_Item.m_Type != ItemTypes.Record) return;

		Destroy(_HeldItem.gameObject);
		Debug.Log("Record Interaction");
		_HasInteraction = true;
		_HideHighlight = true;
	}
}
}