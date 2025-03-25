using System.Collections;
using System.Collections.Generic;
using EditorAttributes.Editor;
using GlueTrap.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GlueTrap
{
    public class HouseMusic : Singleton<HouseMusic>
    {
        public bool Log;
        public string MusicState;
        private bool _Activated = false;
        private bool _JohnActivated;
        private bool doOnce;

        // On scene change
        public override void OnSceneChange(Scene scene, LoadSceneMode mode)
        {
            {   // if not on menu or court scenes and not already playing
                if ((scene.name != "MenuScene" && scene.name != "CourtScene 1" && scene.name != "CourtScene 2" && scene.name != "CourtScene 3" && scene.name != "CourtScene 4") && !_Activated)

                    if (scene.name != "Outside")
                    {   
                        if (!doOnce)
                        {
                            doOnce = true;
                            AkSoundEngine.SetState("HouseMusic", "low");
                        }

                        if (scene.name != "John's Flat") 
                        {
                            // post house music
                            _Activated = true;
                            AkSoundEngine.PostEvent("MusicHouse", gameObject);
                            if (Log) Debug.Log("Started music, " + _Activated);
                        }
                    }
                }
                // john flat music
                if (scene.name == "John's Flat" && !_JohnActivated)
                {
                    _JohnActivated = true;
                    AkSoundEngine.PostEvent("MusicFlat", gameObject);
                    if (Log) Debug.Log("Flat Music = " + _JohnActivated);
                }

                // stop john flat music
                if (scene.name != "John's Flat" && _JohnActivated)
                {
                    StopMusic();
                    _JohnActivated = false;
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
