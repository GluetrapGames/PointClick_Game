using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSound : MonoBehaviour
{
    // Play sound when button is clicked
    public void onClick()
    {
        AkSoundEngine.PostEvent("menu_select", gameObject);
    }

    // Play sound when button is hovered
    public void onHover()
    {
        AkSoundEngine.PostEvent("menu_hover", gameObject);
    }

    // Play back sound for return button only

    public void backOnClick()
    {
        AkSoundEngine.PostEvent("menu_back", gameObject);
    }

    // Play sound when logo is clicked
    public void logoOnClick()
    {
        AkSoundEngine.PostEvent("Albert_Blab", gameObject);
    }

    public void otherLogoOnClick()
    {
        AkSoundEngine.PostEvent("Lawyer_Blab", gameObject);
    }
}
