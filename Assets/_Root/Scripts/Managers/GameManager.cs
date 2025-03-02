using System;
using _Root.Scripts.Utilities;
using EditorAttributes;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using Void = EditorAttributes.Void;

public class GameManager : Singleton<GameManager>
{
	[HideInInspector]
	public InventoryManager m_InventoryManager;
	public PlayerGridController m_Player;
	public States m_CurrentState = States.Moving;

	[SerializeField, FoldoutGroup("Managers", nameof(m_InventoryManager)),
	 PropertyOrder(-1)]
	private Void _MangerGroupHeader;
	[SerializeField]
	private GameObject _PlayerPrefab;
	[SerializeField, ReadOnly]
	private Transform _PlayerSpawnPoint;


	protected override void Awake()
	{
		base.Awake();
		m_InventoryManager = FindFirstObjectByType<InventoryManager>();
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

	public override void OnSceneChange(Scene scene, LoadSceneMode mode)
	{
		// Get Player spawner.
		_PlayerSpawnPoint = Utils.FindSpawner("PlayerSpawner");
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