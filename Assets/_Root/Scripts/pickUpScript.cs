using System;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PickUpScript : MonoBehaviour
{
	public enum InteractionDir
	{
		Left = 0,
		Right = 1,
		Top = 2,
		Bottom = 3
	}

	[Tooltip("Log all major points of interests in the script.")]
	public bool m_Log;
	[Header("Settings")]
	public bool m_IsClicked;
	public bool m_ActivateVariable;
	public InteractionDir m_InteractionDirection = InteractionDir.Left;
	[Range(0, 5)]
	public int m_PickUpDistance = 1;
	[Range(1f, 3f)]
	public float m_ControllerInteractionDistance = 1f;

	[SerializeField]
	private bool _isWallItem;
	[SerializeField]
	private GridMovement _player;
	[SerializeField]
	private Tilemap _navMesh;

	private Camera _camera;


	private void Awake()
	{
		_camera = Camera.main;
		_player = GameObject.FindWithTag("Player")
			?.GetComponent<GridMovement>();
		_navMesh = GameObject.FindWithTag("NavMesh").GetComponent<Tilemap>();
	}

	private void Start()
	{
		gameObject.SetActive(true);
		m_IsClicked = false;
		m_ActivateVariable = false;
	}

	private void Update()
	{
		if (Gamepad.current != null)
			ControllerInteraction();
		else
			MouseInteraction();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!m_IsClicked)
			return;

		Collected();
	}


	private void Collected()
	{
		Debug.Log("Item collected");
		DialogueManager.ShowAlert($"{name} has been collected!");
		m_ActivateVariable = true;
		gameObject.SetActive(false);
	}

	private void HandleItemFunction()
	{
		if (_isWallItem)
			HandleWallFunction();
		else
			HandleGroundFunction();
	}

	// Handle controller interaction.
	private void ControllerInteraction()
	{
		if (_isWallItem)
		{
			// Define a box area below the wall item.
			Vector2 boxCenter = (Vector2)transform.position + new Vector2(
				0, -m_ControllerInteractionDistance * 0.5f);
			var boxSize = new Vector2(m_ControllerInteractionDistance,
				m_ControllerInteractionDistance);

			// Use OverlapBox to see if the player is within that box.
			Collider2D hitCollider =
				Physics2D.OverlapBox(boxCenter, boxSize, 0f);
			if (!hitCollider || hitCollider.gameObject != _player.gameObject)
				return;
		}
		else
		{
			// For ground items, use distance-based circle check.
			if (!(Vector3.Distance(
				      transform.position, _player.transform.position) <
			      m_ControllerInteractionDistance)) return;
		}

		// If controller interaction button was press, then call Collect().
		if (!Gamepad.current.buttonSouth.wasPressedThisFrame) return;
		m_IsClicked = true;
		Collected();
	}

	// Handle mouse interaction.
	private void MouseInteraction()
	{
		if (!Input.GetMouseButtonDown(0))
			return;

		Vector2 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
		RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

		if (hit.collider && hit.collider.gameObject == gameObject)
		{
			m_IsClicked = true;
			HandleItemFunction();
		}
		else
			m_IsClicked = false;
	}

	// Handle ground item functionality.
	private void HandleGroundFunction()
	{
		Vector3Int cellPosition = _navMesh.WorldToCell(transform.position);

		// Based on interaction direction, have player move to that tile instead.
		switch (m_InteractionDirection)
		{
			case InteractionDir.Left:
				cellPosition.x -= 1 * m_PickUpDistance;
				break;
			case InteractionDir.Right:
				cellPosition.x += 1 * m_PickUpDistance;
				break;
			case InteractionDir.Top:
				cellPosition.y += 1 * m_PickUpDistance;
				break;
			case InteractionDir.Bottom:
				cellPosition.y -= 1 * m_PickUpDistance;
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}

		// Move the player to the target tile.
		_player.SetDestination(cellPosition);
		StartCoroutine(_player.MoveAlongPathCoroutine());
	}

	// Handle wall item functionality.
	private void HandleWallFunction()
	{
		Vector3Int cellPosition = _navMesh.WorldToCell(transform.position);

		if (m_Log)
			Debug.Log($"Before Loop: {cellPosition}");

		// Find a valid tile within range.
		while (!_navMesh.HasTile(cellPosition) && cellPosition.y > -100)
		{
			cellPosition.y--;
			if (m_Log) Debug.Log($"In Loop: {cellPosition}");
		}

		// If no valid tile is found, output a warning.
		if (!_navMesh.HasTile(cellPosition))
		{
			Debug.LogWarning(
				$"No valid tile found in range of 100 tiles: {cellPosition}");
			return;
		}

		if (m_Log)
		{
			Debug.Log($"After Loop: {cellPosition}");
			_navMesh.SetTileFlags(cellPosition,
				TileFlags.None); // Allow colour modification.
			_navMesh.SetColor(cellPosition, Color.green);
		}

		// Move the player to the target tile.
		_player.SetDestination(cellPosition);
		StartCoroutine(_player.MoveAlongPathCoroutine(5f));
	}

#if UNITY_EDITOR
	private void Reset()
	{
		_camera = Camera.main;
		_player = GameObject.FindWithTag("Player")
			?.GetComponent<GridMovement>();
		_navMesh = GameObject.FindWithTag("NavMesh").GetComponent<Tilemap>();
	}

	private void OnDrawGizmosSelected()
	{
		// Handle wall item gizmos.
		if (_isWallItem)
		{
			// Draw a box that represents the interaction area.
			Vector2 boxCenter = (Vector2)transform.position + new Vector2
				(0, -m_ControllerInteractionDistance * 0.5f);
			var boxSize = new Vector2(m_ControllerInteractionDistance,
				m_ControllerInteractionDistance);

			Gizmos.color = Color.yellow;
			Gizmos.DrawWireCube(boxCenter, boxSize);
		}
		else // Handle ground item gizmos.
		{
			Vector3Int cellPosition = _navMesh.WorldToCell(transform.position);
			switch (m_InteractionDirection)
			{
				case InteractionDir.Left:
					cellPosition.x -= m_PickUpDistance;
					break;
				case InteractionDir.Right:
					cellPosition.x += m_PickUpDistance;
					break;
				case InteractionDir.Top:
					cellPosition.y += m_PickUpDistance;
					break;
				case InteractionDir.Bottom:
					cellPosition.y -= m_PickUpDistance;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			// Draw player interaction point.
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(_navMesh.GetCellCenterWorld(cellPosition), 0.1f);

			// Draw the controller interaction range for ground items.
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(transform.position,
				m_ControllerInteractionDistance);
		}
	}
#endif
}