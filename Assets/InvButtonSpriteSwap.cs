using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class InvButtonSpriteSwap : MonoBehaviour
{
   
    public Sprite inactiveSprite;
    public Sprite activeSprite;
    public GameObject buttonToSwap;

    bool _active = false;
    
    public void OnButtonClick()
    {
        if (_active)
        {
            buttonToSwap.GetComponent<Image>().sprite = inactiveSprite;
            _active = false;
        }
        else
        {
            buttonToSwap.GetComponent<Image>().sprite = activeSprite;
            _active = true;
        }
    }
    
}
