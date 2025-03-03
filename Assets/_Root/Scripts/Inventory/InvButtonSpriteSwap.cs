using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class InvButtonSpriteSwap : MonoBehaviour
{
   
    [SerializeField]
    private GameObject _buttonBuckle;
    [SerializeField]
    private GameObject _buttonToSwap;
    [SerializeField]
    private GameObject _buttonText;
    
    [SerializeField]
    private GameObject _invToSwap;

    [SerializeField]
    private GameObject inventoryTopBox;
    
    bool _active = false;

    public void OnButtonClick()
    {
        if (_active)
        {
            _buttonBuckle.SetActive(true);
            _buttonText.GetComponent<TextMeshProUGUI>().text = "Expand";
            _active = false;
        }
        else
        {
            _buttonBuckle.SetActive(false);
            _buttonText.GetComponent<TextMeshProUGUI>().text = "Close";
            _active = true;
        }
    }
    
}
