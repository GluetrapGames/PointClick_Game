using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InventoryManager : Singleton<InventoryManager>
{
	public Transform m_Inventory;
	public List<Transform> m_InventorySlots = new();
	public HeldItemSlot m_HeldItemSlot;
	public bool m_Log;

	[SerializeField]
	private GameObject _ItemPrefab;
	[SerializeField]
	private GameObject _InventoryPrefab;

	private GameManager _GameManager;
	public Dictionary<string, InventoryItemData> m_InventoryItems = new();

	protected override void Awake()
	{
		base.Awake();
		_GameManager = FindFirstObjectByType<GameManager>();
		// Get the Inventory.
		GameObject inventoryObject = GameObject.FindWithTag("Inventory");
		if (!inventoryObject)
		{
			Debug.LogWarning("Cannot find 'Inventory Canvas' in the scene.");
			inventoryObject = Instantiate(_InventoryPrefab, transform);
		}

		m_Inventory = inventoryObject.transform;

		GetInventory();
	}

	public override void OnSceneChange(Scene scene, LoadSceneMode mode)
	{
		m_Inventory.gameObject.SetActive(_GameManager.m_CurrentState !=
		                                 States.InMenus);
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
			if (!slot.TryGetComponent(out HeldItemSlot heldItemSlot)) continue;
			m_HeldItemSlot = heldItemSlot;
			slotToRemove = slot;
			break;
		}

		if (slotToRemove != null) m_InventorySlots.Remove(slotToRemove);
	}

	public bool CollectItem(ItemData itemData)
	{
		var inventoryItemData = new InventoryItemData
		{
			m_Item = itemData,
			m_IsCollected = false,
			m_IsEquipped = false,
			m_Slot = null
		};

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

	private bool SetItem(InventoryItemData data)
	{
		if (m_Log) Debug.Log($"Slot {data.m_Slot.name} is empty");

		// Create inventory item instance.
		GameObject itemInstance =
			Instantiate(_ItemPrefab, data.m_Slot.transform);

		if (!itemInstance.TryGetComponent(out InventoryItem item))
		{
			Debug.LogError("Failed to get InventoryItem component!");
			return false;
		}

		itemInstance.GetComponent<Image>().sprite = data.m_Item.m_Sprite;
		item.itemType = data.m_Item.m_Type;
		data.m_Slot.item = item;

		if (m_Log)
		{
			Debug.Log(
				$"Added <{data.m_Item.m_Type}> to slot {data.m_Slot.name} - " +
				$"Type Validation:<{item.itemType}>");
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

public struct ItemData
{
	public string m_Name;
	public Sprite m_Sprite;
	public string m_Type;

	public ItemData(string name, string type, Sprite sprite)
	{
		m_Name = name;
		m_Type = type;
		m_Sprite = sprite;
	}
}

public struct InventoryItemData
{
	public ItemData m_Item;
	public bool m_IsCollected;
	public bool m_IsEquipped;
	public InventorySlot m_Slot;
}