using UnityEngine;

public class FootstepSounds : MonoBehaviour
{
	public string whatMaterial;
	public float footstepSpeed;
	[SerializeField]
	private GameObject Player;
	private GridMovement PlayerGridController;
	private bool playingFootsteps;

	/// <summary>
	///     Idk why this isnt working, Adam found a workaround for now.
	/// </summary>
	private void Awake()
	{
		PlayerGridController = Player.GetComponent<GridMovement>();
	}

	private void Update()
	{
		if (PlayerGridController.m_IsMoving)
		{
			if (!playingFootsteps) startFootsteps();
		}
		else
		{
			if (playingFootsteps) stopFootsteps();
		}
	}


	public void switchFootMat()
	{
		// Default to wood
		if (whatMaterial != null)
			AkSoundEngine.SetSwitch("FootstepMaterial", "Wood", gameObject);

		// Set switch in Wwise 
		AkSoundEngine.SetSwitch("FootstepMaterial", whatMaterial, gameObject);
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

	private void postFootstep()
	{
		AkSoundEngine.PostEvent("Footstep", gameObject);
	}
}