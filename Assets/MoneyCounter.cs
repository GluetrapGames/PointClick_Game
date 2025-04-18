using System.Collections;
using System.Collections.Generic;
using GlueTrap.Utilities;
using TMPro;
using UnityEngine;

namespace GlueTrap
{
    public class MoneyCounter : MonoBehaviour
    {
        
        private GameManager _GameManager;
        
        void Awake()
        {
            _GameManager = Utils.GetGameManager();
        }

        void Update()
        {
            var _textBox = GetComponentInChildren<TextMeshProUGUI>();
            int currentMoney = _GameManager.m_collectedMoney;
            _textBox.text = $"<b>£</b>{currentMoney}";
        }

    }
}
