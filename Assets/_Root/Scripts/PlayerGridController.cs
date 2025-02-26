using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

[RequireComponent(typeof(GridMovement))]
public class PlayerGridController : MonoBehaviour
{
	[SerializeField, Range(0f, 10f)]
	private float _moveSpeed = 5f;
	[SerializeField, Range(0f, 0.5f)]
	private float _inputCooldown = 0.15f;
	[SerializeField]
	private GameObject _highlightPrefab;
	[SerializeField]
	private Vector3Int _startingGridPosition;

	private Camera _camera;
	private GameObject _highlight;
	private Vector2 _inputDirection;
	private float _lastMoveTime;
	private Coroutine _moveCoroutine;
	private GridMovement _movement;
	private bool _usingController;


	private void Awake()
	{
		_camera = Camera.main;
		_movement = GetComponent<GridMovement>();
		_highlight = Instantiate(_highlightPrefab);
		_highlight.SetActive(false);
	}

	private void Start()
	{
		_movement.TeleportToTile(_startingGridPosition);
	}


	public void HandleMovement()
	{
		_usingController = Gamepad.current != null;

		// Choose controls based on input device.
		if (_usingController)
			HandleControllerMovement();
		else
			MouseMovement();
	}

	// Mouse-based grid movement.
	private void MouseMovement()
	{
		Vector3 mouseWorldPosition =
			_camera.ScreenToWorldPoint(Input.mousePosition);
		Vector3Int tilePosition =
			_movement.m_Grid.WorldToCell(mouseWorldPosition);

		// Un-highlight the tile when moving onto another tile.
		if (_movement.m_PreviousTilePosition.HasValue &&
		    _movement.m_PreviousTilePosition.Value != tilePosition)
			_highlight.SetActive(false);

		// Highlight tile if valid and has no obstacle on it.
		if (_movement.m_NavMesh.HasTile(tilePosition) &&
		    !_movement.m_ObstaclesPositions.Contains(tilePosition))
		{
			_highlight.transform.position = tilePosition;
			_highlight.transform.position += new Vector3(0.5f, 0.5f, 0f);
			_highlight.SetActive(true);
		}

		// On click, set destination and start the movement coroutine
		// (if not already running).
		if (Input.GetMouseButtonDown(0))
			_ = SetPlayerDestination(tilePosition);
	}

	public Vector3Int SetPlayerDestination(Vector3Int targetPosition)
	{
		// Set new target position.
		_movement.SetDestination(targetPosition);
		_moveCoroutine ??= StartCoroutine(MovementCoroutine());

		_movement.m_PreviousTilePosition = targetPosition;

		return targetPosition;
	}

	// Controller-based grid movement.
	private void HandleControllerMovement()
	{
		// Ensure the tile highlight is turned off when using the controller.
		if (_highlight.activeInHierarchy)
			_highlight.SetActive(false);

		Vector2 moveInput =
			Gamepad.current?.leftStick.ReadValue() ?? Vector2.zero;
		DpadControl dpad = Gamepad.current?.dpad;

		Vector3Int direction = Vector3Int.zero;
		if (moveInput.x > 0.5f || (dpad?.right.isPressed ?? false))
			direction = Vector3Int.right;
		else if (moveInput.x < -0.5f || (dpad?.left.isPressed ?? false))
			direction = Vector3Int.left;
		else if (moveInput.y > 0.5f || (dpad?.up.isPressed ?? false))
			direction = Vector3Int.up;
		else if (moveInput.y < -0.5f || (dpad?.down.isPressed ?? false))
			direction = Vector3Int.down;

		// If no movement detected or cooldown still active, return.
		if (direction == Vector3Int.zero ||
		    !(Time.time - _lastMoveTime >= _inputCooldown)) return;

		Vector3Int targetTile =
			_movement.m_Grid.WorldToCell(_movement.transform.position) +
			direction;

		// Prevent movement if tile not part of NavMesh or has an obstacle in it.
		if (!_movement.m_NavMesh.HasTile(targetTile) ||
		    _movement.m_ObstaclesPositions.Contains(targetTile)) return;

		_movement.m_Path = new List<Vector3Int> { targetTile };
		_movement.m_CurrentPathIndex = 0;
		_movement.m_IsMoving = true;
		_lastMoveTime = Time.time;
		_moveCoroutine ??= StartCoroutine(MovementCoroutine());
	}

	// Coroutine that moves the player along the path until complete.
	private IEnumerator MovementCoroutine()
	{
		while (_movement.m_IsMoving)
		{
			_movement.MoveToTile(_moveSpeed);
			yield return null;
		}

		_moveCoroutine = null;
	}


	public void StopMovement()
	{
		StopCoroutine(_moveCoroutine);
		_moveCoroutine = null;
		_movement.m_Path.Clear();
		_movement.m_IsMoving = false;
	}
}