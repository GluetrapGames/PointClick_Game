using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GlueTrap.Utilities
{
public static class Utils
{
	/// <summary>
	///     Recursively searches for child components of type <typeparamref name="U" />
	///     within a given parent transform,
	///     applies a selector function to convert them to type
	///     <typeparamref name="T" />, and adds them to the result list.
	/// </summary>
	/// <typeparam name="U">The component type to search for.</typeparam>
	/// <typeparam name="T">The type of data to store in the result list.</typeparam>
	/// <param name="parent">The root transform to start searching from.</param>
	/// <param name="result">A list to store the selected results.</param>
	/// <param name="selector">
	///     A function that converts found components of type
	///     <typeparamref name="U" /> into type <typeparamref name="T" />.
	/// </param>
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
	/// <param name="tag">The tag to look by.</param>
	/// <param name="name">The name of the spawner to filter by.</param>
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
	/// <param name="name">The name of the spawner to filter by.</param>
	/// <returns></returns>
	public static Transform FindSpawner(string name)
	{
		return FindSpawner("Spawner", name);
	}

	/// <summary>
	///     Obtain the game's Game Manager object.
	/// </summary>
	/// <returns></returns>
	public static GameManager GetGameManager()
	{
		// Obtain Game Manager.
		var objs = GameObject.FindGameObjectsWithTag("Manager");
		return objs.Select(obj => obj.GetComponent<GameManager>())
			.FirstOrDefault(component => component);
	}
}
}