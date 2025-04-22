
using UnityEngine;
using System.Collections;
using PixelCrushers.DialogueSystem;

namespace PixelCrushers.DialogueSystem.SequencerCommands
{
    [SequencerCommandGroup("submenu")]
    public class SequencerCommandBlabAudio : SequencerCommand
    {
        private string m_BlabType;
        private Transform m_SpeakerTransform;
        private string m_SpeakerName;

        public void Awake()
        {
            // The type of blab - Angry, Shocked, etc
            m_BlabType = GetParameter(0);

            // The current speaker's transform.
            m_SpeakerTransform = GetSubject(1);

            m_SpeakerName = m_SpeakerTransform.gameObject.name;
            Debug.Log($"{m_SpeakerName}");
        }

        public void Update()
        {
            

            if (m_BlabType == "neutral")
                AkSoundEngine.SetRTPCValue("pitch_blab", 5f);

            if (m_BlabType == "surprise")
                AkSoundEngine.SetRTPCValue("pitch_blab", 10f);

            if (m_BlabType == "angry")
                AkSoundEngine.SetRTPCValue("pitch_blab", 0f);

            if (m_BlabType == "questionHigh")
                AkSoundEngine.SetRTPCValue("pitch_blab", 6.5f);
            
            if (m_BlabType == "questionLow")
                AkSoundEngine.SetRTPCValue("pitch_blab", 3.75f);
            // Add any update code here. When the command is done, call Stop().
            // If you've called stop above in Awake(), you can delete this method.

            /* I imagine this is where you would call your audio stuff.
            * For example if i called this sequence command in the dialogue tree under a John node
            * and wrote it as BlabAudio(Angry, Speaker)
            * Then in this function you would then call the John's angry blab.
            */

            if (m_BlabType == null) Debug.Log("BLABTYPE NULL --- BlabAudio sequence");
            if (m_SpeakerTransform == null) Debug.Log("SPEAKERTRANSFORM NULL --- BlabAudio sequence");
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


