using EditorAttributes;
using Piper;
using PixelCrushers.DialogueSystem;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using CharacterInfo = PixelCrushers.DialogueSystem.CharacterInfo;
using Tools = PixelCrushers.DialogueSystem.Tools;

/// <summary>
///     Extends the Dialogue System by adding text-to-speech functionality using
///     the Piper plugin. Waits for the generated audio to finish before unloading
///     it,
///     mimicking the AudioWait command's behaviour.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class TTSExtension : MonoBehaviour
{
	[SerializeField]
	private PiperManager _PiperManager;
	[SerializeField]
	private bool _LogLines;
	[SerializeField, EnableField(nameof(_LogLines)),
	 Tooltip("Log player lines in this colour.")]
	private Color _PlayerColour = Color.blue;
	[SerializeField, EnableField(nameof(_LogLines)),
	 Tooltip("Log NPC lines in this colour.")]
	private Color _NpcColour = Color.red;
	private bool _audioWaitActive;

	private AudioSource _Source;
	private float stopTime;

	private void Awake()
	{
		_Source = GetComponent<AudioSource>();
		if (!_PiperManager)
			_PiperManager = FindFirstObjectByType<PiperManager>();
	}

#if UNITY_EDITOR
	private void Reset()
	{
		Object prefab = AssetDatabase.LoadAssetAtPath(
			"Assets/_Root/Prefabs/Piper Manager.prefab", typeof(GameObject));
		_PiperManager = FindFirstObjectByType<PiperManager>();
		if (_PiperManager == null)
			_PiperManager = Instantiate(prefab).GetComponent<PiperManager>();
	}
#endif

	// Update checks if the audio has finished playing.
	private void Update()
	{
		if (_audioWaitActive)
		{
			if (DialogueTime.time >= stopTime)
			{
				if (_Source.clip != null)
				{
					DialogueManager
						.UnloadAsset(_Source.clip); // Unload the audio clip.
					_Source.clip = null;
				}

				_audioWaitActive = false;
			}
		}
	}

	private void OnDestroy()
	{
		if (_Source && _Source.clip)
			Destroy(_Source.clip);
	}

	public void OnConversationLine(Subtitle subtitle)
	{
		// Make sure the subtitle isn't null.
		if (subtitle == null || subtitle.formattedText == null ||
		    string.IsNullOrEmpty(subtitle.formattedText.text))
			return;

		// Output spoken line when logging is enabled.
		if (_LogLines)
		{
			CharacterInfo speakerInfo = subtitle.speakerInfo;
			var speakerName =
				speakerInfo != null && speakerInfo.transform != null
					? speakerInfo.transform.name
					: "(null speaker)";
			Debug.Log(string.Format("<color={0}>{1}: {2}</color>",
				GetActorColor(subtitle), speakerName,
				subtitle.formattedText.text));
		}

		// Send text to TTS.
		var text = subtitle.formattedText.text;
		OnInputSubmit(text);
	}

	// Get currently spoken actor's colour.
	private string GetActorColor(Subtitle subtitle)
	{
		if (subtitle == null || subtitle.speakerInfo == null)
			return "white";

		return Tools.ToWebColor(subtitle.speakerInfo.isPlayer
			? _PlayerColour
			: _NpcColour);
	}

	// Create an audio file from text, play it, then wait for it to finish using a stopTime.
	private async void OnInputSubmit(string text)
	{
		var toSpeech = _PiperManager.TextToSpeech(text);
		_Source.Stop();
		if (_Source && _Source.clip)
			Destroy(_Source.clip);

		_Source.clip = await toSpeech;
		_Source.Play();

		// Set stopTime based on the length of the audio, mimicking AudioWait.
		stopTime = DialogueTime.time +
		           GetAudioClipLength(_Source, _Source.clip);
		_audioWaitActive = true;
	}

	// Similar to AudioWait's GetAudioClipLength method.
	public static float GetAudioClipLength(AudioSource audioSource,
		AudioClip audioClip)
	{
		if (audioClip == null) return 0;
		if (audioSource == null) return audioClip.length;
		var pitchAbs = Mathf.Abs(audioSource.pitch);
		if (Time.timeScale > 0)
		{
			if (pitchAbs == 1 || Mathf.Approximately(0, pitchAbs))
				return audioClip.length / Time.timeScale;
			return audioClip.length / Time.timeScale / pitchAbs;
		}

		if (pitchAbs == 1 || Mathf.Approximately(0, pitchAbs))
			return audioClip.length;
		return audioClip.length / pitchAbs;
	}
}