using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GlueTrap.Utilities
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

	/// <summary>
	///     Resizes the collision bounds of a desired object.
	///     Only supports BoxCollider2D!
	/// </summary>
	/// <param name="sprite">Sprite to resize to.</param>
	/// <param name="boxCollider">Collider to resize.</param>
	/// <returns></returns>
	public static bool RecalculateCollisionBounds(Sprite sprite,
		ref BoxCollider2D boxCollider)
	{
		if (!sprite || !boxCollider) return false;

		// Calculate X & Y bounds based on sprite.
		var boundsSize = new Vector2(
			sprite.bounds.size.x - (sprite.border.x + sprite.border.z) /
			sprite.pixelsPerUnit,
			sprite.bounds.size.y - (sprite.border.w + sprite.border.y) /
			sprite.pixelsPerUnit
		);
		boxCollider.size = boundsSize;
		boxCollider.offset = Vector2.zero;
		return true;
	}

	/// <summary>
	///     Returns whether a desired animation has finished playing or not.
	/// </summary>
	/// <param name="animator"></param>
	/// <param name="stateName"></param>
	/// <param name="layer"></param>
	/// <returns></returns>
	public static bool HasAnimationFinished(Animator animator, string stateName,
		int layer = 0)
	{
		if (animator.IsInTransition(layer))
			return false;

		AnimatorStateInfo stateInfo =
			animator.GetCurrentAnimatorStateInfo(layer);

		// Check for animation completion, taking into account looping animations
		var isFinished =
			(stateInfo.IsName(stateName) && stateInfo.normalizedTime >= 1.0f) ||
			stateInfo is { loop: true, normalizedTime: >= 1.0f };

		return isFinished;
	}
}
}