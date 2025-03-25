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
        private bool _ClockActivated = false;
        public bool Log;

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

        // NOTE FOR PHONE CALL SCRIPT----------------------------------------------------
        //void OnConversationLine(Subtitle subtitle)
        //{
        //    if (subtitle.dialogueEntry.conversationID == numba)
        //    {

        //    }
        //}


        public override void OnSceneChange(Scene scene, LoadSceneMode mode)
        {
            // On scene change, If not on menu scene and is not currently playing,
            // set playing to true and post.
            if ((scene.name != "MenuScene" && scene.name != "CourtScene 1" && scene.name != "CourtScene 2" && scene.name != "CourtScene 3" && scene.name != "CourtScene 4") && !_HouseActivated)
            {
                StopCourt();
                if (scene.name != "Outside")
                {
                    HouseAmbience();
                    Debug.Log("House Ambience = " + _HouseActivated);
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

            // Starts clock ambiance for living room only
            if (scene.name == "LivingRoom")
            {
                if (!_ClockActivated)
                {
                    _ClockActivated = true;
                    AkSoundEngine.PostEvent("CourtClock", gameObject);
                    if (Log)
                        Debug.Log("Clock Ambience = " + _ClockActivated);
                }
            }
            // Stops clock ambiance once living room is left
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

    }
}
