using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using Unity.Sentis.Layers;
using UnityEngine;

namespace GlueTrap
{ 
    public class AutoplayCustom : MonoBehaviour
    {
        protected AbstractDialogueUI dialogueUI;
        public bool _Autoplay = false;
        public float _TimerTarget = 2.0f;

        public virtual void ToggleAutoPlay()
        {
            _Autoplay = !_Autoplay;
            var mode = DialogueManager.displaySettings.subtitleSettings.continueButton;
            var newMode = (mode == DisplaySettings.SubtitleSettings.ContinueButtonMode.Never) ? DisplaySettings.SubtitleSettings.ContinueButtonMode.Always : DisplaySettings.SubtitleSettings.ContinueButtonMode.Never;
            DialogueManager.displaySettings.subtitleSettings.continueButton = newMode;
            //if (newMode == DisplaySettings.SubtitleSettings.ContinueButtonMode.Never) 
            //    Invoke("CallOnConversationContinue", 1f);
        }

        public void OnTypewriterFinished() 
        {
            if (_Autoplay)
            {
                StartCoroutine(Timer(OnCoroutineComplete));
            }
        }


        private IEnumerator Timer(System.Action callback) 
        {
            yield return new WaitForSeconds(_TimerTarget);
            Debug.Log("COROUTINE WAITFOR FINISHED");
            callback?.Invoke();
        }

        private void OnCoroutineComplete() 
        {
            dialogueUI.OnContinueConversation();
            Debug.Log("AFTER COROUTINE COMPLETE");
        }


    }
}
