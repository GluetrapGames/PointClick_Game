using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Switcher : MonoBehaviour
{
	public KeyCode m_SwitchKey = KeyCode.Alpha0;
	public List<GameObject> m_SwitchObjects = new();
	public List<TextMeshProUGUI> m_SwitchObjectUis = new();

	private void Update()
	{
		if (!Input.GetKeyDown(m_SwitchKey)) return;

		foreach (var t in m_SwitchObjects)
			t.gameObject.SetActive(!t.activeSelf);

		foreach (var element in m_SwitchObjectUis)
			element.gameObject.SetActive(!element.gameObject.activeSelf);
	}
}