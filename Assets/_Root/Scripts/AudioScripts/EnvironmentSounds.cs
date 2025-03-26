using AK.Wwise;
using PixelCrushers.DialogueSystem;
using PixelCrushers.DialogueSystem.ChatMapper;
using UnityEngine;

namespace GlueTrap
{
public class EnvironmentSounds : MonoBehaviour
{
	private bool _PhoneRinging;

	public void PlayRingSounds()
	{
		DialogueEntry dialogueEntry = DialogueManager.currentConversationState
			.subtitle.dialogueEntry;
		var conversationID = dialogueEntry.conversationID;
			//Debug.Log(conversationID);

			if (conversationID == 56 && !_PhoneRinging)
			{
				_PhoneRinging = true;
				AkSoundEngine.PostEvent("PhoneRing", gameObject);
			}

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
		public void PlayOvenSounds()
		{
			DialogueEntry dialogueEntry = DialogueManager.currentConversationState
			.subtitle.dialogueEntry;
			var conversationID = dialogueEntry.conversationID;
			if (conversationID != 39) return;
			var subtitleId = dialogueEntry.id;
			switch (subtitleId)
			{
				case 1:
					AkSoundEngine.PostEvent("OvenOpen", gameObject);
					break;
			}
		}

		public void PlayFridgeSounds()
		{
            DialogueEntry dialogueEntry = DialogueManager.currentConversationState
            .subtitle.dialogueEntry;
            var conversationID = dialogueEntry.conversationID;
            var subtitleId = dialogueEntry.id;
			if (conversationID != 37) return;

			switch (subtitleId)
			{
				case 1:
					AkSoundEngine.PostEvent("FridgeOpen", gameObject);
					break;
			}

        }
        }
}