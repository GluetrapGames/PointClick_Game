using AYellowpaper.SerializedCollections;
using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GlueTrap
{
    public class NameplateSwap : MonoBehaviour
    {
        [SerializeField]
        private SerializedDictionary<string, Sprite> _SpriteDictionary;
        private string _SpeakerName;
        private Image _Image;

        private void Start()
        {
            _Image = GetComponent<Image>();
        }
        private void OnConversationLine(Subtitle subtitle)
        {
            _SpeakerName = subtitle.speakerInfo.Name;
        }

        private void SetNamePlate() 
        {
            foreach (var kvp in _SpriteDictionary) 
            {
                if (kvp.Key == _SpeakerName) 
                {
                    _Image.sprite = kvp.Value;
                }
            }
        }
    }
}
