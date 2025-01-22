using UnityEngine;
using UnityEngine.Tilemaps;

public class GridMovement : MonoBehaviour
{
	public Vector3Int m_StartingGridPosition;

	[SerializeField]
	private Grid m_Grid;
	[SerializeField]
	private Tilemap m_Tilemap;
	[SerializeField]
	private Color m_HighlightColor = new(0.788f, 0.788f, 0.788f);
	[SerializeField]
	private float m_MoveSpeed = 2f;
	private Camera _camera;
	private bool _isMoving;
	private Vector3Int? _previousTilePosition;
	private Vector3 _targetPosition;

	private void Start()
	{
		_camera = Camera.main;
		// Set the player's initial position to the center tile.
		_targetPosition = m_Grid.GetCellCenterWorld(m_StartingGridPosition);
		transform.position = _targetPosition;
	}

	private void Update()
	{
		var mouseWorldPosition = _camera.ScreenToWorldPoint(Input.mousePosition);

		// Convert the world position to a grid position.
		var tilePosition = m_Grid.WorldToCell(mouseWorldPosition);

		// If the tile position is different, update the highlight.
		if (_previousTilePosition.HasValue && _previousTilePosition.Value != tilePosition)
			ResetTileColor(_previousTilePosition.Value);

		// Highlight the current tile if it exists in the Tilemap.
		if (m_Tilemap.HasTile(tilePosition))
			HighlightTile(tilePosition);

		// Start moving towards the tile when the left mouse button is clicked.
		if (Input.GetMouseButtonDown(0))
			if (m_Tilemap.HasTile(tilePosition))
			{
				_targetPosition = m_Grid.GetCellCenterWorld(tilePosition);
				_isMoving = true;
			}

		// Move the player towards the target position.
		if (_isMoving) MovePlayerTowardsTarget();

		// Store the current tile position.
		_previousTilePosition = tilePosition;
	}

	private void HighlightTile(Vector3Int tilePosition)
	{
		if (!m_Tilemap.HasTile(tilePosition)) return;

		m_Tilemap.SetTileFlags(tilePosition, TileFlags.None); //< Allow colour modification.
		m_Tilemap.SetColor(tilePosition, m_HighlightColor);
	}

	private void ResetTileColor(Vector3Int tilePosition)
	{
		if (!m_Tilemap.HasTile(tilePosition)) return;

		m_Tilemap.SetTileFlags(tilePosition, TileFlags.None); //< Allow colour modification.
		m_Tilemap.SetColor(tilePosition, Color.white);
	}

	private void MovePlayerTowardsTarget()
	{
		// Move the player towards the target position.
		var step = m_MoveSpeed * Time.deltaTime;
		transform.position = Vector3.MoveTowards(transform.position, _targetPosition, step);

		// If the player has reached the target position, stop moving.
		if (transform.position == _targetPosition)
			_isMoving = false;
	}
}