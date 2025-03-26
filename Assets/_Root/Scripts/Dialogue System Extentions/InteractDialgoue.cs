using System;
using GlueTrap.Utilities;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

namespace GlueTrap
{
public class InteractDialgoue : MonoBehaviour
{
	public enum InteractionDir
	{
		Left = 0,
		Right = 1,
		Top = 2,
		Bottom = 3
	}

	public bool m_Log;
	public InteractionDir m_InteractionDirection = InteractionDir.Bottom;

	[SerializeField, Tooltip("The title of the conversation to be played.")]
	private string _ConversationTitle;
	[SerializeField,
	 Tooltip("Tick if the conversation is to only be played once.")]
	private bool _PlayOnce;
	private Vector3Int _CellPosition;

	private GameManager _GameManager;
	private bool _HasPlayedOnce;
	private InputAction _InteractAction;
	private CollideCheck _ItemCollision;
	private PlayerInput _PlayerInput;

	private void Awake()
	{
		// Obtain Game Manager.
		_GameManager = Utils.GetGameManager();
		_ItemCollision = GetComponent<CollideCheck>();
		_PlayerInput = _GameManager.m_Player.GetComponent<PlayerInput>();
	}

	private void Start()
	{
		// Recalculate collision bounds.
		var boxCollider = GetComponent<BoxCollider2D>();
		var spriteRenderer = GetComponent<SpriteRenderer>();
		Sprite spriteObj = null;
		if (spriteRenderer)
			spriteObj = spriteRenderer.sprite;
		if (!spriteObj ||
		    !Utils.RecalculateCollisionBounds(spriteObj, ref boxCollider))
			Debug.LogWarning($"<{name}>: Failed to resize Collision Bounds!");

		_InteractAction = _PlayerInput.actions["Break"];
	}

	private void Update()
	{
		// Don't allow interaction if in conversation or menus.
		if (_GameManager.m_CurrentState is States.Talking or States.InMenus)
			return;

		MouseInteraction();

		// Play the conversion once the Player has reached the object.
		if (!_GameManager.m_Player.m_DestinationReached ||
		    _GameManager.m_Player.m_Destination != _CellPosition) return;
		PlayConversation();
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