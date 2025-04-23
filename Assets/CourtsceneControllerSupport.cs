using System;
using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using InputDevice = PixelCrushers.InputDevice;
using InputDeviceManager = PixelCrushers.Wrappers.InputDeviceManager;

namespace GlueTrap
{
    public class CourtsceneControllerSupport : MonoBehaviour
    {

        private GameObject _button;
        private bool _IsController = Gamepad.current != null;

        private void Start()
        {
            _button = GameObject.FindGameObjectWithTag("Continue");

        }

        public void SetSelectedButton()
        {
            _IsController = Gamepad.current != null;
            if (!_IsController) return;
            
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
            EventSystem.current.SetSelectedGameObject(_button);
            diaManager.gameObject.GetComponent<InputDeviceManager>().SetInputDevice(InputDevice.Joystick);
        }
        
        
        
    }
}
