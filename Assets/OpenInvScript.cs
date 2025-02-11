using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenInvScript : MonoBehaviour
{
    
    public GameObject inventoryUI;

    private bool _invOpen;

    public void OnButtonClick()
    {
        if (_invOpen)
        {
            inventoryUI.SetActive(false);
            _invOpen = false;
        }
        else
        {
            inventoryUI.SetActive(true);
            _invOpen = true;
        }
    }
    
}
