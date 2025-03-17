using GlueTrap.Utilities;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GlueTrap
{
public class InteractDialgoue : MonoBehaviour
{
	[SerializeField, Tooltip("The title of the conversation to be played.")]
	private string _ConversationTitle;
	[SerializeField,
	 Tooltip("Tick if the conversation is to only be pplayed once.")]
	private bool _PlayOnce;

	private GameManager _GameManager;
	private bool _HasPlayedOnce;
	private InputAction _InteractAction;
	private CollideCheck _ItemCollision;
	private PlayerInput _PlayerInput;

	private void Awake()
	{
		// Obtain Game Manager.
		_GameManager = Utils.GetGameManager();
	}

	private void Start()
	{
		_ItemCollision = GetComponent<CollideCheck>();
		_PlayerInput = _GameManager.m_Player.GetComponent<PlayerInput>();
		_InteractAction = _PlayerInput.actions["Break"];
	}

	private void Update()
	{
		// Check if the interact key was pressed and the player was within the collision zone
		if (_InteractAction.WasPressedThisFrame() && _ItemCollision.IsCollided)
			PlayConversation();
	}

	// Plays the conversation
	private void PlayConversation()
	{
		if (_PlayOnce)
		{
			if (!_HasPlayedOnce)
			{
				DialogueManager.StartConversation(_ConversationTitle);
				var collider = GetComponent<BoxCollider2D>();
				collider.enabled = false;
				_HasPlayedOnce = true;
			}
		}
		else
			DialogueManager.StartConversation(_ConversationTitle);
	}
}
}