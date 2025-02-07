using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class AStar
{
	public static List<Vector3Int> FindPath(Vector3Int start, Vector3Int goal,
		Tilemap groundMap, Tilemap obstacleMap)
	{
		// A* algorithm to find the path.
		var openList = new List<Vector3Int>();
		var closedList = new HashSet<Vector3Int>();
		var cameFrom = new Dictionary<Vector3Int, Vector3Int>();
		var gScore =
			new Dictionary<Vector3Int, float>(); //< Cost from start to current.
		var fScore =
			new Dictionary<Vector3Int, float>(); //< Estimated cost to goal.

		openList.Add(start);
		gScore[start] = 0;
		fScore[start] = Heuristic(start, goal);

		while (openList.Count > 0)
		{
			// Get the node with the lowest fScore.
			Vector3Int current = GetLowestFScoreNode(openList, fScore);
			if (current == goal)
				return ReconstructPath(cameFrom, current);

			openList.Remove(current);
			closedList.Add(current);

			foreach (Vector3Int neighbor in GetNeighbors(current, groundMap))
			{
				if (closedList.Contains(neighbor) ||
				    obstacleMap.HasTile(neighbor)) continue;

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

	public static Vector3Int GetLowestFScoreNode(List<Vector3Int> openList,
		Dictionary<Vector3Int, float> fScore)
	{
		Vector3Int lowest = openList[0];
		foreach (Vector3Int node in openList.Where(node =>
			         fScore[node] < fScore[lowest])) lowest = node;

		return lowest;
	}

	public static List<Vector3Int> ReconstructPath(Dictionary<Vector3Int,
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

	public static List<Vector3Int> GetNeighbors(Vector3Int tilePosition,
		Tilemap groundMap)
	{
		// Check for four possible neighbors (up, down, left, right).
		Vector3Int[] directions =
		{
			Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right
		};

		return directions.Select(dir => tilePosition + dir)
			.Where(groundMap.HasTile)
			.ToList();
	}

	public static float Heuristic(Vector3Int a, Vector3Int b)
	{
		return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
	}
}