using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PauseMenu : MonoBehaviour
{
    
    public static bool paused = false;
    public GameObject pauseMenuUI;
    public GameObject inventoryUI;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        inventoryUI.SetActive(true);
        Time.timeScale = 1f;
        paused = false;
        pauseMenuUI.SetActive(false);  
    }
    
    public void Pause()
    {
        inventoryUI.SetActive(false);
        pauseMenuUI.SetActive(true);  
        Time.timeScale = 0f;
        paused = true;
    }
    
}
