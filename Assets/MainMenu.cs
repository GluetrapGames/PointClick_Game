using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    
    public GameObject settingsMenuUI;
    public GameObject mainMenuUI;

    public GameObject firstSelectedMainMenu;
    public GameObject firstSelectedSettings;

    public void OpenSettings()
    {
        EventSystem.current.SetSelectedGameObject(firstSelectedSettings);
    }

    public void CloseSettings()
    {
        EventSystem.current.SetSelectedGameObject(firstSelectedMainMenu);
    }
    
}
