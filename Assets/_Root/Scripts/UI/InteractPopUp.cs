using UnityEngine;

namespace GlueTrap
{
public class InteractPopUp : MonoBehaviour
{
	public float interactionRadius = 3f;
	public GameObject interactionUI;

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
		if (!interactionUI.GetComponent<InteractionPanel>().isDrawn &&
		    _collisionCheck.IsCollided)
		{
			interactionUI.SetActive(true);
			interactionUI.GetComponent<InteractionPanel>().isDrawn = true;
			interactionUI.GetComponent<InteractionPanel>().drawnBy = gameObject;
			if (_Log)
			{
				Debug.Log("Drawing interaction UI, drawn by " +
				          gameObject.name);
			}
		}
		else if (!interactionUI.GetComponent<InteractionPanel>().isDrawn &&
		         !_collisionCheck.IsCollided)
			return;
		else if (interactionUI.GetComponent<InteractionPanel>().isDrawn &&
		         !_collisionCheck.IsCollided)
		{
			if (interactionUI.GetComponent<InteractionPanel>().drawnBy ==
			    gameObject)
			{
				interactionUI.GetComponent<InteractionPanel>().isDrawn = false;
				interactionUI.GetComponent<InteractionPanel>().drawnBy = null;
				interactionUI.SetActive(false);
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