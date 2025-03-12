using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GlueTrap
{
    public class HouseMusic : MonoBehaviour
    {
        private bool hm_IsPlaying = false;
        public string hm_MusicState;

        private void Awake()
        {
            if (!hm_IsPlaying) 
            { 
                hm_IsPlaying = true;
                AkSoundEngine.SetState("HouseMusic", "low");
                AkSoundEngine.PostEvent("MusicHouse", gameObject); 
            }
        }


    }
}
