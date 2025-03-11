using PixelCrushers.DialogueSystem;
using PixelCrushers.DialogueSystem.Wrappers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetVariables : MonoBehaviour
{
    public void ResetDatabaseVariables() 
    {
        //big willy!
        DialogueLua.SetVariable("HallEntryEventComplete", false);
        DialogueLua.SetVariable("BathroomEntryEventComplete", false);
        DialogueLua.SetVariable("BedroomEntryEventComplete", false);
        DialogueLua.SetVariable("SpareRoomEntryEventComplete", false);
        DialogueLua.SetVariable("TaxidermyEntryEventComplete", false);
    }
}
