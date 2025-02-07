using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridMovement : MonoBehaviour
{
	public int m_CurrentPathIndex;
	public bool m_IsMoving;
	public List<Vector3Int> m_Path = new();
	public Grid m_Grid;
	public Tilemap m_NavMesh;
	public Tilemap m_ObstacleTilemap;
	public Vector3Int? m_PreviousTilePosition;


	private void Awake()
	{
		m_Grid = FindFirstObjectByType<Grid>();

		// Loop through the children of the Grid object to assign the tilemaps.
		foreach (Transform child in m_Grid.transform)
			if (child.CompareTag("NavMesh"))
				m_NavMesh = child.GetComponent<Tilemap>();
			else if (!m_ObstacleTilemap)
				m_ObstacleTilemap = child.GetComponent<Tilemap>();

		// Ensure both tilemaps were assigned correctly.
		if (!m_NavMesh) Debug.LogError("NavMesh Tilemap not assigned.");
		if (!m_ObstacleTilemap)
			Debug.LogError("Obstacle Tilemap not assigned.");
	}

#if UNITY_EDITOR
	private void Reset()
	{
		Awake();
	}
#endif

	public void TeleportToTile(Vector3Int tilePosition)
	{
		// Set the object's initial position to the starting tile.
		Vector3 startPos = m_Grid.GetCellCenterWorld(tilePosition);
		transform.position = startPos;
	}

	// Sets the destination tile and calculates an A* path.
	public void SetDestination(Vector3Int tilePosition)
	{
		if (!m_NavMesh.HasTile(tilePosition) ||
		    m_ObstacleTilemap.HasTile(tilePosition))
			return;

		m_Path = AStar.FindPath(m_Grid.WorldToCell(transform.position),
			tilePosition, m_NavMesh, m_ObstacleTilemap);
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