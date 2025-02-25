using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BreakSounds : MonoBehaviour
{
    public string itemType;

    public void postBreakMat() 
    { 
        // Set material switch within Wwise (Theres definitely a better way to do this)
        if (itemType == "Wood")
        {
            AkSoundEngine.SetSwitch("Material", "Wood", gameObject);
        }
        if (itemType == "Metal")
        {
            AkSoundEngine.SetSwitch("Material", "Metal", gameObject);
        }
        if (itemType == "Plant")
        {
            AkSoundEngine.SetSwitch("Material", "Plant", gameObject);
        }
        if (itemType == "Glass")
        {
            AkSoundEngine.SetSwitch("Material", "Glass", gameObject);
        }
        if (itemType == "Electronic")
        {
            AkSoundEngine.SetSwitch("Material", "Electronic", gameObject);
        }
        if (itemType == "Ceramic")
        {
            AkSoundEngine.SetSwitch("Material", "Ceramic", gameObject);
        }
        // Post event
        AkSoundEngine.PostEvent("break_mat", gameObject); 
    }

    // Specific Objects (Objects that need multiple material types)
    public void postBugShelf() { AkSoundEngine.PostEvent("bug_shelf", gameObject); }

    public void postTaxidermy() { AkSoundEngine.PostEvent("taxi_animal", gameObject); }
}
