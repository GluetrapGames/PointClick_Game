using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class InventoryRenderingHierarchy : MonoBehaviour
{

    [SerializeField] private GameObject darkBG;
    
    void Start()
    {
        Transform darkBackgroundTransform = darkBG.transform;
        Transform heldItemTbTransform = transform; // This script is on HeldItemTB

        // Ensure HeldItemTB is drawn above DarkBackground but below Inventory
        int targetIndex = darkBackgroundTransform.GetSiblingIndex() + 1;
        heldItemTbTransform.SetSiblingIndex(targetIndex);
    }
}
