using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GlueTrap
{
    public class CheatCodes : MonoBehaviour
    {
        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyUp(KeyCode.F))
            {
                SceneManager.LoadScene("DownstairsHallway");
            }            
            if (Input.GetKeyUp(KeyCode.G))
            {
                SceneManager.LoadScene("Hallway1");
            }
        }
    }
}
