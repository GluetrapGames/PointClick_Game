using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemChecker : MonoBehaviour
{
    public HeldItemSlot heldItem;
    public SpriteRenderer renderer;
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger Called");
        if (other.gameObject.CompareTag("Player"))
        {
            if (heldItem.playerHeldItem == "Type 0")
            {
                Debug.Log("Item is type 0, Setting sprite colour to red");
                renderer.color = Color.red;
            }
            else if (heldItem.playerHeldItem == "Type 1")
            {
                Debug.Log("Item is type 1, Setting sprite colour to green");
                renderer.color = Color.green;
            }
            else
            {
                Debug.Log("No item / item has no type, Setting sprite colour to yellow");
                renderer.color = Color.yellow;
            }
        }

    }
}
