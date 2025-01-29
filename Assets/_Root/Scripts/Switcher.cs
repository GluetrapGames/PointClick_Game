using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Switcher : MonoBehaviour
{
	public KeyCode m_SwitchKey = KeyCode.Alpha0;
	public bool m_SwitchComponent;
	public List<GameObject> m_SwitchObjects = new();
	public List<TextMeshProUGUI> m_SwitchObjectUis = new();

	private void Update()
	{
		if (!Input.GetKeyDown(m_SwitchKey)) return;

		foreach (var t in m_SwitchObjects)
			if (m_SwitchComponent)
				t.GetComponent<ShadowCaster2D>().enabled =
					!t.GetComponent<ShadowCaster2D>().enabled;
			else
				t.gameObject.SetActive(!t.activeSelf);

		foreach (var element in m_SwitchObjectUis)
			element.gameObject.SetActive(!element.gameObject.activeSelf);
	}
}