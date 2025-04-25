using PixelCrushers.DialogueSystem;
using UnityEngine;

namespace GlueTrap
{
public class FridgeAnimation : MonoBehaviour
{
	public bool isDrinking;
	[SerializeField]
	private Sprite _OpenFridgeSprite;
	[SerializeField]
	private Sprite _CloseFridgeSprite;

	private readonly Vector3 _ClosePosition = new(3.24f, 3.42f, 0f);
	private readonly Vector3 _OpenPosition = new(2.65f, 3.42f, 9.99f);

	private Animator _Animator;
	private bool _ConvoPlayed;

	private DialogueEntry _DialogueEntry;

	private SpriteRenderer _FridgeSprite;
	private Vector3 _OldPosition;

	private GameObject _Player;

	private void Awake()
	{
		_ConvoPlayed = false;

		// Get the fridge's sprite component
		_FridgeSprite = GetComponentInChildren<SpriteRenderer>();

		_FridgeSprite.sprite = _CloseFridgeSprite;

		_Player = GameObject.FindGameObjectWithTag("Player");
		_Animator = _Player.GetComponentInChildren<Animator>();
	}

	private void Update()
	{
		// Is a conversation playing?
		if (DialogueManager.IsConversationActive &&
		    DialogueManager.lastConversationID == 37)
		{
			// Get the current node ID of the conversation playing.
			_DialogueEntry = DialogueManager.currentConversationState.subtitle
				.dialogueEntry;
			var dialogueID = _DialogueEntry.id;

			// Bring the fridge infront of the player.
			_FridgeSprite.sortingOrder = 6;

			// Change the fridge sprite and position
			_FridgeSprite.sprite = _OpenFridgeSprite;
			gameObject.transform.position = _OpenPosition;

			// Check if the current conversation playing is the fridge convo
			if (dialogueID >= 3 && !_ConvoPlayed)
			{
				_Player.transform.position = new Vector3(1.5f, 1.9f, 0.0f);

				// Play the drinking animation
				// Ensure correct facing direction and pattern visuals.
				var playerRenderer =
					_Player.GetComponentInChildren<SpriteRenderer>();
				playerRenderer.flipX = true;
				playerRenderer.material.SetTextureScale("_Pattern",
					new Vector2(24f, 2f));
				_Animator.Play("John_Drink_Beer");

				// Prevent actions from happening again this frame.
				_ConvoPlayed = true;
			}
		}

		// Reset convo flag and fridge sprite so that it can be played again.
		if (!DialogueManager.isConversationActive)
		{
			// Set the fridge back to behind the player.
			_FridgeSprite.sortingOrder = 1;

			// Reset fridge sprite and position
			_FridgeSprite.sprite = _CloseFridgeSprite;
			gameObject.transform.position = _ClosePosition;

			_ConvoPlayed = false;
		}
	}
}
}