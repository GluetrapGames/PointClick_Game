using System.Collections;
using System.Collections.Generic;
using GlueTrap.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GlueTrap
{
    public class AmbientSound : Singleton<AmbientSound>
    {
        private bool _HouseActivated;
        private bool _CourtActivated;

        private void HouseAmbience()
        {
            _HouseActivated = true;
            AkSoundEngine.PostEvent("HouseTone", gameObject);
            AkSoundEngine.PostEvent("HouseBuzz", gameObject);
            AkSoundEngine.PostEvent("HouseCreak", gameObject);
        }  
        private void CourtAmbience()
        {
            _CourtActivated = true;
            AkSoundEngine.PostEvent("CourtHum", gameObject);
            AkSoundEngine.PostEvent("CourtBuzz", gameObject);
            AkSoundEngine.PostEvent("CourtCough", gameObject);
            AkSoundEngine.PostEvent("CourtClock", gameObject);
        }

        public override void OnSceneChange(Scene scene, LoadSceneMode mode)
        {
            // On scene change, If not on menu scene and is not currently playing,
            // set playing to true and post.
            if (scene.name != "MenuScene" && scene.name != "CourtroomIntro" && scene.name != "CourtroomEnding" && !_HouseActivated)
            {
                AkSoundEngine.StopAll();
                HouseAmbience();
                Debug.Log("House Ambience = " + _HouseActivated);
            }

            // Same but for Court
            if ((scene.name == "CourtroomIntro" || scene.name == "CourtroomEnding") && !_CourtActivated)
            {
                CourtAmbience();
                Debug.Log("Court Ambience = " + _CourtActivated);
            }
        }
    }
}
