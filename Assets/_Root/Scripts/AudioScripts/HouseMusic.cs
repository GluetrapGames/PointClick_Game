using AK.Wwise;
using GlueTrap.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GlueTrap
{
    public class HouseMusic : Singleton<HouseMusic>
    {
        public bool Log;
        private int MusicState;
        private bool _Activated = false;
        private bool _JohnActivated;
        private bool inHouse;
        public string currentState = "null";

        // On scene change
        public override void OnSceneChange(Scene scene, LoadSceneMode mode)
        {
            {   // if not on menu or court scenes and not already playing
                if ((scene.name != "MenuScene" && scene.name != "CourtScene 1" && scene.name != "CourtScene 2" && scene.name != "CourtScene 3"
                    && scene.name != "CourtScene 4") && !_Activated)

                    if (scene.name != "Outside")
                    {   

                        if (scene.name != "John's Flat") 
                        {
                            // post house music
                            inHouse = true;
                            _Activated = true;
                            AkSoundEngine.PostEvent("MusicHouse", gameObject);
                            if (Log) Debug.Log("Started music, " + _Activated);
                        }
                    }
                }
            //john flat music
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

            if ((scene.name == "CourtScene 1" || scene.name == "CourtScene 2" || scene.name == "CourtScene 3" || scene.name == "CourtScene 4") && _Activated)
            {
                StopMusic();
                _Activated = false;
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
                currentState = "low";
            }
        }

        public void setMid()
        {
            if (_Activated)
            {
                AkSoundEngine.SetState("HouseMusic", "mid");
                currentState = "mid";
            }
        }
        public void setHigh()
        {
            if (_Activated)
            {
                AkSoundEngine.SetState("HouseMusic", "high");
                currentState = "high";
            }
        }
        public void setHigher()
        {
            if (_Activated)
            {
                AkSoundEngine.SetState("HouseMusic", "higher");
                currentState = "higher";
            }
        }

    }
}
