using TMPro;
using UnityEngine;

namespace GlueTrap
{
public class InvButtonSpriteSwap : MonoBehaviour
{
	[SerializeField]
	private GameObject _buttonBuckle;
	[SerializeField]
	private GameObject _buttonToSwap;
	[SerializeField]
	private GameObject _buttonText;

	[SerializeField]
	private GameObject _invToSwap;

	[SerializeField]
	private GameObject inventoryTopBox;

	private bool _active;

	public void OnButtonClick()
	{
		if (_active)
		{
			_buttonBuckle.SetActive(true);
			_buttonText.GetComponent<TextMeshProUGUI>().text = "Expand";
			_active = false;
		}
		else
		{
			_buttonBuckle.SetActive(false);
			_buttonText.GetComponent<TextMeshProUGUI>().text = "Close";
			_active = true;
		}
	}
}
}