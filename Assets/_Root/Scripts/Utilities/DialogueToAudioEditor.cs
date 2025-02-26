using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Piper;
using PixelCrushers.DialogueSystem;
using UnityEditor;
using UnityEngine;

public class DialogueToAudioEditor : EditorWindow
{
	private static PiperManager _PiperManager;
	private static DialogueDatabase _DialogueDatabase;
	private static List<Conversation> _Conversations;
	private static int _selectedConversationIndex;
	private static Conversation _SelectedConversation;
	private static string _SaveFolder = "Assets/DialogueAudio/";

	private void OnGUI()
	{
		GUILayout.Label("TTS Audio Generator", EditorStyles.boldLabel);

		// Select the Piper Manager.
		_PiperManager = (PiperManager)EditorGUILayout.ObjectField(
			"Piper Manager:", _PiperManager, typeof(PiperManager), true);

		// Specify the save folder.
		_SaveFolder = EditorGUILayout.TextField("Save Folder:", _SaveFolder);

		// Allow manual assignment of the Dialogue Database.
		_DialogueDatabase = (DialogueDatabase)EditorGUILayout.ObjectField(
			"Dialogue Database:", _DialogueDatabase, typeof(DialogueDatabase),
			true);
		if (_DialogueDatabase != null &&
		    (_Conversations == null || _Conversations.Count == 0))
		{
			_Conversations =
				new List<Conversation>(_DialogueDatabase.conversations);
		}

		// Display a dropdown for conversation selection.
		if (_Conversations != null && _Conversations.Count > 0)
		{
			var conversationNames = new List<string>();
			foreach (Conversation conv in _Conversations)
				conversationNames.Add(conv.Title);
			_selectedConversationIndex = EditorGUILayout.Popup("Conversation:",
				_selectedConversationIndex, conversationNames.ToArray());
			if (_Conversations.Count > _selectedConversationIndex)
			{
				_SelectedConversation =
					_Conversations[_selectedConversationIndex];
			}
		}
		else
		{
			EditorGUILayout.HelpBox(
				"No conversations found in the Dialogue Database.",
				MessageType.Warning);
		}

		if (GUILayout.Button("Generate Audio")) GenerateConversationAudio();
	}

	[MenuItem("Tools/Generate TTS Audio from Conversation")]
	public static void ShowWindow()
	{
		GetWindow<DialogueToAudioEditor>("TTS Audio Generator");
	}

	private void GenerateConversationAudio()
	{
		// Check for required references.
		if (_PiperManager == null)
		{
			Debug.LogError("Piper Manager is not assigned.");
			return;
		}

		if (_SelectedConversation == null ||
		    _SelectedConversation.dialogueEntries == null)
		{
			Debug.LogError("No conversation selected.");
			return;
		}

		if (!Directory.Exists(_SaveFolder))
			Directory.CreateDirectory(_SaveFolder);

		// Generate audio for each dialogue entry in the selected conversation.
		foreach (DialogueEntry entry in _SelectedConversation.dialogueEntries)
			if (!string.IsNullOrEmpty(entry.DialogueText))
				_ = GenerateAndSaveAudio(entry.id, entry.DialogueText);
	}

	private async Task GenerateAndSaveAudio(int id, string text)
	{
		Debug.Log(text);
		// Generate the TTS audio using the Piper Manager.
		AudioClip audioClip = await _PiperManager.TextToSpeech(text);
		if (audioClip == null)
		{
			Debug.LogError($"Failed to generate audio for line {id}: {text}.");
			return;
		}

		// Prepare the file path.
		var filePath = Path.Combine(_SaveFolder, $"Line_{id}.wav");
		SaveAudioClip(audioClip, filePath);
	}

	private void SaveAudioClip(AudioClip clip, string filePath)
	{
		// Save the audio clip to a .wav file.
		var success = SavWav.Save(filePath, clip);
		if (success)
			Debug.Log($"Saved audio to {filePath}.");
		else
		{
			Debug.LogError(
				$"Failed to save audio clip <{clip.name}> to {filePath}.");
		}

		AssetDatabase.Refresh();
	}
}