using UnityEngine;

public class BreakableItem : MonoBehaviour
{
	public static InventoryItem PlayerHeldItem;

	public int itemHp;
	public CollideCheck itemCollision;
	//private string _heldItemType = PlayerHeldItem.itemType;
	public string effectiveItemType;

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.E) && itemCollision.IsCollided)
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