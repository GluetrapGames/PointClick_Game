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
	private bool _isWallItem;
	[SerializeField]
	private GridMovement _player;
	[SerializeField]
	private Tilemap _navMesh;

	private Camera _camera;


	private void Awake()
	{
		_camera = Camera.main;
		_player = GameObject.FindWithTag("Player")
			?.GetComponent<GridMovement>();
		_navMesh = GameObject.FindWithTag("NavMesh").GetComponent<Tilemap>();
	}

	private void Start()
	{
		gameObject.SetActive(true);
		isClicked = false;
		activateVariable = false;
	}

	private void Update()
	{
		if (!Input.GetMouseButtonDown(0))
			return;

		Vector2 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
		RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

		if (hit.collider && hit.collider.gameObject == gameObject)
		{
			isClicked = true;
			HandleItemInteraction();
		}
		else
			isClicked = false;
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (!isClicked)
			return;

		Debug.Log("Item collected");
		activateVariable = true;
		gameObject.SetActive(false);
	}

	private void HandleItemInteraction()
	{
		if (_isWallItem)
			HandleWallFunction();
		else
			HandleGroundFunction();
	}

	private void HandleGroundFunction()
	{
		Vector3Int cellPosition = _navMesh.WorldToCell(transform.position);

		// Move the player to the target tile.
		_player.SetDestination(cellPosition);
		StartCoroutine(_player.MoveAlongPathCoroutine(5f));
	}

	// Handles wall item functionality.
	private void HandleWallFunction()
	{
		Vector3Int cellPosition = _navMesh.WorldToCell(transform.position);

		if (m_Log)
			Debug.Log($"Before Loop: {cellPosition}");

		while (!_navMesh.HasTile(cellPosition) && cellPosition.y > -100)
		{
			cellPosition.y--;
			if (m_Log)
				Debug.Log($"In Loop: {cellPosition}");
		}

		if (!_navMesh.HasTile(cellPosition))
			return;

		if (m_Log)
		{
			Debug.Log($"After Loop: {cellPosition}");
			_navMesh.SetTileFlags(cellPosition,
				TileFlags.None); // Allow colour modification.
			_navMesh.SetColor(cellPosition, Color.green);
		}

		// Move the player to the target tile.
		_player.SetDestination(cellPosition);
		StartCoroutine(_player.MoveAlongPathCoroutine(5f));
	}
}