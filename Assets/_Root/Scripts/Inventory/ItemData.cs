using GlueTrap.Scriptable_Objects;
using GlueTrap.Utilities;
using UnityEngine;

namespace GlueTrap
{
public class ItemData
{
	public CollectableItemData m_CollectableData;
	public PathData m_HighlightColliderPath;
	public string m_Name;
	public Sprite m_Sprite;
	public ItemTypes m_Type;

	public ItemData(string name, ItemTypes type, Sprite sprite,
		CollectableItemData collectableData = null,
		PathData highlightColliderPath = null)
	{
		m_Name = name;
		m_Type = type;
		m_Sprite = sprite;
		m_CollectableData = collectableData;
		m_HighlightColliderPath = highlightColliderPath;
	}
}

public class PathData
{
	public int m_Index;
	public int m_PointCount;
	public Vector2[] m_Points;

	public PathData(int index, Vector2[] points)
	{
		m_Index = index;
		m_Points = points;
		m_PointCount = points.Length;
	}
}
}