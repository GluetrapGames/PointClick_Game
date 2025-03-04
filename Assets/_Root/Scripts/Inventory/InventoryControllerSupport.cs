using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InventoryControllerSupport : MonoBehaviour
{
	public GameObject inventoryUI;
	public GameObject firstSelectedSlot;

	public PlayerInput playerInput;

	public InvButtonSpriteSwap spriteSwap;

	[SerializeField]
	private bool _Log;
	private GameManager _GameManager;
	private InputAction _inventoryAction;
	private bool _isOpen;


	private void Awake()
	{
		_GameManager = FindFirstObjectByType<GameManager>();
	}

	private void Start()
	{
		playerInput = _GameManager.m_Player.GetComponent<PlayerInput>();
		_inventoryAction = playerInput.actions["Inventory"];
		if (_inventoryAction == null) Debug.LogError("No menu action found");

		foreach (Transform slot in _GameManager.m_InventoryManager
			         .m_InventorySlots)
		{
			var inventorySlot = slot.GetComponent<InventorySlot>();
			// Check if the button is assigned
			if (inventorySlot.button == null || slot == null)
			{
				Debug.LogError($"Button is null in {slot}: {slot.name}");
				continue; // Skip this slot if the button is missing
			}

			// Add an onClick listener to the Button inside each InventorySlot
			inventorySlot.button.onClick.AddListener(() =>
				OnSlotPressed(inventorySlot));
		}
	}

	private void Update()
	{
		if (_inventoryAction.WasPressedThisFrame())
		{
			if (_Log) Debug.Log("Opening Inv/Closing Inv");
			OpenInv();
		}
	}

	public void OpenInv()
	{
		if (!_isOpen)
		{
			_GameManager.ChangeGameState(States.InMenus);
			if (_Log) Debug.Log("Opening Inv");
			inventoryUI.SetActive(true);
			if (Gamepad.current != null)
				EventSystem.current.SetSelectedGameObject(firstSelectedSlot);
			_isOpen = true;
			spriteSwap.OnButtonClick();
			InvTimeMethod();
		}
		else
		{
			_GameManager.ChangeGameState(States.Moving);
			if (_Log) Debug.Log("Closing Inv");
			inventoryUI.SetActive(false);
			EventSystem.current.SetSelectedGameObject(null);
			_isOpen = false;
			spriteSwap.OnButtonClick();
			InvTimeMethod();
		}
	}

	private void InvTimeMethod()
	{
		Time.timeScale = _isOpen ? 0 : 1;
	}

	private void OnSlotPressed(InventorySlot slot)
	{
		InventorySlot pressedSlot = slot;
		var slotComp = _GameManager.m_InventoryManager.m_HeldItemSlot
			.GetComponent<InventorySlot>();
		HeldItemSlot heldItemSlot =
			_GameManager.m_InventoryManager.m_HeldItemSlot;

		if (_GameManager.m_InventoryManager.m_HeldItemSlot.transform
			    .childCount == 0)
		{
			if (slot.item != null)
			{
				slot.item.transform.SetParent(_GameManager.m_InventoryManager
					.m_HeldItemSlot.transform);
				heldItemSlot.playerHeldItem = slot.item.itemType;

				if (_Log)
				{
					Debug.Log(
						$"Transferred {slot.item.name} to the HeldItemSlot.");
				}

				slot.item = null;
			}
			else if (_Log)
				Debug.Log($"No item in slot {slot.item.name} to transfer.");
		}
		else
		{
			if (slot.item != null)
			{
				if (_Log) Debug.Log("Item in held slot");
				InventorySlot previousSlot = slot;

				Transform currentHeldItemTransform =
					_GameManager.m_InventoryManager.m_HeldItemSlot.transform
						.GetChild(0);
				var currentHeldItem =
					currentHeldItemTransform.GetComponent<InventoryItem>();

				currentHeldItem.transform.SetParent(slot.transform);
				slot.item.transform.SetParent(_GameManager.m_InventoryManager
					.m_HeldItemSlot.transform);

				slot.item = currentHeldItem;

				Transform newHeldItemTransform =
					_GameManager.m_InventoryManager.m_HeldItemSlot.transform
						.GetChild(0);
				var newHeldItem =
					newHeldItemTransform.GetComponent<InventoryItem>();

				slotComp.item = newHeldItem;
				heldItemSlot.playerHeldItem = newHeldItem.itemType;

				if (!_Log) return;
				Debug.Log(
					$"Held item new parent: {currentHeldItem.transform.parent.name}");
				Debug.Log(
					$"Transferred {slot.item.name} to the HeldItemSlot.");
			}
			else
			{
				if (_Log)
				{
					Debug.Log($"No item in slot {slot.name} to transfer, " +
					          "transferring held item to empty slot.");
				}

				Transform currentHeldItemTransform =
					_GameManager.m_InventoryManager.m_HeldItemSlot.transform
						.GetChild(0);
				var currentHeldItem =
					currentHeldItemTransform.GetComponent<InventoryItem>();

				currentHeldItem.transform.SetParent(slot.transform);
				slot.item = currentHeldItem;

				slotComp.item = null;
				heldItemSlot.playerHeldItem = null;
			}
		}
	}
}