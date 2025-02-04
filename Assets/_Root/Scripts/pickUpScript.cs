using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class pickUpScript : MonoBehaviour
{
    public bool isClicked;
    public string itemType;
    public InventorySlot[] itemSlots;
    public GameObject itemPrefab;
    public Sprite sprite;
    private bool _slotFound = false;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(true);
        isClicked = false;
    }

    // Update is called once per frame
    void Update()
    {

        // When mouse is clicked, Ray cast to check if the game object space was clicked.
        if (Input.GetMouseButtonDown(0)) 
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
                isClicked = true;
            else 
                isClicked = false;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        while (!_slotFound)
        {
            Debug.Log("slotFound is false");
            for (int i = 0; i < itemSlots.Length; i++)
            {
                if (itemSlots[i].transform.childCount == 0 && _slotFound == false)
                {
                    Debug.Log("Slot" + itemSlots[i].name + " is empty");
                    itemPrefab.GetComponent<InventoryItem>().itemType = itemType;
                    itemPrefab.GetComponent<Image>().sprite = sprite;
                    GameObject.Instantiate(itemPrefab, itemSlots[i].transform);
                    _slotFound = true;
                    Debug.Log("Added " + itemType + " to slot " + itemSlots[i].name + " - Type Validation: " + itemPrefab.GetComponent<InventoryItem>().itemType);
                }
                
                if (i == itemSlots.Length - 1 && _slotFound == false)
                {
                    Debug.LogWarning("Inventory full while attempting " + itemType +" spawning, exiting while loop");
                    _slotFound = true;
                }
            }
        }
        
        _slotFound = false;
        
        // If the game object has been clicked and the player is within the trigger then hide the object and display alert message.
        if (isClicked) 
        { 
            Debug.Log("Item collected");
            PixelCrushers.DialogueSystem.DialogueManager.ShowAlert(gameObject.name + " has been collected!");
            gameObject.SetActive(false); 
        }
        
    }
}
