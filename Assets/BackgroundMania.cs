using System;
using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace GlueTrap
{
    public class BackgroundMania : MonoBehaviour
    {
        private int _EnvDM;
        private int _DiaDM;
        private int _EnvDMPrev;
        private int _DiaDMPrev;
        private Image _BGImage;

        private void Awake()
        {
            _EnvDM = DialogueLua.GetVariable("Env_DM_Meter").asInt;
            _DiaDM = DialogueLua.GetVariable("Dia_DM_Meter").asInt;
            _EnvDMPrev = DialogueLua.GetVariable("Env_DM_Meter").asInt;
            _DiaDMPrev = DialogueLua.GetVariable("Dia_DM_Meter").asInt;
            _BGImage = GetComponent<Image>();
        }

        private void Update()
        {
            _EnvDM = DialogueLua.GetVariable("Env_DM_Meter").asInt;
            _DiaDM = DialogueLua.GetVariable("Dia_DM_Meter").asInt;
            
            if (_EnvDM >= _EnvDMPrev + 2 || _DiaDMPrev >= _DiaDMPrev + 2)
            {
                _BGImage.tintColor -= new Color(0, 20, 20, 0);
                _EnvDMPrev = DialogueLua.GetVariable("Env_DM_Meter").asInt;
                _DiaDMPrev = DialogueLua.GetVariable("Dia_DM_Meter").asInt;
            }
            
        }
        
    }
}
