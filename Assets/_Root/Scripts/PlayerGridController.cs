using System.Collections;
using System.Collections.Generic;
using EditorAttributes;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

[RequireComponent(typeof(GridMovement))]
public class PlayerGridController : MonoBehaviour
{
	[ReadOnly]
	public GridMovement m_Movement;
	[Range(0f, 10f)]
	public float m_MoveSpeed = 5f;

	[SerializeField]
	private bool _UseCustomStartPos;
	[SerializeField, EnableField(nameof(_UseCustomStartPos))]
	private Vector3Int _startingGridPosition;
	[SerializeField, Range(0f, 0.5f)]
	private float _inputCooldown = 0.15f;
	[SerializeField]
	private GameObject _highlightPrefab;

	private Camera _camera;
	private GameManager _GameManager;
	private GameObject _highlight;
	private Vector2 _inputDirection;
	private float _lastMoveTime;
	private Coroutine _moveCoroutine;
	private bool _usingController;


	private void Awake()
	{
		_GameManager = FindFirstObjectByType<GameManager>();
		m_Movement = GetComponent<GridMovement>();
		_highlight = Instantiate(_highlightPrefab, transform, true);
		_highlight.SetActive(false);
	}

	private void Start()
	{
		_camera = _GameManager.m_Camera;

		if (_UseCustomStartPos)
			m_Movement.TeleportToTile(_startingGridPosition);

		//SetPositionInGrid(transform.position);
	}

	public void SetPositionInGrid(Vector3 position)
	{
		const int searchSizeConst = 2;
		// Get the current tile position.
		Vector3Int tilePosition = _GameManager.m_Grid.WorldToCell(position);

		// Check if the tile at the current position is not blocked.
		if (_GameManager.m_NavMesh.HasTile(tilePosition) &&
		    !m_Movement.m_ObstaclesPositions.Contains(tilePosition))
		{
			transform.position =
				_GameManager.m_Grid.GetCellCenterWorld(tilePosition);
			return;
		}

		// Try finding an unblocked tile within a radius of designated by
		// searchSizeConst.
		Vector3Int bestTile = tilePosition;
		for (var x = -searchSizeConst; x <= searchSizeConst; x++)
		for (var y = -searchSizeConst; y <= searchSizeConst; y++)
		{
			Vector3Int newTile = tilePosition + new Vector3Int(x, y, 0);

			// Check if the tile exists and isn't blocked.
			if (!_GameManager.m_NavMesh.HasTile(newTile) ||
			    m_Movement.m_ObstaclesPositions.Contains(newTile)) continue;

			transform.position =
				_GameManager.m_Grid.GetCellCenterWorld(newTile);
			return;
		}

		// If no valid tile was found, leave the position unchanged.
		Debug.LogWarning(
			$"<{name}> No valid tile found within a {searchSizeConst}-tile radius.");
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
			_GameManager.m_Grid.WorldToCell(mouseWorldPosition);

		// Un-highlight the tile when moving onto another tile.
		if (m_Movement.m_PreviousTilePosition.HasValue &&
		    m_Movement.m_PreviousTilePosition.Value != tilePosition)
			_highlight.SetActive(false);

		// Highlight tile if valid, has no obstacle on it, and isn't the
		// current tile.
		if (_GameManager.m_NavMesh.HasTile(tilePosition) &&
		    !m_Movement.m_ObstaclesPositions.Contains(tilePosition) &&
		    _GameManager.m_Grid.WorldToCell(transform.position) != tilePosition)
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
			_GameManager.m_Grid.WorldToCell(m_Movement.transform.position) +
			direction;

		// Prevent movement if tile not part of NavMesh or has an obstacle in it.
		if (!_GameManager.m_NavMesh.HasTile(targetTile) ||
		    m_Movement.m_ObstaclesPositions.Contains(targetTile)) return;

		m_Movement.m_Path = new List<Vector3Int> { targetTile };
		m_Movement.m_CurrentPathIndex = 0;
		m_Movement.m_IsMoving = true;
		_lastMoveTime = Time.time;
		_moveCoroutine ??= StartCoroutine(MovementCoroutine());
	}

	// Coroutine that moves the player along the path until complete.
	private IEnumerator MovementCoroutine()
	{
		while (m_Movement.m_IsMoving)
		{
			m_Movement.MoveToTile(m_MoveSpeed);
			yield return null;
		}

		_moveCoroutine = null;
	}

	public Vector3Int SetPlayerDestination(Vector3Int targetPosition)
	{
		// Set new target position.
		m_Movement.SetDestination(targetPosition);
		_moveCoroutine ??= StartCoroutine(MovementCoroutine());

		m_Movement.m_PreviousTilePosition = targetPosition;

		return targetPosition;
	}

	public void StopMovement()
	{
		StopCoroutine(_moveCoroutine);
		_moveCoroutine = null;
		m_Movement.m_Path.Clear();
		m_Movement.m_IsMoving = false;
	}
}