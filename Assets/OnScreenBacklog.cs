using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GlueTrap
{
    public class OnScreenBacklog : MonoBehaviour
    {
        
        private ToggleLog _ToggleLog;
        private GameObject _LogButtonSearch;
        private Button _buttonRef;
        
        void Start()
        { 
            _buttonRef = gameObject.GetComponent<Button>();
            var search = GameObject.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var thing in search)
            {
                if (thing.CompareTag("LogButton")) _LogButtonSearch = thing;
            }
            _ToggleLog = _LogButtonSearch.GetComponent<ToggleLog>();
            
            _buttonRef.onClick.AddListener(OnButtonClicked);
        }
        
        void OnButtonClicked()
        {
            _ToggleLog.toggle();
        }
        
        // Update is called once per frame
        void Update()
        {
            if(!_LogButtonSearch)
            {
                var search = GameObject.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                foreach (var thing in search)
                {
                    if (thing.CompareTag("LogButton")) _LogButtonSearch = thing;
                }
            }
            if(_LogButtonSearch && !_ToggleLog) _ToggleLog = _LogButtonSearch.GetComponent<ToggleLog>();
        }
    }
}
