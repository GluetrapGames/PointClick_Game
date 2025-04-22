using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GlueTrap
{
    public class PickupSounds : MonoBehaviour
    {
        public string pickupType;

    public void onPickup()
        {
            AkSoundEngine.SetSwitch("PickupItem", pickupType, gameObject);
            AkSoundEngine.PostEvent("player_pickup", gameObject);
        }
    }
}
