using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;

namespace GlueTrap
{
    public class MainMenuControllerSupp : MonoBehaviour
    {

        private GameObject _PlayButton;
        
        // Start is called before the first frame update
        void Start()
        {
            _PlayButton = GameObject.Find("PlayButton");
            if (_PlayButton) return;
            _PlayButton = GameObject.Find("ResumeButton");
        }

        private void Update()
        {
            if (_PlayButton) return;
            _PlayButton = GameObject.Find("PlayButton");
            if (_PlayButton) return;
            _PlayButton = GameObject.Find("SettingsButton");
        }

        public void SetButton()
        {
            EventSystem.current.SetSelectedGameObject(_PlayButton);
        }
    }
}
