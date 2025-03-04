using UnityEngine;

namespace GlueTrap
{
public class InventoryRenderingHierarchy : MonoBehaviour
{
	[SerializeField]
	private GameObject darkBG;

	private void Start()
	{
		Transform darkBackgroundTransform = darkBG.transform;
		Transform
			heldItemTbTransform = transform; // This script is on HeldItemTB

		// Ensure HeldItemTB is drawn above DarkBackground but below Inventory
		var targetIndex = darkBackgroundTransform.GetSiblingIndex() + 1;
		heldItemTbTransform.SetSiblingIndex(targetIndex);
	}
}
}