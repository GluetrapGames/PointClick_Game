using UnityEngine;

namespace GlueTrap
{
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
		AkSoundEngine.SetSwitch("CharacterBlab", "Albert", gameObject);
		AkSoundEngine.PostEvent("Blab", gameObject);
	}

	public void otherLogoOnClick()
	{
            AkSoundEngine.SetSwitch("CharacterBlab", "Jack", gameObject);
            AkSoundEngine.PostEvent("Blab", gameObject);
	}
}
}