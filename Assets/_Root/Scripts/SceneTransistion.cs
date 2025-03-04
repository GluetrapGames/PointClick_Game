using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GlueTrap
{
public class SceneTransistion : MonoBehaviour
{
	[SerializeField]
	private string _sceneToTransitionTo;

	[SerializeField]
	private Animator _crossfadeAnimator;

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