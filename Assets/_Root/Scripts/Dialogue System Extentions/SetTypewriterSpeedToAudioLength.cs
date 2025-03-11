using UnityEngine;
using System.Collections;

namespace PixelCrushers.DialogueSystem
{
    public class SetTypewriterSpeedToAudioLength : MonoBehaviour
    {

        public AudioSource audioSource;

        void OnTextChange(UITextField textField)
        {
            StartCoroutine(SetTypewriterSpeed(textField.text));
        }

        IEnumerator SetTypewriterSpeed(string text)
        {
            yield return null;
            yield return null;
            var typewriter = GetComponent<TextMeshProTypewriterEffect>();
            //var audioSource = DialogueManager.currentConversationState.subtitle.speakerInfo.transform.GetComponent<AudioSource>();
            if (typewriter != null && audioSource != null && audioSource.clip != null)
            {
                typewriter.charactersPerSecond = text.Length / audioSource.clip.length;
                typewriter.StartTyping(text, 1);
            }
        }

    }
}