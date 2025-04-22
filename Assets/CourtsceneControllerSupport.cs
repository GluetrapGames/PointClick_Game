using System;
using System.Collections;
using System.Collections.Generic;
using PixelCrushers;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using InputDeviceManager = PixelCrushers.Wrappers.InputDeviceManager;

namespace GlueTrap
{
    public class CourtsceneControllerSupport : MonoBehaviour
    {

        private GameObject _button;

        private void Start()
        {
            _button = GameObject.FindGameObjectWithTag("Continue");

        }

        public void SetSelectedButton()
        {
            var search = GameObject.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var thing in search)
            {
                if (thing.CompareTag("Continue")) _button = thing;
            }

            var diaManager = DialogueManager.Instance;
            if (!_button)
            {
                Debug.LogError("CourtsceneControllerSupport Can't Find Button");
                return;
            }
            diaManager.gameObject.GetComponent<InputDeviceManager>().SetInputDevice(InputDevice.Joystick);
            EventSystem.current.SetSelectedGameObject(_button);
        }
        
        
        
    }
}
