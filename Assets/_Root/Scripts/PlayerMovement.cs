using System.Collections.Generic;
using System.Linq;
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
    private Tilemap m_ObsticalTilemap;
    [SerializeField]
	private Color m_HighlightColor = new(0.788f, 0.788f, 0.788f);
	[SerializeField]
	private float m_MoveSpeed = 2f;
	private Camera _camera;
	private int _currentPathIndex;
	private bool _isMoving;

	private List<Vector3Int> _path;
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
		if (m_Tilemap.HasTile(tilePosition) && !m_ObsticalTilemap.HasTile(tilePosition))
			HighlightTile(tilePosition);

		// Start pathfinding when the left mouse button is clicked.
		if (Input.GetMouseButtonDown(0))
			if (m_Tilemap.HasTile(tilePosition) && !m_ObsticalTilemap.HasTile(tilePosition))
			{
				_path = FindPath(m_Grid.WorldToCell(transform.position), tilePosition);
				_currentPathIndex = 0;
				_isMoving = true;
			}

		// Move the player towards the next tile on the path.
		if (_isMoving && _path != null && _currentPathIndex < _path.Count)
			MovePlayerAlongPath();

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

	private List<Vector3Int> FindPath(Vector3Int start, Vector3Int goal)
	{
		// A* algorithm to find the path.
		var openList = new List<Vector3Int>();
		var closedList = new HashSet<Vector3Int>();
		var cameFrom = new Dictionary<Vector3Int, Vector3Int>();
		var gScore = new Dictionary<Vector3Int, float>(); //< Cost from start to current.
		var fScore = new Dictionary<Vector3Int, float>(); //< Estimated cost to goal.

		openList.Add(start);
		gScore[start] = 0;
		fScore[start] = Heuristic(start, goal);

		while (openList.Count > 0)
		{
			// Get the node with the lowest fScore.
			var current = GetLowestFScoreNode(openList, fScore);
			if (current == goal)
				return ReconstructPath(cameFrom, current);

			openList.Remove(current);
			closedList.Add(current);

			foreach (var neighbor in GetNeighbors(current))
			{
				if (closedList.Contains(neighbor) || 
					m_ObsticalTilemap.HasTile(neighbor)) continue;

				var tentativeGScore = gScore[current] + 1;

				if (!openList.Contains(neighbor))
					openList.Add(neighbor);
				else if (tentativeGScore >= gScore[neighbor])
					continue;

				cameFrom[neighbor] = current;
				gScore[neighbor] = tentativeGScore;
				fScore[neighbor] = gScore[neighbor] + Heuristic(neighbor, goal);
			}
		}

		return null;
	}

	private static Vector3Int GetLowestFScoreNode(List<Vector3Int> openList,
		Dictionary<Vector3Int, float> fScore)
	{
		var lowest = openList[0];
		foreach (var node in openList.Where(node =>
			         fScore[node] < fScore[lowest])) lowest = node;

		return lowest;
	}

	private static List<Vector3Int> ReconstructPath(Dictionary<Vector3Int,
		Vector3Int> cameFrom, Vector3Int current)
	{
		var path = new List<Vector3Int> { current };
		while (cameFrom.ContainsKey(current))
		{
			current = cameFrom[current];
			path.Insert(0, current);
		}

		return path;
	}

	private List<Vector3Int> GetNeighbors(Vector3Int tilePosition)
	{
		// Check for four possible neighbors (up, down, left, right).
		Vector3Int[] directions =
		{
			Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right
		};

		return directions.Select(dir => tilePosition + dir)
			.Where(neighbor => m_Tilemap.HasTile(neighbor)).ToList();
	}

	private static float Heuristic(Vector3Int a, Vector3Int b)
	{
		return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
	}

	private void MovePlayerAlongPath()
	{
		if (_currentPathIndex < _path.Count)
		{
			var targetTile = _path[_currentPathIndex];
			var targetPosition = m_Grid.GetCellCenterWorld(targetTile);
			var step = m_MoveSpeed * Time.deltaTime;

			transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

			if (transform.position == targetPosition) _currentPathIndex++;
		}
		else
		{
			_isMoving = false;
		}
	}
}