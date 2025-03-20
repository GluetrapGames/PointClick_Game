using System.Linq;
using GlueTrap.Utilities;
using UnityEngine;

namespace GlueTrap
{
public class DropHeldItem : MonoBehaviour
{
	// Parent to assign new pickup to.
	public Transform m_PickupParent;

	[SerializeField]
	private InventoryItem _heldItem;
	// Prefabs
	[SerializeField]
	private GameObject _pickupPrefab;
	[SerializeField]
	private GameObject _itemPrefab;
	[SerializeField]
	private bool _Log;

	private GameManager _GameManager;
	private InventoryItemData _heldItemData;


	private void Awake()
	{
		_GameManager = Utils.GetGameManager();
	}

	private bool GetHeldItem()
	{
		// Getting data from held item
		_heldItem = _GameManager.m_InventoryManager.m_HeldItemSlot
			.GetComponentInChildren<InventoryItem>();

		if (_heldItem == null)
		{
			Debug.LogWarning("No Held Item Found!");
			return false;
		}

		if (_heldItem.itemData == null)
			Debug.LogError("Failed to find held item data!");

		_heldItemData = _heldItem.itemData;
		return true;
	}

	public void DropItem()
	{
		// Return if retrieval of held item failed.
		if (!GetHeldItem()) return;

		// Find the collected item, and mark it as uncollected.
		var keys =
			_GameManager.m_InventoryManager.m_InventoryItems.Keys.ToList();
		foreach (var key in keys.Where(key =>
			         _GameManager.m_InventoryManager.m_InventoryItems
				         .ContainsKey(_heldItemData.m_Item.m_Name)))
		{
			_heldItemData.m_IsCollected = false;
			InventoryItemData newData = _heldItemData;
			_GameManager.m_InventoryManager.m_InventoryItems[key] = newData;
		}

		// Creating new pickup
		GameObject pickupInstance =
			Instantiate(_pickupPrefab, m_PickupParent.transform);
		var component = pickupInstance.GetComponent<PickUpScript>();
		pickupInstance.name = _heldItemData.m_Item.m_Name;
		component._ItemType = _heldItemData.m_Item.m_Type;
		component.sprite = _heldItemData.m_Item.m_Sprite;
		component.m_IsClicked = false;
		component.m_IsDropped = true;
		component._ItemPrefab = _itemPrefab;
		component.pickupEvent = "player_pickup";
		pickupInstance.GetComponent<SpriteRenderer>().sprite = component.sprite;
		pickupInstance.transform.position =
			_GameManager.m_Player.transform.position;
		pickupInstance.transform.localScale = new Vector3(0.35f, 0.35f, 1f);
		pickupInstance.GetComponent<BoxCollider2D>().size = new Vector2(3.5f, 3.5f);
		Destroy(_heldItem.gameObject);
		pickupInstance.SetActive(true);
	}
}
}