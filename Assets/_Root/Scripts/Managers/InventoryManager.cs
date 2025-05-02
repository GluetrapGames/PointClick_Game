using System.Collections.Generic;
using System.Linq;
using GlueTrap.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GlueTrap
{
public class InventoryManager : Singleton<InventoryManager>
{
	public DropHeldItem m_DropHeldItem { get; private set; }
	public HeldItemSlot m_HeldItemSlot { get; private set; }
	public List<Transform> m_InventorySlots { get; } = new();
	public Transform m_Inventory { get; private set; }
	[SerializeField]
	public bool m_Log;

	public Dictionary<string, InventoryItemData> m_InventoryItems = new();
	[SerializeField]
	private GameObject _InventoryPrefab;
	[SerializeField]
	private GameObject _ItemPrefab;

	private GameManager _GameManager;


	protected override void Awake()
	{
		base.Awake();
		_GameManager = Utils.GetGameManager();
		// Get the Inventory.
		GameObject inventoryObject = GameObject.FindWithTag("Inventory");
		if (!inventoryObject)
		{
			Debug.LogWarning(
				"Cannot find 'Inventory Canvas' in the scene.");
			inventoryObject = Instantiate(_InventoryPrefab, transform);
		}

		m_DropHeldItem =
			FindFirstObjectByType<DropHeldItem>(FindObjectsInactive.Include);

		m_Inventory = inventoryObject.transform;
		GetInventory();
	}

	private void OnEnable()
	{
		_GameManager.m_OnGameReset.AddListener(OnGameReset);
	}

	private void OnDisable()
	{
		_GameManager.m_OnGameReset.RemoveListener(OnGameReset);
	}

	public bool CollectItem(ItemData itemData)
	{
		var inventoryItemData =
			new InventoryItemData(itemData, false, false, null);

		var slotFound = false;
		// Try to find a available inventory slot.
		if (m_Log) Debug.Log("Finding Slot...");
		foreach (Transform itemSlot in m_InventorySlots.Where(itemSlot =>
			         itemSlot.childCount == 0))
		{
			if (m_Log) Debug.Log("Found a Slot!");
			inventoryItemData.m_Slot =
				itemSlot.GetComponent<InventorySlot>();
			slotFound = true;
			break;
		}

		if (!slotFound)
		{
			if (m_Log) Debug.Log("Failed to find a Slot!");
			return false;
		}

		if (m_Log) Debug.Log("Added an Item to the Inventory!");
		return SetItem(inventoryItemData);
	}

	public override void OnSceneChange(Scene scene, LoadSceneMode mode)
	{
		if (_GameManager.m_CurrentState == States.InMenus)
		{
			m_Inventory.gameObject.SetActive(false);
			return;
		}

		// Enable the inventory if it's disabled.
		if (!m_Inventory.gameObject.activeInHierarchy)
			m_Inventory.gameObject.SetActive(true);

		// Find the dropped object parent, if possible.
		GameObject obj = GameObject.Find("----Pickups----");
		if (obj == null)
			obj = new GameObject("----Pickups----");
		m_DropHeldItem.m_PickupParent = obj.transform;
	}

	private void GetInventory()
	{
		// Get the inventory slots.
		m_InventorySlots.Clear();
		var slots =
			FindObjectsByType<InventorySlot>(FindObjectsInactive.Include,
				FindObjectsSortMode.None);

		foreach (InventorySlot slot in slots)
			m_InventorySlots.Add(slot.transform);

		// Obtain the held item slot and remove it from the list.
		if (m_InventorySlots.Count == 0) return;

		Transform slotToRemove = null;
		foreach (Transform slot in m_InventorySlots)
		{
			if (!slot.TryGetComponent(out HeldItemSlot heldItemSlot))
				continue;
			m_HeldItemSlot = heldItemSlot;
			slotToRemove = slot;
			break;
		}

		if (slotToRemove != null) m_InventorySlots.Remove(slotToRemove);
	}

	private void OnGameReset()
	{
		Transform slotTransform;

		// Clear the inventory.
		foreach ((var key, InventoryItemData value) in m_InventoryItems)
		{
			value.m_IsCollected = false;
			value.m_Item = null;
			value.m_Slot.item = null;
			value.m_IsEquipped = false;

			// Remove all children from the slot.
			slotTransform = value.m_Slot.transform;
			for (var i = slotTransform.childCount - 1; i >= 0; i--)
				Destroy(slotTransform.GetChild(i).gameObject);
		}

		// Clear the held item slot.
		if (m_HeldItemSlot.playerHeldItem == null) return;
		m_HeldItemSlot.playerHeldItem.m_Item = null;
		m_HeldItemSlot.playerHeldItem.m_IsCollected = false;
		m_HeldItemSlot.playerHeldItem.m_IsEquipped = false;
		m_HeldItemSlot.playerHeldItem.m_Slot.item = null;

		// Remove all children from the slot.
		slotTransform = m_HeldItemSlot.transform;
		for (var i = slotTransform.childCount - 1; i >= 0; i--)
			Destroy(slotTransform.GetChild(i).gameObject);
	}

	private bool SetItem(InventoryItemData data)
	{
		if (m_Log) Debug.Log($"Slot {data.m_Slot.name} is empty");

		// Create inventory item instance.
		GameObject instance =
			Instantiate(_ItemPrefab, data.m_Slot.transform);

		if (!instance.TryGetComponent(out InventoryItem inventoryItem))
		{
			Debug.LogError("Failed to get InventoryItem component!");
			return false;
		}

		instance.name = data.m_Item.m_Name + "Inventory Item";
		instance.GetComponent<Image>().sprite = data.m_Item.m_Sprite;
		inventoryItem.itemData = data;
		data.m_Slot.item = inventoryItem;

		if (m_Log)
		{
			Debug.Log(
				$"Added <{data.m_Item.m_Type}> to slot {data.m_Slot.name} - " +
				$"Type Validation:<{inventoryItem.itemData.m_Item.m_Type}>");
		}

		if (m_InventoryItems.ContainsKey(data.m_Item.m_Name))
		{
			if (m_Log)
			{
				Debug.Log(
					$"Item '{data.m_Item.m_Name}' already exists in inventory.");
			}

			return true;
		}

		data.m_IsCollected = true;
		m_InventoryItems.Add(data.m_Item.m_Name, data);

		return true;
	}
}
}