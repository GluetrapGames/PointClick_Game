using PixelCrushers.DialogueSystem;
using UnityEngine;

namespace GlueTrap
{
public class PhoneCallSounds : MonoBehaviour
{
	[SerializeField]
	private bool _Log;

	public void PlayRingSounds()
	{
		DialogueEntry dialogueEntry = DialogueManager.currentConversationState
			.subtitle.dialogueEntry;
		var conversationID = dialogueEntry.conversationID;
		if (_Log) Debug.Log(conversationID);

		// Based on conversation played, do something.
		if (conversationID == 56)
			AkSoundEngine.PostEvent("PhoneRing", gameObject);
		if (conversationID != 59) return;

		var subtitleId = dialogueEntry.id;
		switch (subtitleId)
		{
			case 1:
				AkSoundEngine.PostEvent("StopPhone", gameObject);
				AkSoundEngine.PostEvent("PhonePickup", gameObject);
				break;
			case 31:
				AkSoundEngine.PostEvent("PhoneHang", gameObject);
				break;
			case 32:
				AkSoundEngine.PostEvent("PhoneSlam", gameObject);
				AkSoundEngine.PostEvent("StopPhone", gameObject);
				break;
		}
	}
}
}