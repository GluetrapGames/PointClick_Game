using GlueTrap.Utilities;
using UnityEngine;

namespace GlueTrap
{
public class FootstepSounds : MonoBehaviour
{
	[SerializeField]
	private bool _Log;

	public MaterialTypes whatMaterial;
	public float footstepSpeed;
	private GameManager _GameManager;
	private bool playingFootsteps;


	// Idk why this isnt working, Adam found a workaround for now.
	private void Awake()
	{
		_GameManager = Utils.GetGameManager();
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
}