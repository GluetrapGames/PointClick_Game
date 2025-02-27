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
	}
}