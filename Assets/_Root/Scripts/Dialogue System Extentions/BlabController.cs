using AYellowpaper.SerializedCollections;
using PixelCrushers.DialogueSystem;
using UnityEngine;

namespace GlueTrap
{
using Random = Random;

public class BlabController : MonoBehaviour
{
	[SerializeField]
	private SerializedDictionary<string, AudioClip> _ClipDictionary;
	[SerializeField]
	private AudioSource _AudioSource;
	[SerializeField]
	private GameObject _LogWindow;
	[SerializeField]
	private bool _Log;

	public TextMeshProTypewriterEffect m_TypeWritterEffect;
	private string _SpeakerName;
	private bool isBlabbing = false;
	private bool fuckyou = false;
	private float blabSpeed = 0.1f;


        private void Update()
        {
			if (_LogWindow.activeInHierarchy)
			{
				StopActorClip();
			}
        }

        // When a conversation line begins, gets the current speaker's name
        // And calls for audio to play.
        private void OnConversationLine(Subtitle subtitle)
	{
		_SpeakerName = subtitle.speakerInfo.Name;
		PlayActorClip();
		AkSoundEngine.SetSwitch("CharacterBlab", _SpeakerName, gameObject);
		if (_Log) Debug.Log(_SpeakerName);

	}

	// Posts Wwise event in loop
	public void PlayActorClip()
	{
			if (!isBlabbing)
			{
				isBlabbing = true;
                InvokeRepeating("postBlab", 0f, blabSpeed);
            }
			
	}

	// Stops the audio clip at the end of the conversation line.
	public void StopActorClip()
	{
			isBlabbing = false;
			CancelInvoke("postBlab");
	}

	private void postBlab()
		{
			AkSoundEngine.PostEvent("Blab", gameObject);
		}
}
}