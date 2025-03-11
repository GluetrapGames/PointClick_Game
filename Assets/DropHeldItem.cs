using System;
using System.Collections;
using System.Collections.Generic;
using GlueTrap.Utilities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace GlueTrap
{
    public class DropHeldItem : MonoBehaviour
    {
        
        // Held item data
        [SerializeField]
        public HeldItemSlot _heldItemSlot = null;
        
        [SerializeField]
        private InventoryItem _heldItem = null;

        [SerializeField]
        private Sprite _heldItemSprite;
        
        [SerializeField]
        private ItemTypes _heldItemType = ItemTypes.None;

        // Prefabs
        [SerializeField]
        private GameObject _pickupPrefab = null;
        
        [SerializeField]
        private GameObject _itemPrefab = null;
        
        // Parent to assign new pickup to
        [SerializeField]
        public GameObject _pickupParent = null;

        // Player instance to find position to spawn new pickup
        [SerializeField]
        public PlayerGridController _playerInstance;
        
        public static DropHeldItem Instance { get; private set; }
        
        private void Awake()
        {
            
            Instance = this;
            GameObject InventoryGroup = this.transform.parent.gameObject;
            InventoryGroup.SetActive(false);
            
            /*// Finding HeldItemSlot and the pickup parent
            _heldItemSlot = GameObject.Find("HeldItemSlot").GetComponent<HeldItemSlot>();
            _pickupParent = GameObject.Find("----Pickups----");
            
            // Getting player instance for position
            _playerInstance = GameObject.Find("John");
            
            if (_heldItemSlot == null)
            {
                Debug.LogError("Failed to find held item slot!");
            }
            
            if (_pickupParent == null)
            {
                Debug.LogError("Failed to find pickup parent!");
            }*/
            
        }

        private void GetHeldItem()
        {
            // Getting data from held item
            _heldItem = _heldItemSlot.GetComponentInChildren<InventoryItem>();
            _heldItemType = _heldItem.itemType;
            _heldItemSprite = _heldItem.GetComponent<Image>().sprite;

            if (_heldItem == null || _heldItemType == ItemTypes.None || _heldItemSprite == null)
            {
                Debug.LogError("Failed to find held item data!");
            }
            
        }
        
        public void DropItem()
        {
            GetHeldItem();
            
            // Creating new pickup
            GameObject pickupInstance = Instantiate(_pickupPrefab, _pickupParent.transform);
            pickupInstance.name = _heldItemType.ToString() + " (Dropped)";
            pickupInstance.GetComponent<PickUpScript>()._ItemType = _heldItemType;
            pickupInstance.GetComponent<PickUpScript>().sprite = _heldItemSprite;
            pickupInstance.GetComponent<PickUpScript>().m_IsClicked = false;
            pickupInstance.GetComponent<PickUpScript>().m_IsDropped = true;
            pickupInstance.GetComponent<PickUpScript>()._ItemPrefab = _itemPrefab;
            pickupInstance.GetComponent<PickUpScript>().pickupEvent = "player_pickup";
            pickupInstance.GetComponent<SpriteRenderer>().sprite = _heldItemSprite;
            pickupInstance.transform.position = _playerInstance.transform.position;
            Destroy(_heldItem.gameObject);
            pickupInstance.SetActive(true);
        }
        
    }
}
