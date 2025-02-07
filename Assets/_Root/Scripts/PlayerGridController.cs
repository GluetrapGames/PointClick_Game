using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

[RequireComponent(typeof(GridMovement))]
public class PlayerGridController : MonoBehaviour
{
	[SerializeField]
	private float m_MoveSpeed = 5f;
	[SerializeField]
	[Range(0f, 0.5f)]
	private float m_InputCooldown = 0.2f;
	[SerializeField]
	private GameObject m_HighlightPrefab;

	private Camera _camera;
	private GameObject _highlight;
	private Vector2 _inputDirection;
	private float _lastMoveTime;
	private GridMovement _movement;
	private bool _usingController;

	private void Awake()
	{
		_camera = Camera.main;
		_movement = GetComponent<GridMovement>();
		_highlight = Instantiate(m_HighlightPrefab);
		_highlight.SetActive(false);
	}

	public void HandleMovement()
	{
		_usingController = Gamepad.current != null;

		// Change movement controls based on input device used.
		if (_usingController)
			HandleControllerMovement();
		else
			MouseMovement();
	}

	// Grid-based movement through a mouse using A*.
	private void MouseMovement()
	{
		Vector3 mouseWorldPosition =
			_camera.ScreenToWorldPoint(Input.mousePosition);

		// Convert the world position to a grid position.
		Vector3Int tilePosition =
			_movement.m_Grid.WorldToCell(mouseWorldPosition);

		// If the tile position is different, update the colour.
		if (_movement.m_PreviousTilePosition.HasValue &&
		    _movement.m_PreviousTilePosition.Value != tilePosition)
			_highlight.SetActive(false);

		// Highlight the current tile if it exists in the tilemap.
		if (_movement.m_Tilemap.HasTile(tilePosition) &&
		    !_movement.m_ObstacleTilemap.HasTile(tilePosition))
		{
			_highlight.transform.position = tilePosition;
			_highlight.SetActive(true);
		}

		// Start pathfinding when the left mouse button is clicked.
		if (Input.GetMouseButtonDown(0))
			_movement.SetDestination(tilePosition);

		_movement.MoveToTile(m_MoveSpeed);

		// Store the current tile position.
		_movement.m_PreviousTilePosition = tilePosition;
	}

	// Direct grid-based movement through a controller.
	private void HandleControllerMovement()
	{
		// Read left stick and D-pad input.
		Vector2 moveInput =
			Gamepad.current?.leftStick.ReadValue() ?? Vector2.zero;
		DpadControl dpad = Gamepad.current?.dpad;

		// Determine movement direction.
		Vector3Int direction = Vector3Int.zero;
		if (moveInput.x > 0.5f || (dpad?.right.isPressed ?? false))
			direction = Vector3Int.right;
		else if (moveInput.x < -0.5f || (dpad?.left.isPressed ?? false))
			direction = Vector3Int.left;
		else if (moveInput.y > 0.5f || (dpad?.up.isPressed ?? false))
			direction = Vector3Int.up;
		else if (moveInput.y < -0.5f || (dpad?.down.isPressed ?? false))
			direction = Vector3Int.down;

		// Ensure movement only happens after a small delay.
		if (direction != Vector3Int.zero &&
		    Time.time - _lastMoveTime >= m_InputCooldown)
		{
			// Move towards to tile in the desired direction.
			Vector3Int targetTile =
				_movement.m_Grid.WorldToCell(_movement.transform
					.position) + direction;
			if (_movement.m_Tilemap.HasTile(targetTile) &&
			    !_movement.m_ObstacleTilemap.HasTile(targetTile))
			{
				_movement.m_Path = new List<Vector3Int> { targetTile };
				_movement.m_CurrentPathIndex = 0;
				_movement.m_IsMoving = true;
				_lastMoveTime = Time.time;
			}
		}

		if (_movement.m_IsMoving) _movement.MoveAlongPath(m_MoveSpeed);
	}
}