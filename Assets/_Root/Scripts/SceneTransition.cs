using System.Collections;
using EditorAttributes;
using GlueTrap.Utilities;
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
		// Find the Cross-fade animation from one the children.
		var parentChildren =
			transform.parent.GetComponentsInChildren<Transform>();
		foreach (Transform child in parentChildren)
		{
			_crossfadeAnimator = child.GetComponent<Animator>();
			if (_crossfadeAnimator) return;
		}
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

	public void CallFromConversationEnd()
	{
		StartCoroutine(LoadScene(_sceneToTransitionTo));
	}

	private IEnumerator LoadScene(string sceneName)
	{
		_crossfadeAnimator.SetTrigger("Start");
		yield return new WaitForSeconds(1);
		SceneManager.LoadScene(sceneName);
	}
}
}