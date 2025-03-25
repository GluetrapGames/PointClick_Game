using System.Collections;
using System.Collections.Generic;
using GlueTrap.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GlueTrap
{
    public class HouseMusic : Singleton<HouseMusic>
    {
        public bool Log;
        public string MusicState = "low";
        private bool _Activated;
        private bool _JohnActivated;


        // On scene change
        public override void OnSceneChange(Scene scene, LoadSceneMode mode)
        {
            {
                if (scene.name != "MenuScene" && scene.name != "Outside" && scene.name != "John's Flat" && scene.name != "CourtScene 1" && scene.name != "CourtScene 2" && scene.name != "CourtScene 3" && scene.name != "CourtScene 4" && !_Activated)
                {
                    _Activated = true;
                    AkSoundEngine.PostEvent("MusicHouse", gameObject);
                    Debug.Log("Started music, " + _Activated);
                    ;
                }

                if (scene.name == "John's Flat" && !_JohnActivated)
                {
                    _JohnActivated = true;
                    AkSoundEngine.PostEvent("MusicFlat", gameObject);
                    Debug.Log("Flat Music = " + _JohnActivated);
                }

                if (scene.name != "John's Flat" && _JohnActivated)
                {
                    StopMusic();
                }
            }
        }

        // Stop music
        private void StopMusic()
        {
            AkSoundEngine.PostEvent("StopMusic", gameObject);
        }

        // Music state transitions
        public void setLow()
        {
            if (_Activated)
            {
                AkSoundEngine.SetState("HouseMusic", "low");
            }
        }

        public void setMid()
        {
            if (_Activated)
            {
                AkSoundEngine.SetState("HouseMusic", "mid");
            }
        }
        public void setHigh()
        {
            if (_Activated)
            {
                AkSoundEngine.SetState("HouseMusic", "high");
            }
        }
        public void setHigher()
        {
            if (_Activated)
            {
                AkSoundEngine.SetState("HouseMusic", "higher");
            }
        }

    }
}
