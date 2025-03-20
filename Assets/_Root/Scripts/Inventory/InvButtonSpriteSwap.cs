using UnityEngine;
using UnityEngine.UI;

namespace GlueTrap
{
public class InvButtonSpriteSwap : MonoBehaviour
{
	[SerializeField]
	private GameObject _buttonToSwap;
	[SerializeField]
	private Sprite _OpenButtonText;
	[SerializeField]
	private Sprite _CloseButtonText;
	[SerializeField]
	private Image _ShowButtonImage;

	[SerializeField]
	private GameObject _invToSwap;

	[SerializeField]
	private GameObject inventoryTopBox;
	private bool _active;


	public void OnButtonClick()
	{
		if (_active)
		{
			_ShowButtonImage.sprite = _OpenButtonText;
			_active = false;
		}
		else
		{
			_ShowButtonImage.sprite = _CloseButtonText;
			_active = true;
		}
	}
}
}