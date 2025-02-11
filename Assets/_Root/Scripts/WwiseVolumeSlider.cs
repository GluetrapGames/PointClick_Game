using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WwiseVolumeSlider : MonoBehaviour
{
    public Slider thisSlider;
    public float masterVol;
    public float musicVol;
    public float sfxVol;
    public float dialogueVol;
    public float loudVol;

    public void setSliderVolume(string whatSlider)
    {
        float sliderValue = thisSlider.value;

        if (whatSlider == "Master")
        {
            masterVol = thisSlider.value;
            AkSoundEngine.SetRTPCValue("vol_master", masterVol);
        }
        if (whatSlider == "Music")
        {
            musicVol = thisSlider.value;
            AkSoundEngine.SetRTPCValue("vol_music", musicVol);
        }
        if (whatSlider == "SFX")
        {
            sfxVol = thisSlider.value;
            AkSoundEngine.SetRTPCValue("vol_sfx", sfxVol);

        }
        if (whatSlider == "Dialogue")
        {
            dialogueVol = thisSlider.value;
            AkSoundEngine.SetRTPCValue("vol_dialogue", dialogueVol);
        }
        if (whatSlider == "LoudSound")
        {
            loudVol = thisSlider.value;
            AkSoundEngine.SetRTPCValue("vol_loud", loudVol);
        }
    }
}
