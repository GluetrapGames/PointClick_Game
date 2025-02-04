using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    
    public static bool paused = false; 
    public PlayerInput playerInput;
    private InputAction _menuAction;
    public GameObject pauseMenuUI;
    public GameObject inventoryUI;

    private void Awake()
    {
        _menuAction = playerInput.actions["Menu"];
        if (_menuAction == null)
        {
            Debug.LogError("No menu action found");
        }
        //bool startPressed =;
    }

    private void Update()
    {
        if (_menuAction.WasPressedThisFrame())
        {
            Debug.Log("Pausing/Resuming");
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
