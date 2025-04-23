using System;
using EditorAttributes;
using UnityEngine;

namespace GlueTrap
{
public class CollideCheck : MonoBehaviour
{
	[SerializeField]
	private bool _Log;
	[SerializeField, ReadOnly]
	public bool IsCollided;

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (_Log)
			Debug.Log(transform.name + " collided with " + other.name);
		if (other.gameObject.CompareTag("Player"))
		{
			IsCollided = true;
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (_Log)
			Debug.Log("Collision Reset");
		if (other.gameObject.CompareTag("Player")) ResetCollision();
	}

	public void ResetCollision()
	{
		IsCollided = false;
	}
}
}