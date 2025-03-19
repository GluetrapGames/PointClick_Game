using System.Collections;
using System.Collections.Generic;
using GlueTrap.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GlueTrap
{
    public class AmbientSound : Singleton<AmbientSound>
    {
        private bool _Activated;

        public override void OnSceneChange(Scene scene, LoadSceneMode mode)
        {
            // If not on menu scene and is not currently playing
            // set playing to true and post the 3 ambient sounds simultaniously
            if (scene.name != "MenuScene" && scene.name != "CourtroomIntro" && scene.name != "CourtroomEnding" && !_Activated)
            {
                    _Activated = true;
                    AkSoundEngine.PostEvent("HouseTone", gameObject);
                    AkSoundEngine.PostEvent("HouseBuzz", gameObject);
                    AkSoundEngine.PostEvent("HouseCreak", gameObject);
                    Debug.Log("Started ambience, " + _Activated);
            }
        }
    }
}
