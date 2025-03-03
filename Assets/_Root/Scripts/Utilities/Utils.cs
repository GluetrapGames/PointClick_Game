using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Root.Scripts.Utilities
{
	public static class Utils
	{
		public static void FindChildrenByType<U, T>(Transform parent,
			List<T> result, Func<U, T> selector) where U : Component
		{
			foreach (Transform child in parent)
			{
				var component = child.GetComponent<U>();
				if (component != null) result.Add(selector(component));

				// Recursively search in children.
				FindChildrenByType(child, result, selector);
			}
		}

		/// <summary>
		///     Return the Transform the desired spawner object.
		/// </summary>
		/// <param name="tag">The tag to look by</param>
		/// <param name="name">The name of the spawner to filter by</param>
		/// <returns></returns>
		public static Transform FindSpawner(string tag, string name)
		{
			var spawners = GameObject.FindGameObjectsWithTag(tag);
			foreach (GameObject spawner in spawners)
				if (spawner.name == name)
					return spawner.transform;
			Debug.LogWarning(
				$"Cannot find spawner: \"{name}\" tagged <{tag}>!");
			return null;
		}

		/// <summary>
		///     Return the Transform the desired spawner object.
		///     Default search tag is "Spawner".
		/// </summary>
		/// <param name="name">The name of the spawner to filter by</param>
		/// <returns></returns>
		public static Transform FindSpawner(string name)
		{
			return FindSpawner("Spawner", name);
		}
	}
}