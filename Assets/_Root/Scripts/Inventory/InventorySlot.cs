using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
   public Button button;
   public InventoryItem item;

   private void Awake()
   {
      
      item = gameObject.GetComponentInChildren<InventoryItem>();
      
   }
   
   public void OnDrop(PointerEventData eventData)
   {
      if (transform.childCount == 0)
      {
         GameObject dropped = eventData.pointerDrag.gameObject;
         item = dropped.GetComponent<InventoryItem>();
         item.parentAfterDrag = transform;
      }
   }
}
