using System.Collections;
using System.Collections.Generic;
using PixelCrushers;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

public class InventoryControllerSupport : MonoBehaviour
{

    public GameObject inventoryUI;
    public GameObject openInvButton;
    public GameObject firstSelectedSlot;

    public List<InventorySlot> itemSlots;
    public InventorySlot heldItemSlot;
    public HeldItemSlot heldItemSlot2;
    
    private bool _isOpen = false;
    
    public PlayerInput playerInput;
    private InputAction _inventoryAction;
    
    public InvButtonSpriteSwap spriteSwap;
    
    private void Awake()
    {
        _inventoryAction = playerInput.actions["Inventory"];
        if (_inventoryAction == null)
        {
            Debug.LogError("No menu action found");
        } 
        
        foreach (InventorySlot slots in itemSlots)
        {
            // Check if the button is assigned
            if (slots.button == null || slots == null)
            {
                Debug.LogError("Button is null in InventorySlot: " + slots.name);
                continue; // Skip this slot if the button is missing
            }

            // Add an onClick listener to the Button inside each InventorySlot
            slots.button.onClick.AddListener(() => OnSlotPressed(slots));
        }
        
    }

    private void Update()
    {
        if (_inventoryAction.WasPressedThisFrame())
        {
            Debug.Log("Opening Inv/Closing Inv");
            OpenInv();
        }
    }

    public void OpenInv()
    {
        if (!_isOpen)
        {
            Debug.Log("Opening Inv");
            inventoryUI.SetActive(true);
            if (Gamepad.current != null)
            {
                EventSystem.current.SetSelectedGameObject(firstSelectedSlot);
            }
            _isOpen = true;
            spriteSwap.OnButtonClick();
            InvTimeMethod();
        }
        else
        {
            Debug.Log("Closing Inv");
            inventoryUI.SetActive(false);
            EventSystem.current.SetSelectedGameObject(null);
            _isOpen = false;
            spriteSwap.OnButtonClick();
            InvTimeMethod();
        }
 
    }
    
    private void InvTimeMethod()
    {
        if (_isOpen)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }
    
    private void OnSlotPressed(InventorySlot slot)
    {
        InventorySlot pressedSlot = slot;
        
        if (heldItemSlot.transform.childCount == 0)
        {
            if (slot.item != null)
            {
                slot.item.transform.SetParent(heldItemSlot.transform);
                heldItemSlot2.playerHeldItem = slot.item.itemType;
                
                Debug.Log("Transferred " + slot.item.name + " to the HeldItemSlot.");
                
                slot.item = null;
            }
            else
            {
                Debug.Log("No item in slot " + slot.name + " to transfer.");
            }
        }
        else
        {
            if (slot.item != null)
            {
                Debug.Log("Item in held slot");
                InventorySlot previousSlot = slot;
                
                Transform currentHeldItemTransform = heldItemSlot.transform.GetChild(0);
                InventoryItem currentHeldItem = currentHeldItemTransform.GetComponent<InventoryItem>();
                
                currentHeldItem.transform.SetParent(slot.transform);
                slot.item.transform.SetParent(heldItemSlot.transform);
                
                slot.item = currentHeldItem;
                
                Transform newHeldItemTransform = heldItemSlot.transform.GetChild(0);
                InventoryItem newHeldItem = newHeldItemTransform.GetComponent<InventoryItem>();

                heldItemSlot.item = newHeldItem;
                heldItemSlot2.playerHeldItem = newHeldItem.itemType;
            
                Debug.Log("Held item new parent = " + currentHeldItem.transform.parent.name);
                Debug.Log("Transferred " + slot.item.name + " to the HeldItemSlot.");
            }
            else
            {
                Debug.Log("No item in slot " + slot.name + " to transfer, transferring held item to empty slot.");
                Transform currentHeldItemTransform = heldItemSlot.transform.GetChild(0);
                InventoryItem currentHeldItem = currentHeldItemTransform.GetComponent<InventoryItem>();
                
                currentHeldItem.transform.SetParent(slot.transform);
                slot.item = currentHeldItem;
                
                heldItemSlot.item = null;
                heldItemSlot2.playerHeldItem = null;
            }
        }
        
    }
    
}
