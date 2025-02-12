using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.EventSystems;

public class AutoplaySelection : MonoBehaviour
{
   
    public GameObject continueButton;
    public GameObject autoplayButton;

    public ConversationControl conversationControlRef;

    public void setContinueButtonState()
    {
        if (!conversationControlRef.autoplay)
        {
            EventSystem.current.SetSelectedGameObject(continueButton);
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(autoplayButton);
        }
    }
    
}
