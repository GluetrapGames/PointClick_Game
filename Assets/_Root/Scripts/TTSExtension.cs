using Piper;
using PixelCrushers.DialogueSystem;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Tools = PixelCrushers.DialogueSystem.Tools;

/// <summary>
///     Extends the Dialogue System by adding text-to-speech functionality using
///     the Piper plugin.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class TTSExtension : MonoBehaviour
{
	[Tooltip("Log player lines in this colour.")]
	public Color m_PlayerColor = Color.blue;
	[Tooltip("Log NPC lines in this colour.")]
	public Color m_NpcColor = Color.red;

	[SerializeField]
	private PiperManager m_Piper;
	[SerializeField]
	private bool m_LogPlayerLines;
	private AudioSource _source;

	private void Awake()
	{
		_source = GetComponent<AudioSource>();
		// Make sure the Piper Manager still exists on play.
		if (m_Piper == null)
			Reset();
	}

	private void Reset()
	{
		var prefab = AssetDatabase.LoadAssetAtPath(
			"Assets/_Root/Prefabs/Piper Manager.prefab", typeof(GameObject));

		// Check to see if the Piper Manager prefab exists.
		m_Piper = FindFirstObjectByType<PiperManager>();
		if (m_Piper == null) //< If not, create it and grab it.
			m_Piper = Instantiate(prefab).GetComponent<PiperManager>();
	}

	private void OnDestroy()
	{
		if (_source && _source.clip)
			Destroy(_source.clip);
	}

	public void OnConversationLine(Subtitle subtitle)
	{
		// Make sure the subtitle isn't NULL.
		if ((subtitle == null) | (subtitle?.formattedText == null) |
		    string.IsNullOrEmpty(subtitle?.formattedText?.text)) return;

		// Output spoken line when logging is enabled.
		if (m_LogPlayerLines)
		{
			var speakerInfo = subtitle.speakerInfo;
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
			? m_PlayerColor
			: m_NpcColor);
	}

	// Create an audio file from text, play it, then delete it.
	private async void OnInputSubmit(string text)
	{
		var toSpeech = m_Piper.TextToSpeech(text);
		_source.Stop();
		if (_source && _source.clip)
			Destroy(_source.clip);

		_source.clip = await toSpeech;
		_source.Play();
	}
}