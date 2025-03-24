using AYellowpaper.SerializedCollections;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.UI;

namespace GlueTrap
{
public class NameplateSwap : MonoBehaviour
{
	[SerializeField]
	private SerializedDictionary<string, Sprite> _SpriteDictionary = new();

	private Image _Image;
	private string _SpeakerName;

	private void Awake()
	{
		_Image = GetComponent<Image>();
	}

	private void OnConversationLine(Subtitle subtitle)
	{
		if (_SpeakerName == subtitle.speakerInfo.Name) return;
		_SpeakerName = subtitle.speakerInfo.Name;
		SetNamePlate();
	}

	private void SetNamePlate()
	{
		if (_SpriteDictionary.TryGetValue(_SpeakerName, out Sprite sprite))
			_Image.sprite = sprite;
	}
}
}