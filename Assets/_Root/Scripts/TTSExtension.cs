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
///     the Piper plugin.
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

	private AudioSource _Source;


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

		// Check to see if the Piper Manager prefab exists.
		_PiperManager = FindFirstObjectByType<PiperManager>();
		if (_PiperManager == null) //< If not, create it and grab it.
			_PiperManager = Instantiate(prefab).GetComponent<PiperManager>();
	}

#endif
	private void OnDestroy()
	{
		if (_Source && _Source.clip)
			Destroy(_Source.clip);
	}

	public void OnConversationLine(Subtitle subtitle)
	{
		// Make sure the subtitle isn't NULL.
		if ((subtitle == null) | (subtitle?.formattedText == null) |
		    string.IsNullOrEmpty(subtitle?.formattedText?.text)) return;

		// Output spoken line when logging is enabled.
		if (_LogLines)
		{
			CharacterInfo speakerInfo = subtitle.speakerInfo;
			var speakerName = speakerInfo != null &&
			                  speakerInfo.transform != null
				? speakerInfo.transform.name
				: "(null speaker)";

			Debug.Log(string.Format("<color={0}>{1}: {2}</color>",
				new object[]
				{
					GetActorColor(subtitle), speakerName,
					subtitle.formattedText.text
				}));
		}

		// Send text to TTS.
		var text = subtitle.formattedText.text;
		OnInputSubmit(text);
	}

	// Get currently spoken actors name.
	private string GetActorName(Transform actor)
	{
		return actor != null ? actor.name : "(null transform)";
	}

	// Get currently spoken actors colour.
	private string GetActorColor(Subtitle subtitle)
	{
		if ((subtitle == null) | (subtitle?.speakerInfo == null))
			return "white";

		return Tools.ToWebColor(subtitle.speakerInfo.isPlayer
			? _PlayerColour
			: _NpcColour);
	}

	// Create an audio file from text, play it, then delete it.
	private async void OnInputSubmit(string text)
	{
		var toSpeech = _PiperManager.TextToSpeech(text);
		_Source.Stop();
		if (_Source && _Source.clip)
			Destroy(_Source.clip);

		_Source.clip = await toSpeech;
		_Source.Play();
	}
}