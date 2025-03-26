using System.Collections.Generic;
using DG.Tweening;
using EditorAttributes;
using GlueTrap.Utilities;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.Video;

namespace GlueTrap
{
[RequireComponent(typeof(VideoPlayer))]
public class TitleCard : MonoBehaviour
{
	[Tooltip("Conversation to start. Leave blank for no conversation."),
	 ConversationPopup(false, true)]
	public string conversation = string.Empty;

	[Tooltip(
		"Primary actor (e.g., player). If unassigned, GameObject that triggered conversation.")]
	public Transform conversationActor;

	[Tooltip("Other actor (e.g., NPC). If unassigned, this GameObject.")]
	public Transform conversationConversant;

	[SerializeField, PropertyOrder(-1)]
	private bool _Log;
	[SerializeField, PropertyOrder(-1), EnableField(nameof(_Log)),
	 IndentProperty]
	private bool _LogClipLengths;
	[SerializeField]
	private List<GameObject> _ObscuringObjects = new();
	[SerializeField]
	private bool _UseFadeOut;
	[SerializeField, EnableField(nameof(_UseFadeOut))]
	private long _StatingFrameFadeOutTime;
	[SerializeField, EnableField(nameof(_UseFadeOut))]
	private float _FadeSpeed = 1f;
	private GameManager _GameManager;

	private bool _HandledVideoFinish;
	private bool _StartedFadeOut;

	public VideoPlayer m_VideoPlayer { get; private set; }
	public bool m_IsPlaying { get; private set; } = true;


	private void Awake()
	{
		DOTween.SetTweensCapacity(3125, 50);

		_GameManager = Utils.GetGameManager();
		m_VideoPlayer = GetComponent<VideoPlayer>();

		if (!m_VideoPlayer)
			Debug.LogError($"{this}: Missing VideoPlayer component!");
	}

	private void Start()
	{
		_GameManager.ChangeGameState(States.InMenus);

		if (_ObscuringObjects.Count <= 0) return;
		foreach (GameObject obscuringObject in _ObscuringObjects)
			obscuringObject.SetActive(false);
	}

	private void Update()
	{
		Log();
	}

	private void LateUpdate()
	{
		if (!m_VideoPlayer)
			return;

		// Do a fade transition on the video.
		if (_UseFadeOut && !_StartedFadeOut &&
		    m_VideoPlayer.frame >= _StatingFrameFadeOutTime)
		{
			_StartedFadeOut = true;
			PlayFadeOut();
		}

		if (m_VideoPlayer.frame < (long)m_VideoPlayer.frameCount - 1)
			return;

		m_IsPlaying = false;

		if (_HandledVideoFinish) return;
		_HandledVideoFinish = true;

		_GameManager.ChangeGameState(States.Moving);
		// Once video finished playing, start a conversation.
		if (DialogueManager.instance)
		{
			// Start conversation when video finishes.
			DialogueManager.instance.StartConversation(conversation,
				conversationActor, conversationConversant);
		}
		else
			Debug.LogWarning($"{this}: DialogueManager instance is missing!");


		if (_ObscuringObjects.Count <= 0) return;
		foreach (GameObject obscuringObject in _ObscuringObjects)
			obscuringObject.SetActive(true);
	}

	private void Log()
	{
		if (!_Log) return;

		if (_LogClipLengths)
		{
			// Round to 3 decimal places.
			var roundedLength =
				Mathf.Round((float)(m_VideoPlayer.clip.length * 1000f)) /
				1000f;
			Debug.Log(
				$"{m_VideoPlayer.clip.name}: {roundedLength}s/" +
				$"{m_VideoPlayer.clip.frameCount} Frames");
		}

		if (_ObscuringObjects.Count <= 0)
			Debug.Log($"{this}: No obscuring objects.");
	}

	private void PlayFadeOut()
	{
		if (!m_VideoPlayer || m_VideoPlayer.targetCameraAlpha <= 0f)
			return;

		// Calculate remaining time until video ends.
		var timeRemaining =
			(m_VideoPlayer.frameCount - (ulong)m_VideoPlayer.frame) /
			m_VideoPlayer.frameRate;

		// Fade duration should never be longer than the remaining video time.
		var fadeDuration = Mathf.Min(timeRemaining, _FadeSpeed);

		DOTween.To(() => m_VideoPlayer.targetCameraAlpha,
				x => m_VideoPlayer.targetCameraAlpha = x,
				0f, fadeDuration)
			.SetEase(Ease.Linear);
	}
}
}