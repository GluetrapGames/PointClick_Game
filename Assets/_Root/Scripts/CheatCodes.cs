using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GlueTrap
{
public class CheatCodes : MonoBehaviour
{
	// Update is called once per frame
	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.F))
		{
			SceneManager.LoadScene("DownstairsHallway");
			AkSoundEngine.StopAll();
		}

		if (Input.GetKeyUp(KeyCode.G))
		{
			SceneManager.LoadScene("Hallway1");
            AkSoundEngine.StopAll();
        }

		if (Input.GetKeyUp(KeyCode.H))
		{
			GameObject egt = GameObject.Find("EndGameTracker");
			var egts = egt.GetComponent<EndGameTracker>();
			egts._IsGameOver = true;
            AkSoundEngine.StopAll();

            if (SceneManager.GetActiveScene().name != "DownstairsHallway")
				SceneManager.LoadScene("DownstairsHallway");
		}

		if (Input.GetKeyUp(KeyCode.J))
		{
			DialogueLua.SetVariable("Clues_Found", 3);
			DialogueLua.SetVariable("Money_Collected", true);
			SceneManager.LoadScene("DownstairsHallway");
			DialogueManager.StartConversation("Jack_PhoneCall");
            AkSoundEngine.StopAll();
        }
	}
}
}