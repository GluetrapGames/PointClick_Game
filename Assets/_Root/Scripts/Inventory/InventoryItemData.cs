namespace GlueTrap
{
public class InventoryItemData
{
	public bool m_IsCollected;
	public bool m_IsEquipped;
	public ItemData m_Item;
	public InventorySlot m_Slot;

	public InventoryItemData(ItemData item, bool isCollected, bool isEquipped,
		InventorySlot slot)
	{
		m_Item = item;
		m_IsCollected = isCollected;
		m_IsEquipped = isEquipped;
		m_Slot = slot;
	}
}
}