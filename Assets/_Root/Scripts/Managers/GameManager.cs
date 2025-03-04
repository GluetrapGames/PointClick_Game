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

namespace GlueTrap
{
public class GameManager : Singleton<GameManager>
{
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

	public InventoryManager m_InventoryManager { get; private set; }
	public PlayerGridController m_Player { get; private set; }
	public Grid m_Grid { get; private set; }
	public Tilemap m_NavMesh { get; private set; }
	public States m_CurrentState => _CurrentState;
	public Camera m_Camera { get; private set; }
	public List<string> m_NoneGameplayScenes => _NoneGameplayScenes;
	public Scene m_CurrentScene { get; private set; }


	protected override void Awake()
	{
		base.Awake();
		m_InventoryManager = FindFirstObjectByType<InventoryManager>();
		InitGame();
		_PreviousState = _CurrentState;
	}

	private void Update()
	{
		if (DialogueManager.IsConversationActive)
			ChangeGameState(States.Talking);

		switch (m_CurrentState)
		{
			case States.Moving:
				m_Player.HandleMovement();
				break;
			case States.Talking:
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
		// Update current Scene.
		m_CurrentScene = scene;

		// Make sure we are in a gameplay scene.
		if (m_NoneGameplayScenes.Any(noneGameplayScene =>
			    scene.name == noneGameplayScene))
		{
			m_Player.gameObject.SetActive(false);
			ChangeGameState(States.InMenus);
			return;
		}

		// If this is a gameplay scene and the Player is not active, turn them on.
		ChangeGameState(States.Moving);
		if (!m_Player.gameObject.activeInHierarchy)
			m_Player.gameObject.SetActive(true);

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
		_PreviousState = _CurrentState;
		_CurrentState = newState;
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