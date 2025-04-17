using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using EditorAttributes;
using GlueTrap.Utilities;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.Video;

namespace GlueTrap
{
public class GameManager : Singleton<GameManager>
{
	public Camera m_Camera { get; private set; }
	public Scene m_CurrentScene { get; private set; }
	public States m_CurrentState => _CurrentState;
	public EndGameTracker m_EndGameTracker { get; private set; }
	public Grid m_Grid { get; private set; }

	public InventoryManager m_InventoryManager { get; private set; }
	public Tilemap m_NavMesh { get; private set; }
	public List<string> m_NoneGameplayScenes => _NoneGameplayScenes;
	public PlayerGridController m_Player { get; private set; }
	public RoomEntryPoints m_RoomPoint = RoomEntryPoints.None;

	public List<string> m_UniqueRoomList;
	public int m_TotalUniqueRooms;
	public bool m_hasCrowbar;
	public int m_collectedMoney;
	public bool m_HasFlatCall;
	public bool m_hasUpstairsCourt;
	public bool m_hasTaxidermyKey;
	public bool m_hasFrontdoorKey;
	public int m_totalItemsDestroyed;
	public int m_totalItemsPickedUp;
	public double m_moneyAfterMeek;
	public bool m_HasEntered;

	[SerializeField, ReadOnly]
	private States _CurrentState = States.Moving;
	[SerializeField, SceneDropdown]
	private List<string> _NoneGameplayScenes;
	[SerializeField]
	private GameObject _PlayerCameraPrefab;
	[SerializeField]
	private GameObject _PlayerPrefab;
	[SerializeField, ReadOnly]
	private Transform _PlayerSpawnPoint;
	private States _PreviousState;
	private string _PreviousTitleCard;
	private TitleCard _TitleCard;
	private bool _TitleCardPlayed;


	protected override void Awake()
	{
		base.Awake();
		m_InventoryManager = FindFirstObjectByType<InventoryManager>();
		m_EndGameTracker = FindFirstObjectByType<EndGameTracker>();
		//Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
		InitGame();
		_PreviousState = _CurrentState;
	}

	private void Update()
	{
		if (DialogueManager.IsConversationActive)
			ChangeGameState(States.Talking);

		m_HasFlatCall = DialogueLua.GetVariable("MarkPhoneCallFinished").asBool;

		// If the scene has a TitleCard, set the Player's and UI visibility
		// based on if the TitleCard is playing or not.
		if (_TitleCard && !_TitleCardPlayed)
		{
			m_Player.gameObject.SetActive(!_TitleCard.m_IsPlaying);
			m_InventoryManager.m_Inventory.gameObject.SetActive(
				!_TitleCard.m_IsPlaying);

			if (!_TitleCard.m_IsPlaying)
				_PreviousTitleCard = _TitleCard.name;
		}

		switch (m_CurrentState)
		{
			case States.Moving:
				m_InventoryManager.m_Inventory.gameObject.SetActive(true);
				m_Player.HandleMovement();
				break;
			case States.Talking:
				m_InventoryManager.m_Inventory.gameObject.SetActive(false);

				// Return to past state once dialogue ends.
				if (DialogueManager.IsConversationActive) break;
				ChangeGameState(m_NoneGameplayScenes.Any(
					noneGameplayScene =>
						m_CurrentScene.name == noneGameplayScene)
					? States.InMenus
					: States.Moving);

				break;
			case States.Interacting:
				break;
			case States.InMenus:
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	public void calcMoneyMeek()
	{
		float money = m_collectedMoney;
		var itemsDestroyed = m_totalItemsDestroyed;
		var envScore = DialogueLua.GetVariable("Env_DM_Meter").asInt;
		double offset = money;

		if (itemsDestroyed != 0)
			offset = money / (1 + itemsDestroyed * 0.35);
		else
			offset = money;

		var newEnv = envScore + offset;
		m_moneyAfterMeek = newEnv;
		DialogueLua.SetVariable("Env_DM_Meter", newEnv);
	}

	public void ChangeGameState(States newState)
	{
		_PreviousState = _CurrentState;
		_CurrentState = newState;
	}

	public override void OnSceneChange(Scene scene, LoadSceneMode mode)
	{
		// Update current Scene.
		m_CurrentScene = scene;

		if (m_Player)
			m_Player.m_Movement.m_Path.Clear();

		HandleCameraLogic(scene);

		// Make ignore gameplay logic if in a non-gameplay scene.
		if (m_NoneGameplayScenes.Any(noneGameplayScene =>
			    scene.name == noneGameplayScene))
		{
			if (m_Player)
				m_Player.gameObject.SetActive(false);
			ChangeGameState(States.InMenus);
			return;
		}

		// Try and get a Title Card object.
		_TitleCard = FindFirstObjectByType<TitleCard>();
		_TitleCardPlayed = _TitleCard?.name == _PreviousTitleCard;
		if (_TitleCardPlayed)
			_TitleCard?.gameObject.SetActive(false);

		// Get the Grid and the Navmesh.
		m_Grid = FindFirstObjectByType<Grid>();
		var navMeshObj = GameObject.FindGameObjectWithTag("NavMesh");

		// Make sure that both objects could be found.
		if (!m_Grid || !navMeshObj)
		{
			Debug.LogWarning(
				"The Grid or walkable Tilemap could not be found in the scene!");
			return;
		}

		m_NavMesh = navMeshObj.GetComponent<Tilemap>();

		// Get Player spawner.
		_PlayerSpawnPoint = Utils.FindSpawner("PlayerSpawner");
		if (!_PlayerSpawnPoint) return;

		// Move Player to spawner.
		if (!m_Player)
		{
			Debug.LogError("No Player found in the scene!");
			return;
		}

		// Update Player position.
		m_Player.SetPositionInGrid(_PlayerSpawnPoint.position);
	}

	private void HandleCameraLogic(Scene scene)
	{
		// If this is a gameplay scene and the Player is not active, turn them on.
		ChangeGameState(States.Moving);
		if (!m_Player.gameObject.activeInHierarchy)
			m_Player.gameObject.SetActive(true);

		// Handle Camera logic.
		var virtualCamera =
			m_Camera.GetComponentInParent<CinemachineVirtualCamera>();

		UpdateWorldCanvases();
		UpdateVideoPlayers();

		// If the follow target is null, make it the player.
		if (virtualCamera.m_Follow == null)
			virtualCamera.m_Follow = m_Player.transform;

		// Remove the camera's follow target if on the Outside scene.
		if (scene.name == "Outside")
		{
			virtualCamera.m_Follow = null;
			virtualCamera.transform.position = new Vector3(0f, 0f, -10f);
		}
	}

	private void InitGame()
	{
		// Spawn Camera.
		CinemachineVirtualCamera cinemachineCamera = null;
		if (!FindFirstObjectByType<Camera>())
		{
			GameObject cameraObj =
				Instantiate(_PlayerCameraPrefab, transform);
			m_Camera = cameraObj.GetComponent<Camera>();

			// Check if any of the components are on the parent.
			if (!m_Camera)
			{
				Debug.LogWarning(
					$"{m_Camera}: Trying to get the Camera component from children.");
				m_Camera = cameraObj.GetComponentInChildren<Camera>();
			}

			// Try to obtain the VirtualCamera.
			cinemachineCamera =
				cameraObj.GetComponent<CinemachineVirtualCamera>();
			if (!cinemachineCamera)
			{
				Debug.LogWarning(
					$"{m_Camera}: Trying to get the CinemachineVirtualCamera " +
					"component from children.");
				cinemachineCamera = cameraObj
					.GetComponentInChildren<CinemachineVirtualCamera>();
			}
		}

		// Get Player spawner.
		_PlayerSpawnPoint = Utils.FindSpawner("PlayerSpawner");
		Vector3 spawnPos = _PlayerSpawnPoint
			? _PlayerSpawnPoint.position
			: new Vector3(0f, 0f, 9.99f);

		// Make sure we don't already have the Player.
		var obj = FindFirstObjectByType<PlayerGridController>();
		if (obj && m_Player == obj) return;

		// Spawn Player.
		GameObject spawnedPlayer =
			Instantiate(_PlayerPrefab, spawnPos, Quaternion.identity);
		spawnedPlayer.transform.parent = transform;
		m_Player = spawnedPlayer.GetComponent<PlayerGridController>();

		// Update Cinemachine Camera Follow Target.
		if (cinemachineCamera)
			cinemachineCamera.Follow = m_Player.transform;
	}

	private void UpdateVideoPlayers()
	{
		var obj = FindObjectsByType<VideoPlayer>(FindObjectsInactive.Include,
			FindObjectsSortMode.None);

		foreach (VideoPlayer vidPlayer in obj)
			if (vidPlayer.renderMode is VideoRenderMode.CameraFarPlane
			    or VideoRenderMode.CameraNearPlane)
				vidPlayer.targetCamera = m_Camera;
	}

	private void UpdateWorldCanvases()
	{
		var obj = FindObjectsByType<Canvas>(FindObjectsInactive.Include,
			FindObjectsSortMode.None);

		foreach (Canvas canvas in obj)
			canvas.worldCamera = m_Camera;
	}
}

public enum States
{
	Moving = 0,
	Talking = 1,
	Interacting = 2,
	InMenus = 3
}
}