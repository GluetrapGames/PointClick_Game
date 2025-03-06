using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GlueTrap
{
public class AutoplaySelection : MonoBehaviour
{
	public GameObject continueButton;
	public GameObject autoplayButton;

	public ConversationControl conversationControlRef;

	public void setContinueButtonState()
	{
		if (!conversationControlRef.autoplay)
			EventSystem.current.SetSelectedGameObject(continueButton);
		else
			EventSystem.current.SetSelectedGameObject(autoplayButton);
	}
}
}