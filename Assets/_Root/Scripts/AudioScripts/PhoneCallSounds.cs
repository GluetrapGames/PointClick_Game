using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;

namespace GlueTrap
{
    public class PhoneCallSounds : MonoBehaviour
    {

        private bool _PhoneRinging = false;
        void Awake()
        {
            if (!_PhoneRinging)
            {
                _PhoneRinging = true;
                AkSoundEngine.PostEvent("PhoneRing", gameObject);
            }
           
        }

        public void ahhhh()
        {
            var dialogueentry = DialogueManager.currentConversationState.subtitle.dialogueEntry;
            int conversationID = dialogueentry.conversationID;
            int subtitleId = dialogueentry.id;
            if (conversationID == 59 && subtitleId == 1)
            {
                AkSoundEngine.PostEvent("StopPhone", gameObject);
                AkSoundEngine.PostEvent("PhonePickup", gameObject);
                
            }

            if (conversationID == 59 && subtitleId == 31)
            {
                AkSoundEngine.PostEvent("PhoneHang", gameObject);
            }

            if (conversationID == 59 && subtitleId == 32)
            {
                AkSoundEngine.PostEvent("PhoneSlam", gameObject);
                AkSoundEngine.PostEvent("StopPhone", gameObject);
            }
        }
    }
}
