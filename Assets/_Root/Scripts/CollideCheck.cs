using UnityEngine;

public class CollideCheck : MonoBehaviour
{
	[SerializeField]
	private bool _Log;

	public bool IsCollided { get; private set; }

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (_Log)
			Debug.Log(transform.name + " collided with " + other.name);
		if (other.gameObject.CompareTag("Player")) IsCollided = true;
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