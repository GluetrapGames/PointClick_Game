using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    
    public static bool paused = false; 
    public PlayerInput playerInput;
    private InputAction _menuAction;
    
    public GameObject pauseMenuParent;
    public GameObject pauseMenuUI;
    public GameObject settingsMenuUI;
    public GameObject inventoryUI;

    public GameObject firstSelectedPause;
    public GameObject firstSelectedSettings;

    private void Awake()
    {
        _menuAction = playerInput.actions["Menu"];
        if (_menuAction == null)
        {
            Debug.LogError("No menu action found");
        } 
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
        pauseMenuParent.SetActive(false);
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);

    }
    
    public void Pause()
    {
        inventoryUI.SetActive(false);
        pauseMenuParent.SetActive(true);
        pauseMenuUI.SetActive(true);
        settingsMenuUI.SetActive(false);
        EventSystem.current.SetSelectedGameObject(firstSelectedPause);
        Time.timeScale = 0f;
        paused = true;
    }

    public void OpenSettings()
    {
        EventSystem.current.SetSelectedGameObject(firstSelectedSettings);
    }

    public void CloseSettings()
    {
        EventSystem.current.SetSelectedGameObject(firstSelectedPause);
    }
    
}
