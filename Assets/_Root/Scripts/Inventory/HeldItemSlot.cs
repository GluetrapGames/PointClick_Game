using GlueTrap.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GlueTrap
{
public class HeldItemSlot : MonoBehaviour, IDropHandler
{
	public ItemTypes playerHeldItem;

	private void Update()
	{
		if (transform.childCount == 0) playerHeldItem = ItemTypes.None;
	}

	public void OnDrop(PointerEventData eventData)
	{
		if (transform.childCount == 0)
		{
			GameObject dropped = eventData.pointerDrag.gameObject;
			var item = dropped.GetComponent<InventoryItem>();
			playerHeldItem = item.itemType;
			item.parentAfterDrag = transform;
			Debug.Log(playerHeldItem);
		}
		else
		{
			var currentItem =
				transform.GetChild(0).GetComponent<InventoryItem>();
			GameObject dropped = eventData.pointerDrag.gameObject;
			var item = dropped.GetComponent<InventoryItem>();
			playerHeldItem = item.itemType;
			item.parentAfterDrag = transform;
			currentItem.parentAfterDrag = item.parentBeforeDrag;
			currentItem.transform.SetParent(currentItem.parentAfterDrag);
			Debug.Log(currentItem.parentAfterDrag);
			Debug.Log(playerHeldItem);
		}
	}
}
}