using UnityEngine.UI;

namespace PixelCrushers.DialogueSystem
{
    public class ButtonSpeedUp : StandardUIContinueButtonFastForward
    {

        public int fasterSpeed = 100;
        public int defaultTypeSpeed = 25;
        private int multiplier = 1;
        public Text buttonText;

        public void IncreaseMultiplier()
        {   
            multiplier++;

            if (multiplier > 3) multiplier = 1;

            fasterSpeed = defaultTypeSpeed * multiplier;

            buttonText.text = multiplier.ToString() + "x";
            
            if (typewriterEffect != null && typewriterEffect.isPlaying)
            {
                // Restart typing with the new speed immediately
                var completeText = DialogueManager.currentConversationState.subtitle.formattedText.text;
                var textUI = typewriterEffect.GetComponent<UnityEngine.UI.Text>();
                var textSoFar = Tools.StripRichTextCodes(textUI.text.Substring(0, textUI.text.IndexOf("<color=#00000000>")));
                var charsSoFar = textSoFar.Length;

                typewriterEffect.charactersPerSecond = fasterSpeed;
                typewriterEffect.StartTyping(completeText, charsSoFar);
            }
        }

        public override void OnFastForward()
        {
            if ((typewriterEffect != null) && typewriterEffect.isPlaying)
            {
                var completeText = DialogueManager.currentConversationState.subtitle.formattedText.text;
                var textUI = typewriterEffect.GetComponent<UnityEngine.UI.Text>();
                var textSoFar = Tools.StripRichTextCodes(textUI.text.Substring(0, textUI.text.IndexOf("<color=#00000000>")));
                var charsSoFar = textSoFar.Length;
                typewriterEffect.charactersPerSecond = fasterSpeed;
                typewriterEffect.StartTyping(completeText, charsSoFar);
            }
            else
            {
                base.OnFastForward();
            }
        }
    }
}