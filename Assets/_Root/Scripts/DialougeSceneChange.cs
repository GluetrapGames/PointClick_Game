using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GlueTrap
{
public class DialougeSceneChange : MonoBehaviour
{
	[SerializeField]
	private Animator _crossfadeAnimator;

	private void SceneChange(string sceneName)
	{
		StartCoroutine(LoadScene(sceneName));
		AkSoundEngine.StopAll();
	}

	private IEnumerator LoadScene(string sceneName)
	{
		_crossfadeAnimator.SetTrigger("Start");
		yield return new WaitForSeconds(1);
		SceneManager.LoadScene(sceneName);
	}
}
}