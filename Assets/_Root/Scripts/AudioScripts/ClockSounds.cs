using System.Collections;
using System.Collections.Generic;
using GlueTrap.Utilities;
using UnityEngine;

namespace GlueTrap
{
    public class ClockSounds : MonoBehaviour
    {
        [SerializeField]
        private BreakableItem breakItem;
        public AmbientSound ambSound;
        public GameManager gameManager;
        private bool clockPlaying;
        public bool clockBroken;
        [SerializeField]
        private bool _Log;


        // Update is called once per frame
        void Update()
        {
            if (!clockPlaying)
            {
                clockPlaying = true;
                AkSoundEngine.PostEvent("CourtClock", gameObject);
                if (_Log) { Debug.Log("Clock activated: " + clockPlaying); }
            }

            if (breakItem._itemHp <= 0)
            {
                clockBroken = true;
                clockPlaying = false;
                AkSoundEngine.PostEvent("StopClock", gameObject);
                if (_Log) { Debug.Log("Clock activated: " + clockPlaying); }
            }

        }
    }
}
