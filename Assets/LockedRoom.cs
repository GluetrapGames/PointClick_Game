using System;
using System.Collections;
using System.Collections.Generic;
using GlueTrap.Utilities;
using UnityEngine;

namespace GlueTrap
{
    public class LockedRoom : MonoBehaviour
    {
        public bool hasKey;
        private GameManager _gameManager;
        [SerializeField]
        private LockedDoors _lockedDoors;
        private void Awake()
        {
            _gameManager = Utils.GetGameManager();
            hasKey = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (!hasKey) keyCheck();
        }
        
        private void keyCheck()
        {
            switch (_lockedDoors)
            {
                case LockedDoors.Frontdoor:
                    if(_gameManager.m_hasFrontdoorKey) hasKey = true;
                    break;
                case LockedDoors.TaxidermyHallway:
                    if(_gameManager.m_hasTaxidermyKey) hasKey = true;
                    break;
            }
        }
        
    }
    
}
