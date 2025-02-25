using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using System;
using AYellowpaper.SerializedCollections;
using UnityEngine.Rendering;
using UnityEngine;
using Random = UnityEngine.Random;
using PixelCrushers.DialogueSystem.Wrappers;
using PixelCrushers.DialogueSystem.SequencerCommands;
using UnityEngine.InputSystem.Android.LowLevel;

public class BlabController : MonoBehaviour
{
    [SerializeField]
    private AYellowpaper.SerializedCollections.SerializedDictionary<String, AudioClip> _ClipDictionary;
    [SerializeField]
    private AudioSource _AudioSource;
    [SerializeField]
    private GameObject _LogWindow;
    private float _LastAudioTime = 0.0f;
    private string _SpeakerName;
    public PixelCrushers.DialogueSystem.TextMeshProTypewriterEffect m_TypeWritterEffect;


    // Update checks if an audio clip is currently playing.
    // Then changes the pitch for the next clip loop.
    private void Update()
    {
        if (_AudioSource.isPlaying)
        {
            // Detect when the clip loops
            if (_AudioSource.time < _LastAudioTime) // Loop detected
            {
                _AudioSource.pitch = Random.Range(0.8f, 1.2f);
            }
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
    void OnConversationLine(Subtitle subtitle)
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
        {
            if (_SpeakerName == actor.Key)
            {
                m_TypeWritterEffect.audioClip = actor.Value;
            }
        }
    }

    // Stops the audio clip at the end of the conversation line.
    public void StopActorClip()
    {
        m_TypeWritterEffect.audioClip = null;
        _AudioSource.Stop();
        _LastAudioTime = 0f;
    }
}
