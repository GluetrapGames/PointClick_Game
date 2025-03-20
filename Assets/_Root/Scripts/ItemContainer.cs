using GlueTrap.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GlueTrap
{
public class ItemContainer : MonoBehaviour
{
	private CollideCheck _CollideCheck;
	private GameObject _ContainerInventory;
	private GameManager _GameManager;
	private InputAction _InteractionAction;


	private void Awake()
	{
		_GameManager = Utils.GetGameManager();
		var playerInput = _GameManager.m_Player.GetComponent<PlayerInput>();

		_InteractionAction = playerInput.actions["Break"];
		if (_InteractionAction == null) Debug.LogError("No break action found");

		_CollideCheck = GetComponent<CollideCheck>();
	}

	private void Start()
	{
		// Obtain the Container Inventory object.
		var inventoryObjs = GameObject.FindGameObjectsWithTag("Inventory");
		foreach (GameObject obj in inventoryObjs)
			if (obj.name.Contains("Container"))
				_ContainerInventory = obj.transform.GetChild(0).gameObject;

		// Null Check.
		if (!_ContainerInventory)
			Debug.LogWarning("No Container Inventory found!");
	}

	private void Update()
	{
		if (_InteractionAction.WasPressedThisFrame() &&
		    _CollideCheck.IsCollided)
		{
			_ContainerInventory.SetActive(
				!_ContainerInventory.activeInHierarchy);
		}
	}
}
}