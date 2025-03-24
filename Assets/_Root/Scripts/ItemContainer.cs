using System.Collections.Generic;
using GlueTrap.Scriptable_Objects;
using GlueTrap.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GlueTrap
{
public class ItemContainer : MonoBehaviour
{
	[SerializeField]
	private CollectableItemData _CollectableItemData;
	[Header("Dialogue Settings"), Tooltip("Will the item start dialogue."),
	 SerializeField]
	private bool _StartConversation;
	[Tooltip("The GameObject That has the conversation trigger."),
	 SerializeField]
	private GameObject _ConversationObject;

	private CollideCheck _CollideCheck;
	private Transform _ContainerInventory;
	private InventorySlot _ContainerSlot;
	private GameManager _GameManager;
	private InputAction _InteractionAction;


	private void Awake()
	{
		_GameManager = Utils.GetGameManager();
		_CollideCheck = GetComponent<CollideCheck>();
		var playerInput = _GameManager.m_Player.GetComponent<PlayerInput>();

		_InteractionAction = playerInput.actions["Break"];
		if (_InteractionAction == null) Debug.LogError("No break action found");
	}

	private void Start()
	{
		// Obtain the Container Inventory object.
		var inventoryObjs = GameObject.FindGameObjectsWithTag("Inventory");
		foreach (GameObject obj in inventoryObjs)
			if (obj.name.Contains("Container"))
				_ContainerInventory = obj.transform.GetChild(0);

		// Null Check.
		if (!_ContainerInventory)
			Debug.LogWarning("No Container Inventory found!");

		// Obtain the inventory slot from the container.
		List<InventorySlot> slots = new();
		Utils.FindChildrenByType<InventorySlot, InventorySlot>(
			_ContainerInventory, slots,
			c => c);

		// Check if any exist.
		if (slots.Count > 0)
			_ContainerSlot = slots[0];
		else
			Debug.LogWarning("No Inventory Slot found in the container UI!");
	}

	private void Update()
	{
		if (_InteractionAction.WasPressedThisFrame() &&
		    _CollideCheck.IsCollided)
		{
			_ContainerInventory.gameObject.SetActive(
				!_ContainerInventory.gameObject.activeInHierarchy);
		}
	}
}
}