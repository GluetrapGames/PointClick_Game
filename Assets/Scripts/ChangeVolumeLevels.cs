using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeVolumeLevels : MonoBehaviour
{
    public Slider thisSlider;
    public float masterVol;
    public float musicVol;
    public float sfxVol;

    public void SetSpecificVolume(string whatValue)
    {
        float sliderValue = thisSlider.value;

        if (whatValue == "Master")
        {
            masterVol = thisSlider.value;
            AkSoundEngine.SetRTPCValue("vol_master", masterVol);
        }

        if (whatValue == "Music")
        {
            musicVol = thisSlider.value;
            AkSoundEngine.SetRTPCValue("vol_music", musicVol);
        }
    }
}
