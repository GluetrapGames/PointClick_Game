using System;
using System.Collections.Generic;
using EditorAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GlueTrap
{
public class BreakableItem : MonoBehaviour
{
	public enum _itemStates
	{
		Undamaged,
		Damaged,
		Broken
	}

	public string itemType;
	public string eventType;
	public CollideCheck itemCollision;
	public PlayerInput playerInput;
	public _itemStates _damageState = _itemStates.Undamaged;

	[SerializeField, ReadOnly]
	private string _PersistentID;
	[SerializeField]
	private bool _Log;
	[SerializeField]
	private int _itemMaxHp;
	[SerializeField,
	 ProgressBar(nameof(_itemMaxHp), 0.8f, 0f, 0f)]
	private int _itemHp;
	[SerializeField]
	private string _effectiveItemType;
	[SerializeField]
	private Vector3 _afterBreakOffset;
	[SerializeField]
	private List<Sprite> _sprites;
	private InputAction _breakableAction;
	private EndGameTracker _EndGameTracker;

	private GameManager _GameManager;
	private string _heldItemType;
	private HeldItemSlot _playerHeldItem;

	public string m_PersistentID => _PersistentID;


	private void Awake()
	{
		_GameManager = GameObject.FindGameObjectWithTag("Manager")
			.GetComponent<GameManager>();
		_EndGameTracker = FindFirstObjectByType<EndGameTracker>();
	}

	private void Start()
	{
		_playerHeldItem = _GameManager.m_InventoryManager.m_HeldItemSlot;
		playerInput = _GameManager.m_Player.GetComponent<PlayerInput>();
		_breakableAction = playerInput.actions["Break"];
		if (_breakableAction == null) Debug.LogError("No break action found");

		if (!_EndGameTracker._DestroyedItems.ContainsKey(_PersistentID))
			return;
		_itemHp = 0;
		_damageState = _itemStates.Broken;
		SpriteSwap(_damageState);
	}

	private void Update()
	{
		if (_breakableAction.WasPressedThisFrame() && itemCollision.IsCollided)
		{
			if (_Log) Debug.Log("Damage Called");
			Damage();
			AkSoundEngine.SetSwitch("BreakMaterial", itemType, gameObject);
			AkSoundEngine.PostEvent(eventType, gameObject);
		}
		else if (_breakableAction.WasPressedThisFrame() &&
		         !itemCollision.IsCollided)
			Debug.Log("Damage failed to call, no collision detected");
	}

	private void FixedUpdate()
	{
		if (_playerHeldItem == null)
			return;
		_heldItemType = _playerHeldItem.playerHeldItem;
	}

	private void Damage()
	{
		if (_playerHeldItem.playerHeldItem != _effectiveItemType)
		{
			_itemHp = _itemHp - 1;
			if (_Log)
			{
				Debug.Log(transform.name + " took 1 damage - New HP = " +
				          _itemHp);
			}
		}
		else
		{
			_itemHp = _itemHp - 2;
			if (_Log)
			{
				Debug.Log(transform.name +
				          " took 2 damage from effective item " +
				          _playerHeldItem.playerHeldItem + " - New HP = " +
				          _itemHp);
			}
		}

		if (_itemHp <= 0)
			_damageState = _itemStates.Broken;
		else
			_damageState = _itemStates.Damaged;
		SpriteSwap(_damageState);
	}

	private void SpriteSwap(_itemStates state)
	{
		switch (state)
		{
			case _itemStates.Damaged:
				gameObject.GetComponent<SpriteRenderer>().sprite = _sprites[0];
				break;
			case _itemStates.Broken:
				gameObject.GetComponent<SpriteRenderer>().sprite = _sprites[1];
				gameObject.GetComponent<BoxCollider2D>().enabled = false;
				gameObject.transform.position -= _afterBreakOffset;
				break;
		}
	}

#if UNITY_EDITOR
	private void Reset()
	{
		// Assign a unique ID if it's empty.
		if (string.IsNullOrEmpty(_PersistentID))
			_PersistentID = Guid.NewGuid().ToString();
	}

	private void OnValidate()
	{
		_itemHp = _itemMaxHp;
	}
#endif
}
}