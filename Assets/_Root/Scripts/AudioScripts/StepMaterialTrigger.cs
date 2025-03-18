using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GlueTrap
{
    public class StepMaterialTrigger : MonoBehaviour
    {
        public string stepMaterial;

        // Sets switch in Wwise for each npc, player etc
        public void OnTriggerEnter2D(Collider2D other)
        {
            AkSoundEngine.SetSwitch("FootstepMaterial", stepMaterial, other.gameObject);
        }

        public void OnTriggerExit2D(Collider2D other)
        {
            AkSoundEngine.SetSwitch("FootstepMaterial", "Wood", other.gameObject);
        }
    }
}
