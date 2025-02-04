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

    public void ButtonClicked()
    {
        Debug.Log("Button clicked");
        for(int i = 0; i < renderFeatures.Count; i++)
        { 
            Debug.Log("Post Processing Toggled");
            renderFeatures[i].SetActive(_toggled);
            _toggled = !_toggled;
        }
    }
}
