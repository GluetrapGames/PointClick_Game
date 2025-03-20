using System;
using System.Collections.Generic;
using EditorAttributes;
using GlueTrap.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GlueTrap
{
public class BreakableItem : MonoBehaviour
{
	[SerializeField]
	private string _PersistentID;
	[SerializeField]
	private bool _Log;
	[SerializeField, ReadOnly]
	private ItemDamageStates _DamageState = ItemDamageStates.Undamaged;
	[SerializeField]
	private int _itemMaxHp;
	[SerializeField, ProgressBar(nameof(_itemMaxHp), 0.8f, 0f, 0f)]
	private int _itemHp;
	[SerializeField]
	private ItemTypes _ItemType;
	[SerializeField]
	private ItemTypes _effectiveItemType;
	[SerializeField]
	private Vector3 _afterBreakOffset;
	[SerializeField]
	private List<Sprite> _sprites;
	[SerializeField]
	private EventTypes _EventType;

	private InputAction _breakableAction;
	private EndGameTracker _EndGameTracker;
	private GameManager _GameManager;
	private ItemTypes _heldItemType;
	private CollideCheck _ItemCollision;
	private HeldItemSlot _playerHeldItem;
	private PlayerInput _PlayerInput;

	public string m_PersistentID => _PersistentID;
	public ItemDamageStates m_DamageState => _DamageState;


	private void Awake()
	{
		// Obtain Game Manager.
		_GameManager = Utils.GetGameManager();

		_ItemCollision = GetComponent<CollideCheck>();
	}

	private void Start()
	{
		_EndGameTracker = _GameManager.m_EndGameTracker;

		_playerHeldItem = _GameManager.m_InventoryManager.m_HeldItemSlot;
		_PlayerInput = _GameManager.m_Player.GetComponent<PlayerInput>();

		_breakableAction = _PlayerInput.actions["Break"];
		if (_breakableAction == null) Debug.LogError("No break action found");

		if (!_EndGameTracker._DestroyedItems.ContainsKey(_PersistentID))
			return;
		_itemHp = 0;
		_DamageState = ItemDamageStates.Broken;
		SpriteSwap(_DamageState);
	}

	private void Update()
	{
		if (!_playerHeldItem) return;
		if (_playerHeldItem.playerHeldItem != null)
			_heldItemType = _playerHeldItem.playerHeldItem.m_Item.m_Type;

		if (_breakableAction.WasPressedThisFrame() && _ItemCollision.IsCollided)
		{
			if (_Log) Debug.Log("Damage Called");
			Damage();
			AkSoundEngine.SetSwitch("BreakMaterial", _ItemType.ToString(),
				gameObject);
			AkSoundEngine.PostEvent(_EventType.ToString(), gameObject);
		}
		else if (_breakableAction.WasPressedThisFrame() &&
		         !_ItemCollision.IsCollided)
			Debug.Log("Damage failed to call, no collision detected");
	}

	private void Damage()
	{
		// Normal amount of damage if not held item or held item is ineffective.
		_heldItemType = _playerHeldItem.playerHeldItem != null
			? _playerHeldItem.playerHeldItem.m_Item.m_Type
			: ItemTypes.None;
		if (_heldItemType != _effectiveItemType ||
		    _heldItemType == ItemTypes.None)
		{
			_itemHp--;
			if (_Log)
				Debug.Log($"{name} took 1 damage! New HP = {_itemHp}");
		}
		else
		{
			_itemHp -= 2;
			if (_Log)
			{
				Debug.Log(
					$"{name} took 2 damage from effective item " +
					$"{_playerHeldItem.playerHeldItem}! New HP = {_itemHp}");
			}
		}

		_DamageState = _itemHp <= 0
			? ItemDamageStates.Broken
			: ItemDamageStates.Damaged;
		SpriteSwap(_DamageState);
	}

	private void SpriteSwap(ItemDamageStates damageState)
	{
		switch (damageState)
		{
			case ItemDamageStates.Damaged:
				gameObject.GetComponent<SpriteRenderer>().sprite = _sprites[0];
				break;
			case ItemDamageStates.Broken:
				gameObject.GetComponent<SpriteRenderer>().sprite = _sprites[1];
				gameObject.GetComponent<BoxCollider2D>().enabled = false;
				gameObject.transform.position -= _afterBreakOffset;
				break;
		}
	}

	[Button("Generate Persistent ID")]
	private void GenerateID()
	{
		_PersistentID = Guid.NewGuid().ToString();
	}

#if UNITY_EDITOR
	private void Reset()
	{
		// Assign a unique ID if it's empty.
		if (string.IsNullOrEmpty(_PersistentID))
			GenerateID();
	}

	private void OnValidate()
	{
		// Update current Health to new max health.
		_itemHp = _itemMaxHp;
	}
#endif
}
}