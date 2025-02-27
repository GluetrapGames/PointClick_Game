using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
	[SerializeField]
	private Animator _crossfadeAnimator;

	public void PlayGame()
	{
		StartCoroutine(LoadScene("Hallway1"));
		AkSoundEngine.StopAll();
	}

	public void QuitGame()
	{
		Debug.LogWarning("Game quit");
		Application.Quit();
	}

	public void ReturnToMainMenu()
	{
		Time.timeScale = 1f;
		SceneManager.LoadScene(0);
	}

	private IEnumerator LoadScene(string sceneName)
	{
		_crossfadeAnimator.SetTrigger("Start");
		yield return new WaitForSeconds(1);
		SceneManager.LoadScene(sceneName);
	}
}