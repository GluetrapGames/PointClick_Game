using EditorAttributes;
using GlueTrap.Utilities;
using UnityEngine;

namespace GlueTrap.Scriptable_Objects
{
[CreateAssetMenu(fileName = "Collectable Item", menuName = "GlueTrap/Items",
	order = 0)]
public class CollectableItemData : ScriptableObject
{
	[AssetPreview]
	public Sprite m_Sprite;
	public Vector2 m_Size = new(1f, 1f);
	public PickUpScript.InteractionDir m_InteractionDirection =
		PickUpScript.InteractionDir.Left;
	[Range(0, 5)]
	public int m_PickUpDistance = 1;
	[Range(1f, 3f)]
	public float m_ControllerInteractionDistance = 1f;
	public string m_PickupEvent = "player_pickup";
	public bool m_IsWallItem;
	public ItemTypes m_ItemType;
}
}