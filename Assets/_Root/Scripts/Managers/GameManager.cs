using System;
using PixelCrushers.DialogueSystem;

public class GameManager : Singleton<GameManager>
{
	public PlayerGridController m_Player;
	public States m_CurrentState = States.Moving;

	protected override void Awake()
	{
		m_Player = FindFirstObjectByType<PlayerGridController>();
		base.Awake();
	}

#if UNITY_EDITOR
	private void Reset()
	{
		m_Player = FindFirstObjectByType<PlayerGridController>();
	}
#endif

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