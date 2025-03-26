using System.Collections.Generic;
using System.Linq;
using EditorAttributes;
using GlueTrap.Utilities;
using UnityEngine;

namespace GlueTrap
{
public class HighlightComposite : MonoBehaviour
{
	[SerializeField, ReadOnly]
	private List<Highlight> _highlights = new();
	private GameManager _GameManager;


	[Button("Highlights")]
	private void Awake()
	{
		_GameManager = Utils.GetGameManager();
		List<Highlight> objs = new();
		Utils.FindChildrenByType<Highlight, Highlight>(transform, objs, c => c);
		// Only affect Highlights that want to be used by the composite.
		foreach (Highlight obj in objs.Where(obj => obj.m_UsedByComposite))
			_highlights.Add(obj);

		// Combine all highlights colliders.
		CombineColliders();
	}

	private void Update()
	{
		// Only allow highlighting when not talking or in menus.
		if (_GameManager.m_CurrentState != States.Moving)
		{
			HideAll();
			return;
		}

		Vector2 mousePos =
			_GameManager.m_Camera.ScreenToWorldPoint(Input.mousePosition);
		var colliderComp = GetComponent<Collider2D>();

		if (colliderComp.OverlapPoint(mousePos))
			ShowAll();
		else
			HideAll();
	}

	private void ShowAll()
	{
		if (_highlights.Count <= 0)
		{
			Debug.LogWarning($"<{this}> Highlights are empty!");
			return;
		}

		foreach (Highlight highlight in _highlights)
			highlight.Show();
	}

	private void HideAll()
	{
		if (_highlights.Count <= 0)
		{
			Debug.LogWarning($"<{this}> Highlights are empty!");
			return;
		}

		foreach (Highlight highlight in _highlights)
			highlight.Hide();
	}

	private void CombineColliders()
	{
		// Retrieve or add a PolygonCollider2D.
		var combinedCollider = GetComponent<PolygonCollider2D>();
		if (!combinedCollider)
			combinedCollider = gameObject.AddComponent<PolygonCollider2D>();

		combinedCollider.isTrigger = true;
		var allPoints = new List<Vector2>();
		// Iterate through each highlight to collect all polygon points.
		foreach (PolygonCollider2D polyCollider in _highlights
			         .Select(highlight =>
				         highlight.GetComponent<PolygonCollider2D>())
			         .Where(polyCollider => polyCollider))
			// Convert local points to world coordinates.
			allPoints.AddRange(polyCollider.points
				.Select(point => polyCollider.transform.TransformPoint(point))
				.Select(dummy => (Vector2)dummy));

		// Exit if no points were found.
		if (allPoints.Count == 0)
			return;

		// Calculate the centroid of all points.
		Vector2 centroid = allPoints.Aggregate(Vector2.zero,
			(current, point) => current + point);
		centroid /= allPoints.Count;


		// Set the collider's offset to the centroid.
		combinedCollider.offset = centroid - (Vector2)transform.position;
		combinedCollider.pathCount = 1;
		// Adjust points to be relative to the centroid.
		combinedCollider.SetPath(0,
			allPoints.Select(point => point - centroid).ToArray());
	}
}
}