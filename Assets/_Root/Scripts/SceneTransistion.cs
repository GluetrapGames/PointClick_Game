using System.Collections;
using EditorAttributes;
using GlueTrap.Utilities;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GlueTrap
{
public class SceneTransistion : MonoBehaviour
{
	[SerializeField, SceneDropdown]
	private string _sceneToTransitionTo;
	private GameManager _gameManager;

	private Animator _crossfadeAnimator;
	private bool _hasCrowbar;
	private bool _isPlaying = false;


	private void Awake()
	{
		_gameManager = Utils.GetGameManager();
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

	private void OnTriggerStay2D(Collider2D other)
	{
		if (other.CompareTag("Feet"))
			{
				if (_sceneToTransitionTo.ToString() == "CourtScene 3")
				{
					crowbarCheck();
					return;
				}
                StartCoroutine(LoadScene(_sceneToTransitionTo));

                // Play scene transition sound
                if (!_isPlaying)
                {
                    AkSoundEngine.PostEvent("RoomTransition", gameObject);
                    _isPlaying = true;
                }
            }
			

            
        }

	private void UniqueRoomCheck()
	{
		if (!_gameManager.m_UniqueRoomList.Contains(_sceneToTransitionTo.ToString()))
		{
			_gameManager.m_UniqueRoomList.Add(_sceneToTransitionTo.ToString());
			DialogueLua.SetVariable("Rooms_Entered", (DialogueLua.GetVariable("Rooms_Entered").asInt + 1));
		}
	}

	private void crowbarCheck()
	{
		if (_gameManager.m_hasCrowbar)
		{
			StartCoroutine(LoadScene(_sceneToTransitionTo));
			return;
		}
		else
		{
			DialogueManager.StartConversation("NoCrowbar");
		}
	}
	
	public void CallFromConversationEnd()
	{
		StartCoroutine(LoadScene(_sceneToTransitionTo));
	}

	private IEnumerator LoadScene(string sceneName)
	{
		_crossfadeAnimator.SetTrigger("Start");
		UniqueRoomCheck();
		yield return new WaitForSeconds(1);
		SceneManager.LoadScene(sceneName);
	}
}
}