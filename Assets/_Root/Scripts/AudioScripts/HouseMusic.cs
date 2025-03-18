using System.Collections;
using System.Collections.Generic;
using GlueTrap.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GlueTrap
{
    public class HouseMusic : Singleton<HouseMusic>
    {
        public string MusicState;
        private bool _Activated;


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

        public override void OnSceneChange(Scene scene, LoadSceneMode mode)
        {
            {
                if (scene.name != "MenuScene" && !_Activated)
                {
                    _Activated = true;
                    AkSoundEngine.SetState("HouseMusic", MusicState);
                    AkSoundEngine.PostEvent("MusicHouse", gameObject);
                    Debug.Log("Started music, " + _Activated);
                    ;
                }
            }
        }
    }
}
