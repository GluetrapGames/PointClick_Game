using PixelCrushers.DialogueSystem;
using UnityEngine.UI;

namespace GlueTrap
{
public class ButtonSpeedUp : StandardUIContinueButtonFastForward
{
	public Text buttonText;
	public int defaultTypeSpeed = 25;

	public int fasterSpeed = 100;
	private int multiplier = 1;

	public void IncreaseMultiplier()
	{
		multiplier++;

		if (multiplier > 3) multiplier = 1;

		fasterSpeed = defaultTypeSpeed * multiplier;

		buttonText.text = multiplier + "x";

		if (typewriterEffect != null && typewriterEffect.isPlaying)
		{
			// Restart typing with the new speed immediately
			var completeText = DialogueManager.currentConversationState.subtitle
				.formattedText.text;
			var textUI = typewriterEffect.GetComponent<Text>();
			var textSoFar = Tools.StripRichTextCodes(
				textUI.text.Substring(0,
					textUI.text.IndexOf("<color=#00000000>")));
			var charsSoFar = textSoFar.Length;

			typewriterEffect.charactersPerSecond = fasterSpeed;
			typewriterEffect.StartTyping(completeText, charsSoFar);
		}
	}

	public override void OnFastForward()
	{
		if (typewriterEffect != null && typewriterEffect.isPlaying)
		{
			var completeText = DialogueManager.currentConversationState.subtitle
				.formattedText.text;
			var textUI = typewriterEffect.GetComponent<Text>();
			var textSoFar = Tools.StripRichTextCodes(
				textUI.text.Substring(0,
					textUI.text.IndexOf("<color=#00000000>")));
			var charsSoFar = textSoFar.Length;
			typewriterEffect.charactersPerSecond = fasterSpeed;
			typewriterEffect.StartTyping(completeText, charsSoFar);
		}
		else
			base.OnFastForward();
	}
}
}