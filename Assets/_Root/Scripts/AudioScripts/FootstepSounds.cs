using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FootstepSounds : MonoBehaviour
{
    public string whatMaterial;
    public float footstepSpeed;
    private bool playingFootsteps;
    [SerializeField]
    private GameObject Player;
    private GridMovement PlayerGridController;

    /// <summary>
    /// Idk why this isnt working, Adam found a workaround for now.
    /// </summary>
    private void Awake()
    {
        PlayerGridController = Player.GetComponent<GridMovement>();
    }


    public void switchFootMat()
    {
        // Default to wood
        if (whatMaterial != null) { AkSoundEngine.SetSwitch("FootstepMaterial", "Wood", gameObject); }

        // Set switch in Wwise 
        AkSoundEngine.SetSwitch("FootstepMaterial", whatMaterial, gameObject);

    }

    private void Update() 
    {
        if (PlayerGridController.m_IsMoving)
        {
            if (!playingFootsteps)
            {
                startFootsteps();
            }

        if (PlayerGridController.m_IsMoving == false)
            {
                if (playingFootsteps)
                {
                    stopFootsteps();
                }
            }
        }
    }

    public void startFootsteps()
    {
        playingFootsteps = true;
        InvokeRepeating("postFootstep", 0f, footstepSpeed);
        Debug.Log("Started Footstep");
    }

    private void stopFootsteps()
    {
        playingFootsteps = false;
        CancelInvoke("postFootstep");
        Debug.Log("Stopped Footstep");
    }

    // This gets called every 3 frames on the albert animation for now
    private void postFootstep()
    {
        AkSoundEngine.PostEvent("Footstep", gameObject);
    }
}
