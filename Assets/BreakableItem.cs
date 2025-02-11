using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

public class BreakableItem : MonoBehaviour
{
	public HeldItemSlot playerHeldItem;

	public int itemHp;
	public CollideCheck itemCollision;
	private string _heldItemType;
	public string effectiveItemType;

	public PlayerInput playerInput;
	private InputAction _breakableAction;

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
		_heldItemType = playerHeldItem.playerHeldItem;
	}

	private void Update()
	{
		if (_breakableAction.WasPressedThisFrame() && itemCollision.IsCollided)
		{
			Debug.Log("Damage Called");
			Damage();
		}
		else if (Input.GetKeyDown(KeyCode.E))
			Debug.Log("Damage failed to call, no collision detected");
	}

	private void Damage()
	{
		itemHp = itemHp - 1;
		Debug.Log(transform.name + " took 1 damage - New HP = " + itemHp);

		if (itemHp <= 0) Destroy(gameObject);
	}
}