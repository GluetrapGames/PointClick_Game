using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GlueTrap
{
    public class HouseMusic : MonoBehaviour
    {
        private bool hm_IsPlaying = false;
        public string hm_MusicState;

        private void Awake()
        {
            Debug.Log("Awake: " + SceneManager.GetActiveScene().name);

            if (SceneManager.GetActiveScene().name != "MenuScene")
            {
                if (!hm_IsPlaying)
                {
                    hm_IsPlaying = true;
                    AkSoundEngine.SetState("HouseMusic", "low");
                    AkSoundEngine.PostEvent("MusicHouse", gameObject);
                }
            }
        }

        public void setLow()
        {
            if (hm_IsPlaying)
            {
                AkSoundEngine.SetState("HouseMusic", "low");
            }
        }

        public void setMid()
        {
            if (hm_IsPlaying)
            {
                AkSoundEngine.SetState("HouseMusic", "mid");
            }
        }
        public void setHigh()
        {
            if (hm_IsPlaying)
            {
                AkSoundEngine.SetState("HouseMusic", "high");
            }
        }
        public void setHigher()
        {
            if (hm_IsPlaying)
            {
                AkSoundEngine.SetState("HouseMusic", "higher");
            }
        }


    }
}
