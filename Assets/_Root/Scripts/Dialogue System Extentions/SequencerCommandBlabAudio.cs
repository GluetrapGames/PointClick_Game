using UnityEngine;

namespace PixelCrushers.DialogueSystem.SequencerCommands
{
[SequencerCommandGroup("submenu")]
public class SequencerCommandBlabAudio : SequencerCommand
{
	private string m_BlabType;
	private string m_SpeakerName;
	private Transform m_SpeakerTransform;

	public void Start()
	{
		// The type of blab - Angry, Shocked, etc
		m_BlabType = GetParameter(0);

		// The current speaker's transform.
		m_SpeakerTransform = GetSubject(1);

		if (m_SpeakerTransform)
		{
			m_SpeakerName = m_SpeakerTransform.gameObject.name;
			Debug.Log($"{m_SpeakerName}");
		}
		else
		{
			Debug.LogWarning($"{this}: Speaker Transform is Null!");
		}
	}

	public void Update()
	{
		if (m_BlabType == "neutral") // Blank
			AkSoundEngine.SetRTPCValue("pitch_blab", 50f);

		if (m_BlabType == "shock" || m_BlabType == "surprise") // Yellow
			AkSoundEngine.SetRTPCValue("pitch_blab", 100f);

		if (m_BlabType == "angry") // Red
			AkSoundEngine.SetRTPCValue("pitch_blab", 0f);

		if (m_BlabType == "thinking" || m_BlabType == "questionHigh") // Purple
			AkSoundEngine.SetRTPCValue("pitch_blab", 70f);

		if (m_BlabType == "happy") // Green
			AkSoundEngine.SetRTPCValue("pitch_blab", 85f);

		if (m_BlabType == "sad" || m_BlabType == "questionLow") // Blue
			AkSoundEngine.SetRTPCValue("pitch_blab", 37.5f);


		// Add any update code here. When the command is done, call Stop().
		// If you've called stop above in Awake(), you can delete this method.

		/* I imagine this is where you would call your audio stuff.
		 * For example if i called this sequence command in the dialogue tree under a John node
		 * and wrote it as BlabAudio(Angry, Speaker)
		 * Then in this function you would then call the John's angry blab.
		 */

		if (m_BlabType == null)
			Debug.Log("BLABTYPE NULL --- BlabAudio sequence");
		if (m_SpeakerTransform == null)
			Debug.Log("SPEAKERTRANSFORM NULL --- BlabAudio sequence");
		Stop();
	}

	public void OnDestroy()
	{
		// Add your finalization code here. This is critical. If the sequence is cancelled and this
		// command is marked as "required", then only Awake() and OnDestroy() will be called.
		// Use it to clean up whatever needs cleaning at the end of the sequencer command.
		// If you don't need to do anything at the end, you can delete this method.
	}
}
}