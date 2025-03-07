using PixelCrushers.DialogueSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GlueTrap
{
public class PlayButton : MonoBehaviour
{
	[SerializeField]
	private Animator _crossfadeAnimator;

	public void PlayGame()
	{
		StartCoroutine(LoadScene("CourtroomIntro"));
		AkSoundEngine.StopAll();
	}

<<<<<<< HEAD
        StartCoroutine(LoadScene("CourtroomIntro"));
        AkSoundEngine.StopAll();

    }

    public void QuitGame()
    {
        Debug.LogError("Game quit");
        Application.Quit();
    }

    public void ReturnToMainMenu()
    {
        // If a conversation is playing when returning to menu, then stop all conversations.
        if (DialogueManager.IsConversationActive)
            DialogueManager.StopAllConversations();
        //run adam is coming
=======
	public void QuitGame()
	{
		Debug.LogError("Game quit");
		Application.Quit();
	}

	public void ReturnToMainMenu()
	{
		// Check if converssaton is playing and cancel it
		if (DialogueManager.IsConversationActive)
			DialogueManager.StopAllConversations();
>>>>>>> MergeBranch

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
}