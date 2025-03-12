using GlueTrap.Utilities;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace GlueTrap
{
public class PauseMenu : MonoBehaviour
{
	public static bool paused;
	public PlayerInput playerInput;

	public GameObject pauseMenuParent;
	public GameObject pauseMenuUI;
	public GameObject settingsMenuUI;
	public GameObject onScreenButton;
	public GameObject inventoryUI;

	public GameObject firstSelectedPause;
	public GameObject firstSelectedSettings;

	private GameManager _GameManager;
	private InputAction _menuAction;

	private void Awake()
	{
		_GameManager = Utils.GetGameManager();
	}

	private void Start()
	{
		playerInput = _GameManager.m_Player.GetComponent<PlayerInput>();
		inventoryUI = _GameManager.m_InventoryManager.m_Inventory.gameObject;
		_menuAction = playerInput.actions["Menu"];
		if (_menuAction == null) Debug.LogError("No menu action found");

		// Assign all WorldSpace & ScreenSpaceCamera Canvases.
		var canvases = FindObjectsByType<Canvas>(FindObjectsInactive.Include,
			FindObjectsSortMode.None);
		foreach (Canvas canvas in canvases)
			if (canvas.renderMode is RenderMode.ScreenSpaceCamera
			    or RenderMode.WorldSpace)
				canvas.worldCamera = _GameManager.m_Camera;
	}

	private void Update()
	{
		if (_menuAction.WasPressedThisFrame())
		{
			Debug.Log("Pausing/Resuming");
			if (paused)
				Resume();
			else
				Pause();
		}
	}

	public void Resume()
	{
		if (DialogueManager.IsConversationActive)
			inventoryUI.SetActive(false);
		else
			inventoryUI.SetActive(true);
		//inventoryUI.SetActive(true);
		Time.timeScale = 1f;
		paused = false;
		pauseMenuParent.SetActive(false);
		//onScreenButton.SetActive(true);
		pauseMenuUI.SetActive(false);
		settingsMenuUI.SetActive(false);
		EventSystem.current.SetSelectedGameObject(null);
	}

	public void Pause()
	{
		//inventoryUI.SetActive(false);
		pauseMenuParent.SetActive(true);
		pauseMenuUI.SetActive(true);
		//onScreenButton.SetActive(false);
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
}