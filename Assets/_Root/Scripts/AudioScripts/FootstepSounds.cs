using UnityEngine;

public class FootstepSounds : MonoBehaviour
{
	[SerializeField]
	private bool _Log;

	public string whatMaterial;
	public float footstepSpeed;
	private GameManager _GameManager;
	private bool playingFootsteps;


	// Idk why this isnt working, Adam found a workaround for now.
	private void Awake()
	{
		_GameManager = FindFirstObjectByType<GameManager>();
	}

	private void Update()
	{
		if (_GameManager.m_Player.m_Movement.m_IsMoving)
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
		if (_Log) Debug.Log("Started Footstep");
	}

	private void stopFootsteps()
	{
		playingFootsteps = false;
		CancelInvoke("postFootstep");
		if (_Log) Debug.Log("Stopped Footstep");
	}

	private void postFootstep()
	{
		AkSoundEngine.PostEvent("Footstep", gameObject);
	}
}