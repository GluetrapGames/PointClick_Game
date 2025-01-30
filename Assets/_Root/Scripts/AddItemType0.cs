using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddItemType0 : MonoBehaviour
{
    public InventorySlot[] itemSlots;
    private bool _slotFound = false;
    public GameObject itemPrefab;
    public Sprite sprite;
    public void OnButtonPress()
    {
        while (!_slotFound)
        {
            Debug.Log("slotFound is false");
            for (int i = 0; i < itemSlots.Length; i++)
            {
                if (itemSlots[i].transform.childCount == 0 && _slotFound == false)
                {
                    Debug.Log("Slot" + itemSlots[i].name + " is empty");
                    itemPrefab.GetComponent<InventoryItem>().itemType = "Type 0";
                    itemPrefab.GetComponent<Image>().sprite = sprite;
                    GameObject.Instantiate(itemPrefab, itemSlots[i].transform);
                    _slotFound = true;
                    Debug.Log("Added type 0 item to slot " + itemSlots[i].name + " - Type Validation: " + itemPrefab.GetComponent<InventoryItem>().itemType);
                }
                
                if (i == itemSlots.Length - 1 && _slotFound == false)
                {
                    Debug.LogWarning("Inventory full while attempting type 0 spawning, exiting while loop");
                    _slotFound = true;
                }
            }
        }
        
        _slotFound = false;
        
    }
    
}
