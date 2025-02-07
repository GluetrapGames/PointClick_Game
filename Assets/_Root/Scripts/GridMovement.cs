using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridMovement : MonoBehaviour
{
	public Vector3Int m_StartingGridPosition;
	public int m_CurrentPathIndex;
	public bool m_IsMoving;
	public List<Vector3Int> m_Path;
	public Grid m_Grid;
	public Tilemap m_Tilemap;
	public Tilemap m_ObstacleTilemap;

	[SerializeField]
	private bool m_Debug;

	private Vector3 _targetPosition;
	public Vector3Int? m_PreviousTilePosition;


	private void Start()
	{
		// Set the object's initial position to a starting tile.
		_targetPosition = m_Grid.GetCellCenterWorld(m_StartingGridPosition);
		transform.position = _targetPosition;
	}

	// Move the object towards the next tile on the path.
	public void MoveToTile(float speed = 5f)
	{
		if (m_IsMoving && m_Path != null && m_CurrentPathIndex < m_Path.Count)
			MoveAlongPath(speed);
	}

	// Set the destination tile and calculate path.
	public void SetDestination(Vector3Int tilePosition)
	{
		if (!m_Tilemap.HasTile(tilePosition) ||
		    m_ObstacleTilemap.HasTile(tilePosition)) return;

		m_Path = AStar.FindPath(m_Grid.WorldToCell(transform.position),
			tilePosition, m_Tilemap, m_ObstacleTilemap);
		m_CurrentPathIndex = 0;
		m_IsMoving = true;
	}

	public void MoveAlongPath(float speed)
	{
		if (m_Path == null || m_Path.Count == 0)
		{
			m_IsMoving = false;
			return;
		}

		if (m_CurrentPathIndex < m_Path.Count)
		{
			Vector3Int targetTile = m_Path[m_CurrentPathIndex];
			Vector3 targetPosition = m_Grid.GetCellCenterWorld(targetTile);
			var step = speed * Time.deltaTime;

			transform.position =
				Vector3.MoveTowards(transform.position, targetPosition, step);

			if (transform.position == targetPosition) m_CurrentPathIndex++;
		}
		else
			m_IsMoving = false;
	}
}