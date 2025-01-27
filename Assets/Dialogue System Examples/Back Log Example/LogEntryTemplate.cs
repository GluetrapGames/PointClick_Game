using UnityEngine;
using UnityEngine.UI;
using PixelCrushers.DialogueSystem;

/// <summary>
/// This class is the UI template for a single subtitle in the back log.
/// </summary>
public class LogEntryTemplate : MonoBehaviour
{
    public Text speakerName;
    public Text dialogueText;
    public Button hornButton;

    public void Assign(Subtitle subtitle)
    {
        speakerName.text = subtitle.speakerInfo.Name;
        dialogueText.text = subtitle.formattedText.text;
        hornButton.onClick.AddListener(() =>
        {
            DialogueManager.instance.PlaySequence($"Audio(entrytag)", 
                subtitle.speakerInfo.transform, subtitle.listenerInfo.transform, 
                false, true, subtitle.entrytag);
        });
    }
}
