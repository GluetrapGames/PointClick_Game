using AYellowpaper.SerializedCollections;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using Random = UnityEngine.Random;

public class BlabController : MonoBehaviour
{
	[SerializeField]
	private SerializedDictionary<string, AudioClip> _ClipDictionary;
	[SerializeField]
	private AudioSource _AudioSource;
	[SerializeField]
	private GameObject _LogWindow;
	public TextMeshProTypewriterEffect m_TypeWritterEffect;
	private float _LastAudioTime;
	private string _SpeakerName;


	// Update checks if an audio clip is currently playing.
	// Then changes the pitch for the next clip loop.
	private void Update()
	{
		if (_AudioSource.isPlaying)
		{
			// Detect when the clip loops
			if (_AudioSource.time < _LastAudioTime) // Loop detected
				_AudioSource.pitch = Random.Range(0.8f, 1.2f);
			_LastAudioTime = _AudioSource.time;
		}

		// If the hitory window is open, then pause the blabs.
		if (_LogWindow.activeInHierarchy)
			_AudioSource.Pause();
		else
			_AudioSource.UnPause();
	}

	private void OnConversationStart(Transform subtitle)
	{
	}

	// When a conversation line begins, gets the current speaker's name
	// And calls for audio to play.
	private void OnConversationLine(Subtitle subtitle)
	{
		_SpeakerName = subtitle.speakerInfo.Name;
		PlayActorClip();
		Debug.Log(_SpeakerName);
	}

	// Plays the audio clip associated with the speaker from the database.
	public void PlayActorClip()
	{
		//Debug.Log("Entered audio");
		foreach (var actor in _ClipDictionary)
			if (_SpeakerName == actor.Key)
				m_TypeWritterEffect.audioClip = actor.Value;
	}

	// Stops the audio clip at the end of the conversation line.
	public void StopActorClip()
	{
		m_TypeWritterEffect.audioClip = null;
		_AudioSource.Stop();
		_LastAudioTime = 0f;
	}
}