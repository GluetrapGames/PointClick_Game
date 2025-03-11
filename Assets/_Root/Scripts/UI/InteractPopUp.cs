using UnityEngine;

namespace GlueTrap
{
public class InteractPopUp : MonoBehaviour
{
	public float interactionRadius = 3f;
	public InteractionPanel interactionUI;

	[SerializeField]
	private bool _Log;

	private CollideCheck _collisionCheck;
	private GameManager _GameManager;


	private void Awake()
	{
		_GameManager = FindFirstObjectByType<GameManager>();
		_collisionCheck = GetComponent<CollideCheck>();
	}

	// Update is called once per frame
	private void FixedUpdate()
	{
		if (!interactionUI) return;

		DrawInteractUI();
	}

	private void DrawInteractUI()
	{
		switch (interactionUI.isDrawn)
		{
			case false when _collisionCheck.IsCollided:
			{
				interactionUI.gameObject.SetActive(true);
				interactionUI.isDrawn = true;
				interactionUI.drawnBy = gameObject;
				if (_Log)
				{
					Debug.Log("Drawing interaction UI, drawn by " +
					          gameObject.name);
				}

				break;
			}
			case false when !_collisionCheck.IsCollided:
				return;
			case true when !_collisionCheck.IsCollided:
			{
				if (interactionUI.drawnBy != gameObject) return;
				interactionUI.isDrawn = false;
				interactionUI.drawnBy = null;
				interactionUI.gameObject.SetActive(false);
				break;
			}
		}
	}

	private void Interact()
	{
		if (_Log)
			Debug.Log($"Interacted with {name}");
	}
}
}