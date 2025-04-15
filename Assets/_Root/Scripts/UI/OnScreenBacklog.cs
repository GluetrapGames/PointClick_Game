using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GlueTrap
{
    public class OnScreenBacklog : MonoBehaviour
    {
        private GameObject _ManagerTemplate;
        private ToggleLog _ToggleScript;

        private void Start()
        {
            _ManagerTemplate = GameObject.Find("Runic Standard Dialogue UI");
            _ToggleScript = _ManagerTemplate.GetComponent<ToggleLog>();
        }

        public void CallToggleLog() 
        {
            _ToggleScript.toggle();
        }
    }
}
