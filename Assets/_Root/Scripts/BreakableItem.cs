using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BreakableItem : MonoBehaviour
{
	public enum _itemStates
	{
		Undamaged,
		Damaged,
		Broken
	}

	[SerializeField]
	private HeldItemSlot _playerHeldItem;

	[SerializeField]
	private int _itemHp;

	[SerializeField]
	private int _itemMaxHp;

	[SerializeField]
	private string _effectiveItemType;

	[SerializeField]
	private Vector3 _afterBreakOffset;
	[SerializeField]
	private bool _Log;

	public CollideCheck itemCollision;

	public PlayerInput playerInput;

	[SerializeField]
	private List<Sprite> _sprites;

	public string itemType;
	public string eventType;

	public _itemStates _damageState = _itemStates.Undamaged;

	[SerializeField]
	private string _PersistentID;
	public EndGameTracker m_EndGameTracker;
	private InputAction _breakableAction;
	private GameManager _GameManager;
	private string _heldItemType;
	public string m_PersistentID => _PersistentID;


	private void Awake()
	{
		_GameManager = GameObject.FindGameObjectWithTag("Manager")
			.GetComponent<GameManager>();
		m_EndGameTracker = FindFirstObjectByType<EndGameTracker>();
	}

#if UNITY_EDITOR
	private void Reset()
	{
		// Assign a unique ID if it's empty.
		if (string.IsNullOrEmpty(_PersistentID))
			_PersistentID = Guid.NewGuid().ToString();
	}
#endif

	private void Start()
	{
		playerInput = _GameManager.m_Player.GetComponent<PlayerInput>();
		_breakableAction = playerInput.actions["Break"];
		if (_breakableAction == null) Debug.LogError("No break action found");

		if (!m_EndGameTracker._DestroyedItems.ContainsKey(_PersistentID))
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
}