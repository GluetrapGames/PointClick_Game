using AYellowpaper.SerializedCollections;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using Random = UnityEngine.Random;

public class BlabController : MonoBehaviour
{
	[SerializeField]
	private GameObject _LogWindow;
	private string _SpeakerName;
	public float blabSpeed = 0.1f;
	private bool isBlabbing = false;


	private void Update()
	{
		// If the hitory window is open, then pause the blabs.
		// Currently doesn't restart blabs
		if (_LogWindow.activeInHierarchy)
		{
			CancelInvoke("postBlab");
		}
	}

	// When a conversation line begins, gets the current speaker's name
	// And calls for audio to play.
	private void OnConversationLine(Subtitle subtitle)
	{
		_SpeakerName = subtitle.speakerInfo.Name;
        AkSoundEngine.SetSwitch("CharacterBlab", _SpeakerName, gameObject);
        PlayActorClip();
		Debug.Log(_SpeakerName);
	}

	// Plays the audio clip associated with the speaker from the database.
	public void PlayActorClip()
	{
		if (!isBlabbing)
		{
            isBlabbing = true;
            InvokeRepeating("postBlab", 0f, blabSpeed);
            Debug.Log("Started blabbing");
        }
		
	}

	// Stops the audio clip at the end of the conversation line.
	public void StopActorClip()
	{
        isBlabbing = false;
        CancelInvoke("postBlab");
		Debug.Log("Stopped blabbing");
	}

	private void postBlab()
	{
		AkSoundEngine.PostEvent("Blab", gameObject);
	}
}