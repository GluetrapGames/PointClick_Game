using System.Collections;
using EditorAttributes;
using GlueTrap.Utilities;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GlueTrap
{
public class SceneTransition : MonoBehaviour
{
	[Tooltip("What type of entry is it.")]
	public RoomEntryPoints m_EntryPoint = RoomEntryPoints.None;
	[Tooltip("To what point does it leads to.")]
	public RoomEntryPoints m_ExitPoint = RoomEntryPoints.None;

	[SerializeField, SceneDropdown]
	private string _sceneToTransitionTo;

	private Animator _crossfadeAnimator;
	private GameManager _GameManager;
	private bool _isPlaying;


	private void Awake()
	{
		_GameManager = Utils.GetGameManager();

		if (!transform.parent) return;
		// Search itself, its parent, and all of its children's components.
		GameObject targetObject = GameObject.Find("----SceneTransisitions----");

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
		if (other.CompareTag("Feet") && !_GameManager.m_HasEntered)
		{
			_GameManager.m_HasEntered = true;
			_GameManager.m_RoomPoint = m_ExitPoint;
			
			// Upstairs checks
			if (_sceneToTransitionTo == "CourtScene 3" && !_GameManager.m_hasCrowbar)
			{
				DialogueManager.StartConversation("NoCrowbar");
				return;
			}
			if (_sceneToTransitionTo == "CourtScene 3" && _GameManager.m_hasUpstairsCourt) StartCoroutine(LoadScene("Hallway1"));
			
			StartCoroutine(LoadScene(_sceneToTransitionTo));

			// Play scene transition sound
			if (_isPlaying) return;
			AkSoundEngine.PostEvent("RoomTransition", gameObject);
			_isPlaying = true;
		}
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
				_GameManager.m_Player.SetPositionInGrid(newPos);
				break;
			}

		_isPlaying = false;
	}

	private void UniqueRoomCheck()
	{
		if (!_GameManager.m_UniqueRoomList.Contains(_sceneToTransitionTo.ToString()))
		{
			_GameManager.m_UniqueRoomList.Add(_sceneToTransitionTo.ToString());
			DialogueLua.SetVariable("Rooms_Entered", (DialogueLua.GetVariable("Rooms_Entered").asInt + 1));
		}
	}
	
	public void CallFromConversationEnd()
	{
		StartCoroutine(LoadScene(_sceneToTransitionTo));
	}

	private IEnumerator LoadScene(string sceneName)
	{
		_crossfadeAnimator.SetTrigger("Start");
		yield return new WaitForSeconds(1);
		if(sceneName == "CourtScene 3") _GameManager.m_hasUpstairsCourt = true;
		UniqueRoomCheck();
		SceneManager.LoadScene(sceneName);
	}
}
}