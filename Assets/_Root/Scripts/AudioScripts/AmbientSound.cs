using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GlueTrap
{
    public class AmbientSound : MonoBehaviour
    {
        private bool amb_isPlaying;
        private void Awake()
        {
            // If not on menu scene and is not currently playing
            // set playing to true and post the 3 ambient sounds simultaniously
            if (SceneManager.GetActiveScene().name != "MenuScene")
            {
                if (!amb_isPlaying)
                {
                    amb_isPlaying = true;
                    AkSoundEngine.PostEvent("HouseTone", gameObject);
                    AkSoundEngine.PostEvent("HouseBuzz", gameObject);
                    AkSoundEngine.PostEvent("HouseCreak", gameObject);
                }
            }
        }
    }
}
