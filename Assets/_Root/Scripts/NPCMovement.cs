using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(GridMovement))]
public class NPCMovement : MonoBehaviour
{
	public float m_MovementSpeed = 3.0f;
	[Tooltip("The wait time between each destination point. \nIn seconds.")]
	public float m_WaitTime = 2.0f;
	public List<Vector2> m_Path = new();

	[SerializeField, Tooltip("Enables viewing of transformed path nodes.")]
	private bool m_Debug;
	[SerializeField]
	private List<Vector3Int> _cellPath = new();

	private GameManager _GameManager;
	private GridMovement _gridMovement;


	private void Awake()
	{
		_GameManager = FindFirstObjectByType<GameManager>();
		_gridMovement = GetComponent<GridMovement>();
	}

	private void Reset()
	{
		Awake();
	}

	private void Start()
	{
		StartCoroutine(FollowPath());
	}

	private void OnDrawGizmosSelected()
	{
		// Display the NPC's target positions.
		Gizmos.color = Color.yellow;
		foreach (Vector2 point in m_Path)
			Gizmos.DrawSphere(point, 0.2f);

		if (!m_Debug) return;
		// Display the NPC's transformed target positions.
		Gizmos.color = Color.red;
		foreach (Vector3Int cell in _cellPath)
			Gizmos.DrawSphere(cell, 0.2f);
	}

	private IEnumerator FollowPath()
	{
		while (true)
		{
			// Iterate through each destination in the current path.
			foreach (Vector3Int gridDestination in m_Path.Select(point =>
				         _GameManager.m_Grid.WorldToCell(point)))
			{
				_gridMovement.SetDestination(gridDestination);

				// Continue moving until GridMovement finishes the current path.
				while (_gridMovement.m_IsMoving)
				{
					_gridMovement.MoveToTile(m_MovementSpeed);
					yield return null;
				}

				// Wait at the destination before moving on.
				yield return new WaitForSeconds(m_WaitTime);
			}

			// After reaching the end of the path, reverse the list.
			m_Path.Reverse();
			yield return new WaitForSeconds(m_WaitTime / 2f);
		}
	}
}