using PixelCrushers.DialogueSystem;
using PixelCrushers.DialogueSystem.Wrappers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetVariables : MonoBehaviour
{
    public void ResetDatabaseVariables() 
    {
        // Entry Dialogue //
        DialogueLua.SetVariable("HallEntryEventComplete", false);               // Upstairs Hallway
        DialogueLua.SetVariable("BathroomEntryEventComplete", false);           // Upstairs Bathroom
        DialogueLua.SetVariable("BedroomEntryEventComplete", false);            // Master Bedroom
        DialogueLua.SetVariable("SpareRoomEntryEventComplete", false);          // Spare Bedroom
        DialogueLua.SetVariable("TaxidermyEntryEventComplete", false);          // Taxidermy Room
        DialogueLua.SetVariable("DownstairsHallwayEntryEventComplete", false);  // Downstairs Hallway
        
        // Room Checks //
        DialogueLua.SetVariable("IsFrontDoorUnlocked", false);                  // Outside front door locked
        DialogueLua.SetVariable("MarkPhoneCallFinished", false);                // John's flat phonecall finished
        DialogueLua.SetVariable("Final_Phonecall", false);                      // Final phonecall to Jack
        DialogueLua.SetVariable("Rooms_Entered", 0);                            // Total (unique?) rooms entered
        DialogueLua.SetVariable("Items_Broken", 0);                             // Total scenery items broken
        DialogueLua.SetVariable("TV_Broken", false);                            // Livingroom TV broken
        DialogueLua.SetVariable("Fridge_Used", false);                          // Kitchen fridge used
        DialogueLua.SetVariable("BedPressed", 0);                               // Master bedroom bed used

        // Item collection //
        DialogueLua.SetVariable("Stole_Cigs", false);                           // Cigarettes
        DialogueLua.SetVariable("Crowbar_Collected", false);                    // Crowbar
        DialogueLua.SetVariable("Money_Collected", false);                      // Money wad
        DialogueLua.SetVariable("Clues_Found", 0);                              // Total clues found
        DialogueLua.SetVariable("Collected_Item_List", "");                     // List of collected items

        // Destruction/Meek //
        DialogueLua.SetVariable("Dialogue_DM_Meter", 20);                       // In dialogue
        DialogueLua.SetVariable("Env_DM_Meter", 20);                            // In environment

        // Other variables //
        DialogueLua.SetVariable("Albert_Death_Type", "Murder");                 // Type of Albert's death
        DialogueLua.SetVariable("Sentence_Length", 0);                          // John's sentence length
        DialogueLua.SetVariable("UseFists", false);                             // Did John use fists or crowbar
        DialogueLua.SetVariable("AlbertHidden", false);                         // Did John hide Albert or leave him
    
    
    
    }
}
