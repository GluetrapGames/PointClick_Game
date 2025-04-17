using System;
using System.Collections;
using System.Collections.Generic;
using GlueTrap.Utilities;
using PixelCrushers.DialogueSystem;
using UnityEngine;

namespace GlueTrap
{
    public class RecordPlayer : MonoBehaviour
    {
        private BreakableItem _RecordPlayerBreakableRef;
        private InteractDialgoue _RecordPlayerInteractRef;
        private InventoryItem _HeldItem;
        private GameManager _GameManager;
        private bool _CanInteract;
        private bool _HasInteraction;
        
        void Start()
        {
            _GameManager = Utils.GetGameManager();
            _RecordPlayerBreakableRef = GetComponent<BreakableItem>();
            _RecordPlayerInteractRef = GetComponent<InteractDialgoue>();
        }

        private void Update()
        {
            if (_HasInteraction) return;
            _CanInteract = DialogueLua.GetVariable("Has_Record").asBool;
            if (!_CanInteract || !_RecordPlayerInteractRef.m_Interacting) return;
            Interaction();
        }

        private void Interaction()
        {
            _HeldItem = _GameManager.m_InventoryManager.m_HeldItemSlot.GetComponentInChildren<InventoryItem>();
            
            if(!_HeldItem) return;
            if (!_RecordPlayerBreakableRef || _RecordPlayerBreakableRef._itemHp != _RecordPlayerBreakableRef._itemMaxHp) return;
            if (_HeldItem.itemData.m_Item.m_Type != ItemTypes.Record) return;
            
            Destroy(_HeldItem.gameObject);
            Debug.Log("Record Interaction");
            _HasInteraction = true;
            
            // do audio thing here !!!!       

        }
        
    }
}
