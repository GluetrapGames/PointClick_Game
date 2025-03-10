using UnityEngine;
using System.Collections;

namespace PixelCrushers.DialogueSystem
{

    /// <summary>
    /// Add to a subtitle text GameObject to adjust the typewriter speed to match the current
    /// audio clip.
    /// </summary>
    public class SetTypewriterSpeedToAudioLength : MonoBehaviour
    {
        void OnTextChange(UITextField textField)
        {
            StartCoroutine(SetTypewriterSpeed(textField.text));
        }

        IEnumerator SetTypewriterSpeed(string text)
        {
            yield return null;
            yield return null;
            var typewriter = GetComponent<AbstractTypewriterEffect>();
            var audioSource = DialogueManager.currentConversationState.subtitle.speakerInfo.transform.GetComponent<AudioSource>();
            if (typewriter != null && audioSource != null && audioSource.clip != null)
            {
                typewriter.charactersPerSecond = text.Length / audioSource.clip.length;
                typewriter.StartTyping(text, 1);
            }
        }

    }
}