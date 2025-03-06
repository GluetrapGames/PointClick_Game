using System.Collections;
using EditorAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GlueTrap
{
public class SceneTransistion : MonoBehaviour
{
	[SerializeField, SceneDropdown]
	private string _sceneToTransitionTo;

	private Animator _crossfadeAnimator;


	private void Awake()
	{
		// Find the Cross-fade animation from one the children.
		var parentChildren =
			transform.parent.GetComponentsInChildren<Transform>();
		foreach (Transform child in parentChildren)
		{
			_crossfadeAnimator = child.GetComponent<Animator>();
			if (_crossfadeAnimator) return;
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		StartCoroutine(LoadScene(_sceneToTransitionTo));
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