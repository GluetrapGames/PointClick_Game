using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.UI;

namespace GlueTrap
{
    public class BackgroundMania : MonoBehaviour
    {
        private int _EnvDM;
        private int _DiaDM;
        private int _EnvDMPrev;
        private int _DiaDMPrev;
        private UnityEngine.UI.Image _BGImage;

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
                _BGImage.color -= new Color(0f, 0.03f, 0.03f, 0f);
                _EnvDMPrev = DialogueLua.GetVariable("Env_DM_Meter").asInt;
                _DiaDMPrev = DialogueLua.GetVariable("Dia_DM_Meter").asInt;
            } else if (_EnvDM >= _EnvDMPrev - 2 || _DiaDMPrev >= _DiaDMPrev - 2)
            {
                _BGImage.color += new Color(0f, 0.03f, 0.03f, 0f);
                _EnvDMPrev = DialogueLua.GetVariable("Env_DM_Meter").asInt;
                _DiaDMPrev = DialogueLua.GetVariable("Dia_DM_Meter").asInt;
            }
            
        }

        public void updateAlpha()
        {
            _BGImage.color += new Color(0f, 0f, 0f, 0.15f);
        }
        
    }
}
