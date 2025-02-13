using System;
using PixelCrushers.DialogueSystem;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	[SerializeField]
	private PlayerGridController m_Player;
	private States _currentState = States.Moving;

	private void Update()
	{
		if (DialogueManager.IsConversationActive)
			_currentState = States.Talking;
		else if // Resume movement when dialogue ends.
			(_currentState == States.Talking)
			_currentState = States.Moving;

		switch (_currentState)
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

		//Debug.Log(m_Player._moveCoroutine);
	}


	public void ChangeGameState(States newState)
	{
		_currentState = newState;
	}
}

public enum States
{
	Moving = 0,
	Talking = 1,
	Interacting = 2,
	InMenus = 3
}