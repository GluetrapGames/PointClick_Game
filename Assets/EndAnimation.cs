using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GlueTrap
{
    public class EndAnimation : MonoBehaviour
    {
        void Start()
        {
        
        }

        void Update()
        {
            var IsAlbertHidden = DialogueLua.GetVariable("AlbertHidden").AsBool;
            if (!DialogueManager.IsConversationActive && DialogueManager.LastConversationID == 47) 
            {
                if (IsAlbertHidden)
                {
                    SceneManager.LoadScene("Cupboard");
                }
                else 
                {
                    SceneManager.LoadScene("CourtScene 4");
                }
            }

        }
    }
}
