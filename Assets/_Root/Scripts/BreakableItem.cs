using PixelCrushers.DialogueSystem;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

public class BreakableItem : MonoBehaviour
{
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
	
	public CollideCheck itemCollision;
	
	private string _heldItemType;
	
	public PlayerInput playerInput;
	
	private InputAction _breakableAction;

	[SerializeField]
	private List<Sprite> _sprites;

	public string itemType;
	public string eventType;
	
	private enum _itemStates
	{
		Undamaged,
		Damaged,
		Broken
	}
	
	private _itemStates _damageState = _itemStates.Undamaged;
	
	private void Awake()
	{
		_breakableAction = playerInput.actions["Break"];
		if (_breakableAction == null)
		{
			Debug.LogError("No break action found");
		}
	}

	private void FixedUpdate()
	{
		if (_playerHeldItem == null)
		{
			return;
		}
		else
		{
			_heldItemType = _playerHeldItem.playerHeldItem;
		}
	}

	private void Update()
	{
		if (_breakableAction.WasPressedThisFrame() && itemCollision.IsCollided)
		{
			Debug.Log("Damage Called");
			Damage();
			AkSoundEngine.SetSwitch("BreakMaterial", itemType, gameObject);
			AkSoundEngine.PostEvent(eventType, gameObject);
		}
		else if (_breakableAction.WasPressedThisFrame() && !itemCollision.IsCollided)
			Debug.Log("Damage failed to call, no collision detected");
	}

	private void Damage()
	{
		if (_playerHeldItem.playerHeldItem != _effectiveItemType)
		{
			_itemHp = _itemHp - 1;
			Debug.Log(transform.name + " took 1 damage - New HP = " + _itemHp);

		}else
		{
			_itemHp = _itemHp - 2;
			Debug.Log(transform.name + " took 2 damage from effective item " + _playerHeldItem.playerHeldItem + " - New HP = " + _itemHp);
		}
		
		if (_itemHp <= 0)
		{
			_damageState = _itemStates.Broken;
		}
		else
		{
			_damageState = _itemStates.Damaged;
		}
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