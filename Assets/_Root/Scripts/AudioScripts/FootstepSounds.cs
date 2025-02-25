using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepSounds : MonoBehaviour
{
    public string whatMaterial;
    public float footstepSpeed;
    private bool playingFootsteps;
    private PlayerGridController PlayerGridController;

    /// <summary>
    /// Idk why this isnt working, Adam found a workaround for now.
    /// </summary>

    //public void switchFootMat()
    //{
    //    // Default to wood
    //    if (whatMaterial != null) { AkSoundEngine.SetSwitch("FootstepMaterial", "Wood", gameObject); }

    //    // Set switch in Wwise 
    //    AkSoundEngine.SetSwitch("FootstepMaterial", whatMaterial, gameObject);

    //}

    //private void Update()
    //{
    //    if (PlayerGridController._movement.m_IsMoving) { startFootsteps(); }
    //    else { stopFootsteps(); }
    //}

    //public void startFootsteps()
    //{
    //    playingFootsteps = true;
    //    InvokeRepeating("postFootstep", 0f, footstepSpeed);
    //}

    //private void stopFootsteps()
    //{
    //    playingFootsteps = false;
    //    CancelInvoke(nameof(postFootstep));
    //}

    // This gets called every 3 frames on the albert animation for now
    private void postFootstep()
    {
        AkSoundEngine.PostEvent("Footstep", gameObject);
    }
}
