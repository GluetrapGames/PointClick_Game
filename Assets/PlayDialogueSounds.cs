using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueSound : MonoBehaviour
{
    // Play sound when button is clicked
    public void onClick()
    {
        AkSoundEngine.PostEvent("ui_select", gameObject);
    }

    // Play sound when button is hovered
    public void onHover()
    {
        AkSoundEngine.PostEvent("ui_hover", gameObject);
    }

    // Play back sound for return button only

    public void backOnClick()
    {
        AkSoundEngine.PostEvent("ui_back", gameObject);
    }

    // Play sound when letter appears
    public void typewriteLetter()
    {
        AkSoundEngine.PostEvent("ui_letter", gameObject);
    }

    public void dialoguePopup()
    {
        AkSoundEngine.PostEvent("ui_dialoguePopup", gameObject);
    }
}
