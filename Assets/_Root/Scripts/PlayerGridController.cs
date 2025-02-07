using System.Collections;
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
	[SerializeField]
	private Vector3Int m_StartingGridPosition;

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
		_highlight = Instantiate(m_HighlightPrefab);
		_highlight.SetActive(false);
	}

	private void Start()
	{
		_movement.TeleportToTile(m_StartingGridPosition);
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

		if (_movement.m_PreviousTilePosition.HasValue &&
		    _movement.m_PreviousTilePosition.Value != tilePosition)
			_highlight.SetActive(false);

		if (_movement.m_NavMesh.HasTile(tilePosition) &&
		    !_movement.m_ObstacleTilemap.HasTile(tilePosition))
		{
			_highlight.transform.position = tilePosition;
			_highlight.SetActive(true);
		}

		// On click, set destination and start the movement coroutine
		// (if not already running).
		if (Input.GetMouseButtonDown(0))
		{
			_movement.SetDestination(tilePosition);
			_moveCoroutine ??= StartCoroutine(MovementCoroutine());
		}

		_movement.m_PreviousTilePosition = tilePosition;
	}

	// Controller-based grid movement.
	private void HandleControllerMovement()
	{
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

		if (direction == Vector3Int.zero ||
		    !(Time.time - _lastMoveTime >= m_InputCooldown)) return;

		Vector3Int targetTile =
			_movement.m_Grid.WorldToCell(_movement.transform.position) +
			direction;

		if (!_movement.m_NavMesh.HasTile(targetTile) ||
		    _movement.m_ObstacleTilemap.HasTile(targetTile)) return;

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
			_movement.MoveToTile(m_MoveSpeed);
			yield return null;
		}

		_moveCoroutine = null;
	}
}