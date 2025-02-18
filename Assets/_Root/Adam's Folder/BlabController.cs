using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;
using System;
using AYellowpaper.SerializedCollections;
using UnityEngine.Rendering;

public class BlabController : MonoBehaviour
{
    [SerializeField]
    private AYellowpaper.SerializedCollections.SerializedDictionary<String, AudioClip> _ClipDictionary;
    private string speakerName;

    public UnityUITypewriterEffect m_TypeWritterEffect;

    private void OnConversationStart(Transform subtitle)
    {
        //Debug.Log(DialogueManager.CurrentConversationState.subtitle.listenerInfo.Name);
        //string conversationTitle = DialogueManager.lastConversationStarted;
        //if (!string.IsNullOrEmpty(conversationTitle))
        //{
        //    var conversation = DialogueManager.masterDatabase.GetConversation(conversationTitle);
        //    if (conversation.dialogueEntries.Count > 0)
        //    {
        //        var firstEntry = conversation.dialogueEntries[0];
        //        var speakerInfo = DialogueManager.MasterDatabase.GetActor(firstEntry.ActorID);

        //        if (speakerInfo != null)
        //        {
        //            Debug.Log("First Speaker Name: " + speakerInfo.Name);
        //        }
        //    }
        //}
    }


    void OnConversationLine(Subtitle subtitle)
    {
        speakerName = subtitle.speakerInfo.Name;
        PlayActorClip();
        Debug.Log(speakerName);
    }


    public void PlayActorClip()
    {
        Debug.Log("Entered audio");
        foreach (var actor in _ClipDictionary)
        {
            if (speakerName == actor.Key)
            {
                m_TypeWritterEffect.audioClip = actor.Value;
            }
        }
    }

    public void StopActorClip() 
    {
        m_TypeWritterEffect.audioClip = null;
        m_TypeWritterEffect.audioSource.Stop();
    }

}
