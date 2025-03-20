using GlueTrap.Utilities;
using UnityEngine;

namespace GlueTrap
{
public class ItemData
{
	public string m_Name;
	public Sprite m_Sprite;
	public ItemTypes m_Type;

	public ItemData(string name, ItemTypes type, Sprite sprite)
	{
		m_Name = name;
		m_Type = type;
		m_Sprite = sprite;
	}
}
}