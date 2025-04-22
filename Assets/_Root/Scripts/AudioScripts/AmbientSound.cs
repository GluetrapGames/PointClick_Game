using System.Collections;
using System.Collections.Generic;
using GlueTrap.Utilities;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace GlueTrap
{
    public class AmbientSound : Singleton<AmbientSound>
    {
        private bool _HouseActivated = false;
        private bool _CourtActivated = false;
        public bool inLivingRoom;
        private bool _ClockActivated = false;
        private bool _OutsideActivated = false;
        public bool Log;
        public ClockSounds clockSounds;

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

        private void OutsideAmbiance()
        {
            _OutsideActivated = true;
            AkSoundEngine.PostEvent("OutsideAmbiance", gameObject);
        }

        public override void OnSceneChange(Scene scene, LoadSceneMode mode)
        {
            // On scene change, If not on menu scene and is not currently playing,
            // set playing to true and post.
            if ((scene.name != "MenuScene" && scene.name != "CourtScene 1" && scene.name != "CourtScene 2" && scene.name != "CourtScene 3" && scene.name != "CourtScene 4") && !_HouseActivated)
            {
                OutsideAmbiance();
                StopCourt();
                if (scene.name != "Outside")
                {
                    StopOutside();
                    HouseAmbience();
                    Debug.Log("House Ambience = " + _HouseActivated);
                }

                if (scene.name == "John's Flat")
                {
                    AkSoundEngine.PostEvent("GameShow", gameObject);
                }

            }

            // Same but for Court
            if ((scene.name == "CourtScene 1" || scene.name == "CourtScene 2" || scene.name == "CourtScene 3" || scene.name == "CourtScene 4") && !_CourtActivated)
            {
                StopHouse();
                CourtAmbience();
                if (Log)
                    Debug.Log("Court Ambience = " + _CourtActivated);
            }

            // bool for 'ClockSounds' script
            if (scene.name == "LivingRoom")
            {
                inLivingRoom = true;
            }
            if (scene.name != "LivingRoom")
            {
                inLivingRoom = false;
            }

            // Stops all sound and plays menu music
            if (scene.name == "MenuScene")
            {
                AkSoundEngine.StopAll();
                AkSoundEngine.PostEvent("MusicMenu", gameObject);
            }

        }
        private void StopCourt()
        {
            _CourtActivated = false;
            AkSoundEngine.PostEvent("StopCourtAmb", gameObject);
        }

        private void StopHouse()
        {
            _HouseActivated = false;
            AkSoundEngine.PostEvent("StopHouseAmb", gameObject);
        }

        private void StopOutside()
        {
            _OutsideActivated = false;
            AkSoundEngine.PostEvent("StopOutsideAmb", gameObject);
        }
    }
}
