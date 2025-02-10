using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridMovement : MonoBehaviour
{
	public int m_CurrentPathIndex;
	public bool m_IsMoving;
	public List<Vector3Int> m_Path = new();
	public Grid m_Grid;
	public Tilemap m_NavMesh;

	[SerializeField]
	private List<string> _obstacleTags = new();
	[SerializeField]
	private List<Transform> _obstacles = new();
	[SerializeField]
	private bool _debug;
	[SerializeField]
	[Range(0f, 1f)]
	private float _obstacleBoundsOffset = 0.15f;
	public readonly List<Vector3Int> m_ObstaclesPositions = new();

	private GridDebug _gridDebug;
	private Dictionary<Transform, bool> _obstacleStates = new();
	private List<string> _oldObstacleTags;
	public Vector3Int? m_PreviousTilePosition;


	private void Awake()
	{
		m_Grid = FindFirstObjectByType<Grid>();
		// If grid doesn't have debug script, add it.
		if (!m_Grid.TryGetComponent(out _gridDebug))
			_gridDebug = m_Grid.gameObject.AddComponent<GridDebug>();

		// Loop through the children of the Grid object to assign the Navmesh.
		foreach (Transform child in m_Grid.transform)
			if (child.CompareTag("NavMesh"))
				m_NavMesh = child.GetComponent<Tilemap>();

		// Ensure both tilemaps were assigned correctly.
		if (!m_NavMesh) Debug.LogError("NavMesh Tilemap not assigned.");

		// Store current tag list.
		_oldObstacleTags = _obstacleTags;

		// Ignore if no tags provided.
		if (_obstacleTags.Count == 0) return;
		AddObstacles(_obstacleTags);
		CalculateObstacleTiles();
	}

#if UNITY_EDITOR
	private void Reset()
	{
		Awake();
	}

	/*private void OnValidate()
	{
		if (_debug)
		{
			_gridDebug.m_ToggleDebug = true;
			foreach (Vector3Int cellPos in _obstaclesPositions)
			{
				m_NavMesh.SetTileFlags(cellPos, TileFlags.None);
				m_NavMesh.SetColor(cellPos, Color.red);
			}
		}
		else
		{
			_gridDebug.m_ToggleDebug = false;
			foreach (Vector3Int cellPos in _obstaclesPositions)
				m_NavMesh.SetColor(cellPos, Color.cyan);
		}
	}*/

#endif

	private void Update()
	{
		if (ShouldUpdateObstacle())
			UpdateObstacles();
	}

	private void UpdateObstacles()
	{
		var addedTags = _obstacleTags.Except(_oldObstacleTags).ToList();
		var removedTags = _oldObstacleTags.Except(_obstacleTags).ToList();

		if (_debug)
		{
			Debug.Log(
				$"Adding {addedTags.Count} obstacles: {string.Join(", ", addedTags)}.");
			Debug.Log(
				$"Removing {removedTags.Count} obstacles: {string.Join(", ", removedTags)}.");
		}

		AddObstacles(addedTags);
		RemoveObstacles(removedTags);
		// Remove only destroyed obstacles.
		_obstacles.RemoveAll(obstacle => !obstacle);
		_obstacleStates =
			_obstacles.ToDictionary(o => o,
				o => o.gameObject.activeInHierarchy);

		// Recalculate the obstacle tile positions.
		CalculateObstacleTiles();

		_oldObstacleTags = new List<string>(_obstacleTags);
	}

	private bool ShouldUpdateObstacle()
	{
		if (!_oldObstacleTags.SequenceEqual(_obstacleTags)) return true;

		// Check if any obstacle's active state has changed.
		var anyStateChanged = false;
		foreach (Transform obstacle in _obstacles)
		{
			if (!obstacle) continue; //< Continue if NULL.

			var isActive = obstacle.gameObject.activeInHierarchy;
			// Continue if no change.
			if (_obstacleStates.TryGetValue(obstacle, out var lastState) &&
			    lastState == isActive) continue;

			// Update object state.
			_obstacleStates[obstacle] = isActive;
			anyStateChanged = true;
		}

		return anyStateChanged;
	}

	// Add obstacles to the list.
	private void AddObstacles(List<string> tags)
	{
		var newObstacles = tags
			.SelectMany(GameObject.FindGameObjectsWithTag)
			.Select(obj => obj.transform)
			.Where(obstacle => !_obstacles.Contains(obstacle));

		_obstacles.AddRange(newObstacles);
	}

	// Remove obstacles from the list.
	private void RemoveObstacles(List<string> tags)
	{
		// Remove obstacles that belong to the removed tags.
		var removedTags = tags.ToHashSet();
		_obstacles.RemoveAll(obstacle => removedTags.Contains(obstacle.tag));
	}

	// Calculate the amount of tile that count as obstacles, based on the
	// size of the obstacle.
	private void CalculateObstacleTiles()
	{
		// For each obstacle, add every cell it occupies.
		m_ObstaclesPositions.Clear();
		foreach (Transform obstacle in _obstacles.Where(obstacle =>
			         obstacle && obstacle.gameObject.activeInHierarchy))
		{
			if (!obstacle.TryGetComponent(out Renderer obstacleRenderer))
			{
				// Fallback: use the obstacle's transform position.
				Vector3Int cellPos = m_Grid.WorldToCell(obstacle.position);
				if (m_NavMesh.HasTile(cellPos) &&
				    !m_ObstaclesPositions.Contains(cellPos))
					m_ObstaclesPositions.Add(cellPos);
				continue;
			}

			Bounds bounds = obstacleRenderer.bounds;

			// Create an offset for the bounding box.
			var offset = new Vector3(
				_obstacleBoundsOffset, _obstacleBoundsOffset, 0f);
			// Convert bounds to grid positions.
			Vector3Int minCell = m_Grid.WorldToCell(bounds.min + offset);
			Vector3Int maxCell = m_Grid.WorldToCell(bounds.max - offset);

			// Loop over every cell in the bounding rectangle.
			for (var x = minCell.x; x <= maxCell.x; x++)
			for (var y = minCell.y; y <= maxCell.y; y++)
			{
				var cell = new Vector3Int(x, y, 0);
				// Add the cell if it's a valid NavMesh tile.
				if (m_NavMesh.HasTile(cell) &&
				    !m_ObstaclesPositions.Contains(cell))
					m_ObstaclesPositions.Add(cell);
			}
		}
	}

	public void TeleportToTile(Vector3Int tilePosition)
	{
		// Set the object's initial position to the starting tile.
		Vector3 startPos = m_Grid.GetCellCenterWorld(tilePosition);
		transform.position = startPos;
	}

	// Sets the destination tile and calculates an A* path.
	public void SetDestination(Vector3Int tilePosition)
	{
		if (!m_NavMesh.HasTile(tilePosition)) return;

		m_Path = AStar.FindPath(m_Grid.WorldToCell(transform.position),
			tilePosition, m_NavMesh, m_ObstaclesPositions);
		if (m_Path is { Count: > 0 })
		{
			Vector3Int currentCell = m_Grid.WorldToCell(transform.position);
			if (m_Path[0] == currentCell)
				m_Path.RemoveAt(0);
		}

		m_CurrentPathIndex = 0;
		m_IsMoving = true;
	}

	// Moves the object along the precomputed path.
	public void MoveToTile(float speed = 5f)
	{
		if (!m_IsMoving || m_Path == null || m_CurrentPathIndex >= m_Path.Count)
		{
			m_IsMoving = false;
			return;
		}

		Vector3Int targetTile = m_Path[m_CurrentPathIndex];
		Vector3 targetPosition = m_Grid.GetCellCenterWorld(targetTile);
		var step = speed * Time.deltaTime;
		transform.position =
			Vector3.MoveTowards(transform.position, targetPosition, step);

		// Use a threshold to account for floating point imprecision.
		if (!(Vector3.Distance(transform.position, targetPosition) < 0.01f))
			return;

		transform.position = targetPosition;
		m_CurrentPathIndex++;
		if (m_CurrentPathIndex >= m_Path.Count)
			m_IsMoving = false;
	}

	// Coroutine that calls MoveToTile until the path is complete.
	public IEnumerator MoveAlongPathCoroutine(float speed)
	{
		while (m_IsMoving)
		{
			MoveToTile(speed);
			yield return null;
		}
	}
}