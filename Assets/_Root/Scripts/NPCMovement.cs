using System.Collections;
using System.Collections.Generic;
using EditorAttributes;
using GlueTrap.Utilities;
using UnityEngine;

namespace GlueTrap
{
[RequireComponent(typeof(GridMovement))]
public class NPCMovement : MonoBehaviour
{
	public bool m_IsLooping;
	public float m_MovementSpeed = 3.0f;
	[Tooltip("The wait time between each destination point. \nIn seconds.")]
	public float m_WaitTime = 2.0f;
	[OnValueChanged(nameof(UpdateCellPath))]
	public List<Vector2> m_Path = new();

	[SerializeField, Tooltip("Enables viewing of transformed path nodes."),
	 PropertyOrder(-1)]
	private bool _Debug;
	[SerializeField, ReadOnly]
	private List<Vector3Int> _CellPath = new();

	private GameManager _GameManager;
	private GridMovement _GridMovement;


	private void Awake()
	{
		_GameManager = Utils.GetGameManager();
		_GridMovement = GetComponent<GridMovement>();
	}

	private void Start()
	{
		StartCoroutine(FollowPath());
	}

	private IEnumerator FollowPath()
	{
		do
		{
			// Create a copy to avoid modifying the list during enumeration.
			List<Vector3Int> pathToFollow = new(_CellPath);

			foreach (Vector3Int cellPoint in pathToFollow)
			{
				_GridMovement.SetDestination(cellPoint);

				while (_GridMovement.m_IsMoving)
				{
					_GridMovement.MoveToTile(m_MovementSpeed);
					yield return null;
				}

				yield return new WaitForSeconds(m_WaitTime);
			}

			if (m_IsLooping)
			{
				m_Path.Reverse();
				yield return new WaitForSeconds(m_WaitTime * 0.5f);
			}
		} while (m_IsLooping);
	}


	// Convert the current path points into their cell values.
	private void UpdateCellPath()
	{
		_CellPath.Clear();

		// If in Play mode access the grid through the Game Manager.
		if (Application.isPlaying)
		{
			foreach (Vector2 point in m_Path)
				_CellPath.Add(_GameManager.m_Grid.WorldToCell(point));
			return;
		}

		// If in the Editor access the grid directly.
#if UNITY_EDITOR
		var grid = FindFirstObjectByType<Grid>();
		if (!grid)
		{
			Debug.LogWarning($"{name} can't find a grid!");
			return;
		}

		foreach (Vector2 point in m_Path)
			_CellPath.Add(grid.WorldToCell(point));
#endif
	}

#if UNITY_EDITOR
	private void Reset()
	{
		Awake();
	}

	private void OnDrawGizmosSelected()
	{
		// Display the NPC's target positions.
		Gizmos.color = Color.yellow;
		foreach (Vector2 point in m_Path)
			Gizmos.DrawSphere(point, 0.2f);

		if (!_Debug) return;
		// Display the NPC's transformed target positions.
		Gizmos.color = Color.red;
		foreach (Vector3Int cell in _CellPath)
			Gizmos.DrawSphere(cell, 0.2f);
	}
#endif
}
}