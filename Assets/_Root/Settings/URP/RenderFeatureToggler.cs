using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

[System.Serializable]

public class RenderFeatureToggler : MonoBehaviour
{
    [SerializeField]
    private List<ScriptableRendererFeature> renderFeatures = new List<ScriptableRendererFeature>();
    [SerializeField]
    private UniversalRenderPipelineAsset pipelineAsset;
    private bool _toggled = true;
    [SerializeField]
    private Sprite _offSprite;
    [SerializeField]
    private Sprite _onSprite;
    private Image _Image;

    private void Awake()
    {
        _Image = GetComponent<Image>();
    }

    public void ButtonClicked()
    {
        Debug.Log("Button clicked");
        for(int i = 0; i < renderFeatures.Count; i++)
        { 
            Debug.Log("Post Processing Toggled");
            renderFeatures[i].SetActive(_toggled);
            SwapSprite();
            _toggled = !_toggled;
        }
    }

    private void SwapSprite()
    {
        if (_toggled)
        {
            _Image.sprite = _onSprite;
        }
        else
        {
            _Image.sprite = _offSprite;
        }
    }
    
}
