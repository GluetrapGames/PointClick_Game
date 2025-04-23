using System;
using System.Collections;
using GlueTrap.Utilities;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

namespace GlueTrap
{
public class InteractDialgoue : MonoBehaviour
{
	public bool m_Log;
	public InteractionDir m_InteractionDirection = InteractionDir.Bottom;
	public bool m_Interacting;

	[SerializeField,
	 Tooltip("Conversation to start. Leave blank for no conversation."),
	 ConversationPopup(false, true)]
	private string _ConversationTitle;

	[SerializeField,
	 Tooltip("Tick if the conversation is to only be played once.")]
	private bool _PlayOnce;
	[SerializeField]
	private bool _recordInteraction;
	[SerializeField]
	private bool _calcCollisions = true;

	private Vector3Int _CellPosition;
	private CollideCheck _collideCheck;
	private BoxCollider2D _Collider;
	private GameManager _GameManager;
	private bool _HasPlayedOnce;
	private InputAction _InteractAction;
	private bool _isController;
	private CollideCheck _ItemCollision;
	private PlayerInput _PlayerInput;

	private void Awake()
	{
		// Obtain Game Manager.
		_GameManager = Utils.GetGameManager();
		_ItemCollision = GetComponent<CollideCheck>();
		_PlayerInput = _GameManager.m_Player.GetComponent<PlayerInput>();
		_Collider = GetComponent<BoxCollider2D>();
	}

	private void Start()
	{
		// Recalculate collision bounds.
		var boxCollider = GetComponent<BoxCollider2D>();
		var spriteRenderer = GetComponent<SpriteRenderer>();
		_collideCheck = GetComponent<CollideCheck>();
		if (_calcCollisions)
		{
			Sprite spriteObj = null;
			if (spriteRenderer)
				spriteObj = spriteRenderer.sprite;
			if (!spriteObj ||
			    !Utils.RecalculateCollisionBounds(spriteObj, ref boxCollider))
			{
				Debug.LogWarning(
					$"<{name}>: Failed to resize Collision Bounds!");
			}
		}


		_InteractAction = _PlayerInput.actions["Break"];
	}

	private void Update()
	{
		// Don't allow interaction if in conversation or menus.
		if (_GameManager.m_CurrentState is States.Talking or States.InMenus)
			return;

		// If there is no active conversation and the collider is disabled, re-enable it.
		if (!DialogueManager.instance.isConversationActive &&
		    !_Collider.enabled)
			_Collider.enabled = true;

		_isController = Gamepad.current != null;
		MouseInteraction();

		// Play the conversion once the Player has reached the object.
		if (!_GameManager.m_Player.m_DestinationReached ||
		    _GameManager.m_Player.m_Destination != _CellPosition) return;
		if (_isController)
		{
			ControllerInteraction();
			return;
		}

		PlayConversation();
	}

	private void ControllerInteraction()
	{
		if (_collideCheck == false) _collideCheck.enabled = true;
		if (_collideCheck.IsCollided && _InteractAction.WasPressedThisFrame())
			PlayConversation();
	}

	// Handle wall item functionality.
	private void HandleWallFunction()
	{
		Vector3Int cellPosition =
			_GameManager.m_Grid.WorldToCell(transform.position);

		if (m_Log)
			Debug.Log($"Before Loop: {cellPosition}");

		// Determine movement direction based on InteractionDir.
		Vector3Int step;
		var rangeLimit = 100;
		switch (m_InteractionDirection)
		{
			case InteractionDir.Left:
				step = Vector3Int.left;
				rangeLimit = cellPosition.x - rangeLimit;
				break;
			case InteractionDir.Right:
				step = Vector3Int.right;
				rangeLimit = cellPosition.x + rangeLimit;
				break;
			case InteractionDir.Top:
				step = Vector3Int.up;
				rangeLimit = cellPosition.y + rangeLimit;
				break;
			case InteractionDir.Bottom:
				step = Vector3Int.down;
				rangeLimit = cellPosition.y - rangeLimit;
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}

		// Find a valid tile within range.
		while (!_GameManager.m_NavMesh.HasTile(cellPosition) && (step.x == 0
			       ? cellPosition.y != rangeLimit
			       : cellPosition.x != rangeLimit))
		{
			cellPosition += step;
			if (m_Log) Debug.Log($"In Loop: {cellPosition}");
		}

		// If no valid tile is found, output a warning.
		if (!_GameManager.m_NavMesh.HasTile(cellPosition))
		{
			Debug.LogWarning(
				$"No valid tile found in range of 100 tiles: {cellPosition}");
			return;
		}

		if (m_Log)
		{
			Debug.Log($"After Loop: {cellPosition}");
			_GameManager.m_NavMesh.SetTileFlags(cellPosition,
				TileFlags.None); // Allow colour modification.
			_GameManager.m_NavMesh.SetColor(cellPosition, Color.green);
		}

		// Update object's cell position.
		_CellPosition = cellPosition;

		// Move the player to the target tile.
		_ = _GameManager.m_Player.SetPlayerDestination(cellPosition);
	}

	private void MouseInteraction()
	{
		if (!Input.GetMouseButtonDown(0))
			return;

		Vector2 mousePos =
			_GameManager.m_Camera.ScreenToWorldPoint(Input.mousePosition);

		// Create a layer mask to ignore the "Player" & "Highlighter" layers.
		var layerMask = ~LayerMask.GetMask("Player", "Highlighter");

		RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero,
			Mathf.Infinity, layerMask);

		if (m_Log) Debug.Log(hit.collider);

		// Ensure the clicked collider is the one attached to this GameObject.
		if (hit.collider && hit.collider == GetComponent<Collider2D>())
			HandleWallFunction();
	}

	// Plays the conversation
	private void PlayConversation()
	{
		if (!_recordInteraction)
		{
			if (_PlayOnce)
			{
				if (!_HasPlayedOnce)
				{
					DialogueManager.StartConversation(_ConversationTitle);
					m_Interacting = true;
					StartCoroutine(resetInteracting());
					_Collider.enabled = false;
					_HasPlayedOnce = true;
				}
			}
			else
			{
				if (!_HasPlayedOnce)
				{
					Debug.LogWarning("STARTING CONVO");
					DialogueManager.StartConversation(_ConversationTitle);
					m_Interacting = true;
					StartCoroutine(resetInteracting());
					StartCoroutine(restartDialogueInteraction());
					_HasPlayedOnce = true;
				}
			}
		}
		else if (DialogueLua.GetVariable("Has_Record").asBool)
		{
			var _HeldItem = _GameManager.m_InventoryManager.m_HeldItemSlot
				.GetComponentInChildren<InventoryItem>();
			var _RecordPlayerBreakableRef = GameObject
				.FindGameObjectWithTag("Record").GetComponent<BreakableItem>();
			if (!_HeldItem || _RecordPlayerBreakableRef._itemHp <
			    _RecordPlayerBreakableRef._itemMaxHp) return;
			if (!_HasPlayedOnce &&
			    _HeldItem.itemData.m_Item.m_Type == ItemTypes.Record)
			{
				DialogueManager.StartConversation(_ConversationTitle);
				m_Interacting = true;
				StartCoroutine(resetInteracting());
				_Collider.enabled = false;
				_HasPlayedOnce = true;
			}
		}
	}

	private IEnumerator resetInteracting()
	{
		yield return new WaitUntil(() => !DialogueManager.isConversationActive);
		m_Interacting = false;
	}

	private IEnumerator restartDialogueInteraction()
	{
		yield return new WaitUntil(() => !DialogueManager.isConversationActive);
		Debug.Log("CONVERSATION ENDED");
		yield return new WaitUntil(() =>
			_GameManager.m_Player.m_Destination != _CellPosition);
		_HasPlayedOnce = false;
	}
}
}