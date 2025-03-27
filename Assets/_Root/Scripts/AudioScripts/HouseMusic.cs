using AK.Wwise;
using GlueTrap.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GlueTrap
{
    public class HouseMusic : Singleton<HouseMusic>
    {
        public bool Log;
        [SerializeField]
        private int musicState;
        [SerializeField]
        private bool _Activated = false;
        [SerializeField]
        private bool _JohnActivated;
        private bool doOnce;
        [SerializeField]
        public string currentState;
        [SerializeField]
        private GameManager _gameManager;
        private bool _beenUpstairs;
        private bool isEvil;
        private void Update()
        {
            musicState = _gameManager.m_totalItemsDestroyed + _gameManager.m_totalItemsPickedUp;

            if (_gameManager.m_totalItemsDestroyed > 0)
            {
                isEvil = true;
            }

            if (musicState <= 0) return;

            // If the player has broken items
            if (isEvil)
            {
                switch (musicState) 
                {
                    case 1:
                        setLow();
                        break;
                    case 5:
                        setMid();
                        break;
                    case 12:
                        if (_beenUpstairs) { setHigh(); }
                        break;
                    case 20:
                        if (_beenUpstairs) { setHigher(); }
                        break;
                }
            }

            // If the player has broken no items
            if (!isEvil)
            {
                switch (musicState)
                {
                    case 1:
                        setLow();
                        break;
                    case 2:
                        setMid();
                        break;
                    case 3:
                        if (_beenUpstairs) { setHigh(); }
                        break;
                    case 4:
                        if (_beenUpstairs) { setHigher(); }
                        break;
                }
            }
        }

        // On scene change, multiple checks for what scene is loaded, post according music
        public override void OnSceneChange(Scene scene, LoadSceneMode mode)
        {

            {   // if not on menu or court scenes and not already playing
                if ((scene.name != "MenuScene" && scene.name != "CourtScene 1" && scene.name != "CourtScene 2" && scene.name != "CourtScene 3" && scene.name != "CourtScene 4") && !_Activated)
                {
                    if (scene.name != "Outside" && scene.name != "John's Flat")
                    {
                        //if (!doOnce) { AkSoundEngine.SetState("HouseMusic", "low"); doOnce = true; }
                        _Activated = true;
                        AkSoundEngine.PostEvent("MusicHouse", gameObject);
                        if (Log) Debug.Log("Started music, " + _Activated);
                    }
                }

                if ((scene.name == "CourtScene 1" || scene.name == "CourtScene 2" || scene.name == "CourtScene 3" || scene.name == "CourtScene 4") && _Activated)
                {
                    StopMusic();
                    _Activated = false;
                }

                if (scene.name == "Hallway1" && !_beenUpstairs)
                {
                    _beenUpstairs = true;
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
                AkSoundEngine.SetState("HouseMusic", "low");
                currentState = "low";
        }

        public void setMid()
        {
                AkSoundEngine.SetState("HouseMusic", "mid");
                currentState = "mid";
        }
        public void setHigh()
        {
                AkSoundEngine.SetState("HouseMusic", "high");
                currentState = "high";
        }
        public void setHigher()
        {
                AkSoundEngine.SetState("HouseMusic", "higher");
                currentState = "higher";
        }

    }
}
