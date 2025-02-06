using UnityEngine;
using UnityEngine.Tilemaps;

public class PickUpScript : MonoBehaviour
{
	[Tooltip("Log all major points of interests in the script.")]
	public bool m_Log;
	[Header("Settings")]
	public bool isClicked;
	public bool activateVariable;

	[SerializeField]
	private bool m_IsWallItem;
	[SerializeField]
	private GridMovement m_Player;
	[SerializeField]
	private Tilemap m_Ground;

	private Camera _camera;


	private void Awake()
	{
		_camera = Camera.main;
		m_Player = GameObject.FindWithTag("Player")
			?.GetComponent<GridMovement>();
	}

	private void Start()
	{
		gameObject.SetActive(true);
		isClicked = false;
		activateVariable = false;
	}

	private void Update()
	{
		// When mouse is clicked, Ray cast to check if the game object space was clicked.
		if (!Input.GetMouseButtonDown(0)) return;

		Vector2 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
		RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

		if (hit.collider && hit.collider.gameObject == gameObject)
		{
			isClicked = true;
			HandleWallFunction();
		}
		else
			isClicked = false;
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		// If the game object has been clicked and the player is within the
		// trigger then hide the object and display alert message.
		if (!isClicked) return;

		Debug.Log("Item collected");

		activateVariable = true;
		gameObject.SetActive(false);
	}

	// Handle functionality for wall items.
	private void HandleWallFunction()
	{
		if (!m_IsWallItem) return;

		Vector3Int cellPosition =
			m_Ground.WorldToCell(transform.position);

		if (m_Log) Debug.Log($"Before Loop: {cellPosition}");
		// Continue searching down until a valid tile has been foudn.
		while (!m_Ground.HasTile(cellPosition) && cellPosition.y > -100)
		{
			cellPosition.y--;
			if (m_Log) Debug.Log($"In Loop: {cellPosition}");
		}

		if (!m_Ground.HasTile(cellPosition)) return;

		if (m_Log)
		{
			Debug.Log($"After Loop: {cellPosition}");
			m_Ground.SetTileFlags(cellPosition,
				TileFlags.None); //< Allow colour modification.
			m_Ground.SetColor(cellPosition, Color.green);
		}

		// Move the player to that tile.
		m_Player.MoveToTile(cellPosition);
	}
}