using System;
using _Root.Scripts.Utilities;
using Cinemachine;
using EditorAttributes;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Void = EditorAttributes.Void;

public class GameManager : Singleton<GameManager>
{
	[HideInInspector]
	public InventoryManager m_InventoryManager;
	public PlayerGridController m_Player;

	[SerializeField, FoldoutGroup("Managers", nameof(m_InventoryManager)),
	 PropertyOrder(-1)]
	private Void _MangerGroupHeader;
	[SerializeField]
	private GameObject _PlayerPrefab;
	[SerializeField]
	private GameObject _PlayerCameraPrefab;
	[SerializeField, ReadOnly]
	private Transform _PlayerSpawnPoint;

	[ReadOnly] public Grid m_Grid { get; private set; }
	[ReadOnly] public Tilemap m_NavMesh { get; private set; }
	[ReadOnly]
	public States m_CurrentState { get; private set; } = States.Moving;
	public Camera m_Camera { get; private set; }


	protected override void Awake()
	{
		base.Awake();
		m_InventoryManager = FindFirstObjectByType<InventoryManager>();

		InitGame();
	}

	private void Update()
	{
		if (DialogueManager.IsConversationActive)
			m_CurrentState = States.Talking;
		else if // Resume movement when dialogue ends.
			(m_CurrentState == States.Talking)
			m_CurrentState = States.Moving;

		switch (m_CurrentState)
		{
			case States.Moving:
				m_Player.HandleMovement();
				break;
			case States.Talking:
				break;
			case States.Interacting:
				break;
			case States.InMenus:
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private void InitGame()
	{
		// Spawn Camera.
		CinemachineVirtualCamera cinemachineCamera = null;
		if (!FindFirstObjectByType<Camera>())
		{
			GameObject cameraObj = Instantiate(_PlayerCameraPrefab, transform);
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
		if (!_PlayerSpawnPoint) return;

		// Make sure we don't already have the Player.
		var obj = FindFirstObjectByType<PlayerGridController>();
		if (obj && m_Player == obj) return;

		// Spawn Player.
		GameObject spawnedPlayer = Instantiate(_PlayerPrefab,
			_PlayerSpawnPoint.position, Quaternion.identity);
		spawnedPlayer.transform.parent = transform;
		m_Player = spawnedPlayer.GetComponent<PlayerGridController>();

		// Update Cinemachine Camera Follow Target.
		if (cinemachineCamera)
			cinemachineCamera.Follow = m_Player.transform;
	}

	public override void OnSceneChange(Scene scene, LoadSceneMode mode)
	{
		// Get the Grid and the Navmesh.
		m_Grid = FindFirstObjectByType<Grid>();
		m_NavMesh = GameObject.FindGameObjectWithTag("NavMesh")
			.GetComponent<Tilemap>();

		// Make sure that both objects could be found.
		if (!m_Grid || !m_NavMesh)
		{
			Debug.LogError(
				"The Grid or walkable Tilemap could not be found in the scene!");
			return;
		}

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

	public void ChangeGameState(States newState)
	{
		m_CurrentState = newState;
	}
}

public enum States
{
	Moving = 0,
	Talking = 1,
	Interacting = 2,
	InMenus = 3
}