using System.Collections;
using System.Linq;
using EditorAttributes;
using GlueTrap.Utilities;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GlueTrap
{
public class SceneTransition : MonoBehaviour
{
	public InteractionDir m_InteractionDirection = InteractionDir.Bottom;
	public float m_OffsetAmount = 1.5f;
	[Space, Tooltip("What type of entry is it.")]
	public RoomEntryPoints m_EntryPoint = RoomEntryPoints.None;
	[Tooltip("To what point does it leads to.")]
	public RoomEntryPoints m_ExitPoint = RoomEntryPoints.None;
	public bool m_increaseAlpha;

	[SerializeField]
	private bool _Log;
	[SerializeField, SceneDropdown]
	private string _sceneToTransitionTo;

	private Animator _crossfadeAnimator;
	private GameManager _GameManager;
	private bool _isPlaying;
	private LockedRoom _lockedRoom;


	private void Awake()
	{
		_GameManager = Utils.GetGameManager();

		if (!transform.parent) return;
		// Search itself, its parent, and all of its children's components.
		GameObject targetObject = GameObject.Find("----SceneTransisitions----");

		_lockedRoom = GetComponent<LockedRoom>();

		if (targetObject)
		{
			// Search itself and all of its children for the Animator component
			_crossfadeAnimator = targetObject.GetComponent<Animator>();
			if (_crossfadeAnimator) return;

			// Search all children of the target object.
			var children = targetObject.GetComponentsInChildren<Transform>();
			foreach (Transform child in children)
			{
				_crossfadeAnimator = child.GetComponent<Animator>();
				if (_crossfadeAnimator) return;
			}
		}

		// If the target object is not found or no Animator is found, proceed parent.
		_crossfadeAnimator = transform.parent.GetComponent<Animator>();
		if (_crossfadeAnimator) return;
	}

	private void OnEnable()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnDisable()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (other.CompareTag("Feet") && _GameManager.m_HasEntered &&
		    !_isPlaying)
			_GameManager.m_HasEntered = false;
	}

	private void OnTriggerStay2D(Collider2D other)
	{
		if (!other.CompareTag("Feet") /*|| _GameManager.m_HasEntered*/) return;
		//_GameManager.m_HasEntered = true;
		_GameManager.m_RoomPoint = m_ExitPoint;

		if (_lockedRoom)
		{
			if (!_lockedRoom.hasKey)
			{
				DialogueManager.StartConversation("Door_Locked");
				StartCoroutine(colliderCooldown());
				return;
			}
		}

		switch (_sceneToTransitionTo)
		{
			// Upstairs checks
			case "CourtScene 3" when !_GameManager.m_hasCrowbar:
				DialogueManager.StartConversation("NoCrowbar");
				return;
			case "CourtScene 3" when _GameManager.m_hasUpstairsCourt:
			{
				StartCoroutine(LoadScene("Hallway1"));
				if (_isPlaying) return;
				AkSoundEngine.PostEvent("RoomTransition", gameObject);
				_isPlaying = true;
				return;
			}
			default:
				StartCoroutine(LoadScene(_sceneToTransitionTo));
				// Play scene transition sound
				if (_isPlaying) return;
				AkSoundEngine.PostEvent("RoomTransition", gameObject);
				_isPlaying = true;
				break;
		}
	}

	public void CallFromConversationEnd()
	{
		if (m_increaseAlpha)
		{
			var bgRef = GameObject.FindGameObjectWithTag("BG")
				.GetComponent<BackgroundMania>();
			if (!bgRef)
			{
				Debug.LogError("Can't find BG!");
				return;
			}

			bgRef.updateAlpha();
		}

		StartCoroutine(LoadScene(_sceneToTransitionTo));
	}

	private IEnumerator colliderCooldown()
	{
		var collider = GetComponent<BoxCollider2D>();
		yield return new WaitUntil(() => !DialogueManager.isConversationActive);
		collider.enabled = false;
		yield return new WaitForSeconds(3);
		collider.enabled = true;
	}

	private IEnumerator LoadScene(string sceneName)
	{
		_crossfadeAnimator.SetTrigger("Start");
		yield return new WaitForSeconds(1);
		if (sceneName == "CourtScene 3") _GameManager.m_hasUpstairsCourt = true;
		UniqueRoomCheck();
		if (_Log) Debug.Log($"Loading to a new Scene {sceneName}");
		if (_GameManager.m_NoneGameplayScenes.Contains(sceneName))
		{
			if (sceneName == "MenuScene") yield break;
			_GameManager.GetComponent<CourtsceneControllerSupport>().SetSelectedButton();
		}
		SceneManager.LoadScene(sceneName);
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		var objs = FindObjectsByType<SceneTransition>(FindObjectsSortMode.None);
		foreach (SceneTransition obj in objs)
			if (obj.m_EntryPoint == _GameManager.m_RoomPoint &&
			    obj.m_ExitPoint != RoomEntryPoints.None)
			{
				Vector3 newPos = obj.transform.position;
				newPos.z = 9.99f;

				// Apply directional offset.
				switch (obj.m_InteractionDirection)
				{
					case InteractionDir.Top:
						newPos += Vector3.up * m_OffsetAmount;
						break;
					case InteractionDir.Bottom:
						newPos += Vector3.down * m_OffsetAmount;
						break;
					case InteractionDir.Left:
						newPos += Vector3.left * m_OffsetAmount;
						break;
					case InteractionDir.Right:
						newPos += Vector3.right * m_OffsetAmount;
						break;
				}

				_GameManager.m_Player.SetPositionInGrid(newPos);
				break;
			}

		_isPlaying = false;
		_GameManager.m_Player.m_DestinationReached = false;
		
	}

	private void UniqueRoomCheck()
	{
		if (!_GameManager.m_UniqueRoomList.Contains(_sceneToTransitionTo))
		{
			_GameManager.m_UniqueRoomList.Add(_sceneToTransitionTo);
			DialogueLua.SetVariable("Rooms_Entered",
				DialogueLua.GetVariable("Rooms_Entered").asInt + 1);
		}
	}
}
}