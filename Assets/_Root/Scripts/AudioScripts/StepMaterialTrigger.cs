using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GlueTrap
{
    public class StepMaterialTrigger : MonoBehaviour
    {
        public string stepMaterial;
        private Scene currentScene;
        private void Awake()
        {
            currentScene = SceneManager.GetActiveScene();
        }
        // Sets switch in Wwise for each npc, player etc
        public void OnTriggerEnter2D(Collider2D other)
        {
            AkSoundEngine.SetSwitch("FootstepMaterial", stepMaterial, other.gameObject);
        }

        public void OnTriggerExit2D(Collider2D other)
        {
            if (currentScene.name == "Outside")
                AkSoundEngine.SetSwitch("FootstepMaterial", "Grass",other.gameObject);

            if (currentScene.name != "Outside")
                AkSoundEngine.SetSwitch("FootstepMaterial", "Wood", other.gameObject);
        }
    }
}
