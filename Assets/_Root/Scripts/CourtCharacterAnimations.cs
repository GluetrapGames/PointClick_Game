using GlueTrap;
using UnityEngine;
using UnityEngine.Video;

public class CourtCharacterAnimations : MonoBehaviour
{
	public string animationName = "PlayAnimation";

	private Animator _Animator;
	private VideoPlayer _VideoPlayer;

	private void Awake()
	{
		_Animator = GetComponent<Animator>();
		// Ensure that the Animator is properly set in the Inspector
		if (!_Animator) Debug.LogError("Animator reference is not assigned!");
		_VideoPlayer = FindFirstObjectByType<TitleCard>()
			.GetComponent<VideoPlayer>();
	}

	private void Update()
	{
		// If non are true, try to continually grab them.
		//if (!animator || !image || !_VideoPlayer) Awake();

		//	On TitleCard end, play the desired animation if not already playing.
		if (_VideoPlayer.targetCameraAlpha <= 0.001f && !_Animator
			    .GetCurrentAnimatorStateInfo(0).IsName(animationName))
		{
			// Trigger the animation
			_Animator.Play(animationName);
		}
	}
}