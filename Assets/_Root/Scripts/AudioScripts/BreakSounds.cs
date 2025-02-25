using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BreakSounds : MonoBehaviour
{
    public string itemType;

    public void postBreakMat() 
    {
        // Default to wood
        if (itemType != null) { AkSoundEngine.SetSwitch("BreakMaterial", "Wood", gameObject); }

        // Set material switch within Wwise (Theres definitely a better way to do this)
        AkSoundEngine.SetSwitch("BreakMaterial", itemType, gameObject);

        // Post event
        AkSoundEngine.PostEvent("break_mat", gameObject); 
    }

    // Specific Objects (Objects that need multiple material types)
    public void postBugShelf() { AkSoundEngine.PostEvent("bug_shelf", gameObject); }

    public void postTaxidermy() { AkSoundEngine.PostEvent("taxi_animal", gameObject); }
}
