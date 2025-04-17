using AK.Wwise;
using PixelCrushers.DialogueSystem;
using PixelCrushers.DialogueSystem.ChatMapper;
using Unity.Sentis.Layers;
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
				AkSoundEngine.SetRTPCValue("vol_gameShow", 25f);
				break;
			case 31:
				AkSoundEngine.PostEvent("PhoneHang", gameObject);
				break;
			case 32:
				AkSoundEngine.PostEvent("PhoneSlam", gameObject);
				AkSoundEngine.PostEvent("StopPhone", gameObject);
				break;
			case 35:
				AkSoundEngine.SetRTPCValue("vol_gameshow", 50f);
				break;
        }

		
        }

		public void GameshowVolume()
		{
            DialogueEntry dialogueEntry = DialogueManager.currentConversationState
            .subtitle.dialogueEntry;
            var conversationID = dialogueEntry.conversationID;
            var subtitleId = dialogueEntry.id;

            if (conversationID != 58) return;

            if (conversationID == 58)
            {
                switch (subtitleId)
                {
                    case 3:
                        Debug.Log("gameshow louder, music down");

                        AkSoundEngine.SetRTPCValue("vol_gameshow", 100f);
                        AkSoundEngine.SetRTPCValue("vol_music", 2.5f);
                        break;
							
                    case 6:
                        Debug.Log("gameshow down, music louder");
                        AkSoundEngine.SetRTPCValue("vol_gameshow", 50f);
                        AkSoundEngine.SetRTPCValue("vol_music", 5f);
                        break;
                }
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

		public void PlayJournalSounds()
		{
            DialogueEntry dialogueEntry = DialogueManager.currentConversationState
            .subtitle.dialogueEntry;
            var conversationID = dialogueEntry.conversationID;
            var subtitleId = dialogueEntry.id;

			if (conversationID != 23) return;

			switch (subtitleId)
			{
				case 1:
					AkSoundEngine.PostEvent("JournalOpen", gameObject);
					break;
				case 2:
					AkSoundEngine.PostEvent("JournalFlip", gameObject);
					break;
                case 3:
                    AkSoundEngine.PostEvent("JournalFlip", gameObject);
                    break;
                case 4:
                    AkSoundEngine.PostEvent("JournalFlip", gameObject);
                    break;
                case 5:
                    AkSoundEngine.PostEvent("JournalFlip", gameObject);
                    break;
                case 6:
                    AkSoundEngine.PostEvent("JournalFlip", gameObject);
                    break;
                case 7:
                    AkSoundEngine.PostEvent("JournalFlip", gameObject);
                    break;
                case 8:
                    AkSoundEngine.PostEvent("JournalFlip", gameObject);
                    break;
				case 9:
                    AkSoundEngine.PostEvent("JournalFlip", gameObject);
                    break;
				case 10:
					AkSoundEngine.PostEvent("JournalClose", gameObject);
					break;
            }
        }
		public void PlayPoemSounds()
		{
            DialogueEntry dialogueEntry = DialogueManager.currentConversationState
            .subtitle.dialogueEntry;
            var conversationID = dialogueEntry.conversationID;
            var subtitleId = dialogueEntry.id;

			if (conversationID != 24) return;

			switch (subtitleId)
			{
				case 1:
					AkSoundEngine.PostEvent("JournalOpen", gameObject);
					break;
				case 2:
					AkSoundEngine.PostEvent("JournalFlip", gameObject);
					break;
                case 3:
                    AkSoundEngine.PostEvent("JournalFlip", gameObject);
                    break;
                case 4:
                    AkSoundEngine.PostEvent("JournalFlip", gameObject);
                    break;
                case 5:
                    AkSoundEngine.PostEvent("JournalFlip", gameObject);
                    break;
                case 6:
                    AkSoundEngine.PostEvent("JournalFlip", gameObject);
                    break;
                case 7:
                    AkSoundEngine.PostEvent("JournalFlip", gameObject);
                    break;
                case 8:
                    AkSoundEngine.PostEvent("JournalFlip", gameObject);
                    break;
				case 9:
					AkSoundEngine.PostEvent("JournalFlip", gameObject);
					break;
				case 10:
					AkSoundEngine.PostEvent("JournalFlip", gameObject);
					break;
                case 11:
                    AkSoundEngine.PostEvent("JournalFlip", gameObject);
                    break;
                case 12:
                    AkSoundEngine.PostEvent("JournalClose", gameObject);
                    break;
            }
        }
        public void PlayAlbertArriveSounds()
        {
            DialogueEntry dialogueEntry = DialogueManager.currentConversationState
            .subtitle.dialogueEntry;
            var conversationID = dialogueEntry.conversationID;
            var subtitleId = dialogueEntry.id;

            if (conversationID != 49) return;

            switch (subtitleId)
            {
                case 1:
                    AkSoundEngine.SetRTPCValue("vol_music", 1f);
                    break;
                case 35:
                    AkSoundEngine.PostEvent("AlbertIsHome", gameObject);
                    break;
                //case 38:
                //    AkSoundEngine.PostEvent("ExitWalk", gameObject);
                //    break;
                //case 39:
                //    AkSoundEngine.PostEvent("KeyJangle", gameObject);
                //    break;
                //case 40:
                //    AkSoundEngine.PostEvent("DoorOpen", gameObject);
                //    break;
                case 41:
                    AkSoundEngine.SetRTPCValue("vol_music", 5f);
                    break;
            }
        }

        public void PlayRecordSounds()
        {
            DialogueEntry dialogueEntry = DialogueManager.currentConversationState
            .subtitle.dialogueEntry;
            var conversationID = dialogueEntry.conversationID;
            var subtitleId = dialogueEntry.id;

            if (conversationID != 34) return;

            switch (subtitleId)
            {
                case 3:
                    AkSoundEngine.PostEvent("MusicRecord", gameObject);
                    AkSoundEngine.SetRTPCValue("vol_music", 0f);
                    break;
                case 5:
                    AkSoundEngine.PostEvent("StopRecord", gameObject);
                    AkSoundEngine.SetRTPCValue("vol_music", 5f);
                    break;
            }
        }
        }
}