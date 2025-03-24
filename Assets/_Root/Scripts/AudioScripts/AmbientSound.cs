using System.Collections;
using System.Collections.Generic;
using GlueTrap.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace GlueTrap
{
    public class AmbientSound : Singleton<AmbientSound>
    {
        private bool _HouseActivated;
        private bool _CourtActivated;
        private bool _ClockActivated;

        private void HouseAmbience()
        {
            _HouseActivated = true;
            AkSoundEngine.SetRTPCValue("CourtReverb", 0, gameObject);
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
                _CourtActivated = false;
                HouseAmbience();
                Debug.Log("House Ambience = " + _HouseActivated);
            }

            // Same but for Court
            if ((scene.name == "CourtroomIntro" || scene.name == "CourtroomEnding") && !_CourtActivated)
            {
                CourtAmbience();
                Debug.Log("Court Ambience = " + _CourtActivated);
            }
                
            // Starts clock ambience for living room only
            if (scene.name == "LivingRoom")
            {
                if (!_ClockActivated)
                {
                    _ClockActivated = true;
                    AkSoundEngine.PostEvent("CourtClock", gameObject);
                    Debug.Log("Clock Ambience = " + _ClockActivated);
                }
            }

            if (scene.name != "LivingRoom")
            {
                if (_ClockActivated)
                {
                    _ClockActivated = false;
                    AkSoundEngine.PostEvent("StopClock", gameObject);
                }
            }


            // Stops all sound and plays menu music
            if (scene.name == "MenuScene")
            {
                AkSoundEngine.StopAll();
                AkSoundEngine.PostEvent("MusicMenu", gameObject);
            }
        }
    }
}
