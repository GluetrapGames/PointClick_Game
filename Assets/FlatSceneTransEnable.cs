using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GlueTrap.Utilities;
using UnityEngine.SceneManagement;

using UnityEngine;

namespace GlueTrap
{
    public class FlatSceneTransEnable : MonoBehaviour
    {
        [SerializeField]
        private GameObject _sceneTransition;
        private GameManager _GameManager;
        private FlatSceneTransEnable _adam;

        private void Awake()
        {
            _GameManager = Utils.GetGameManager();
            _adam = this;
        }

        private void Update()
        {
            if (_GameManager.m_HasFlatCall)
            {
                _sceneTransition.transform.position = new Vector3(-1.77f, -5.79f, 1);
                _adam.enabled = false;
            }
        }
    }
}
