using PixelCrushers.DialogueSystem;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace GlueTrap
{
public class ToggleLog : MonoBehaviour
{
	[Tooltip("The portrait image's animator."), SerializeField]
	private Animator _PortraitAnimator;
	[Tooltip("The backlog window."), SerializeField]
	private GameObject _LogWindow;
	[Tooltip("The dialogue system controller script."), SerializeField]
	private DialogueSystemController _dsController;
	[Tooltip("The back log example script."), SerializeField]
	private BackLogExample _blExample;
	[Tooltip("The UI continue button"), SerializeField]
	private Button _ContinueButton;
	[Tooltip("The UI auto play button"), SerializeField]
	private Button _AutoButton;
	[Tooltip("The UI skip button"), SerializeField]
	private Button _SkipButton;
	[Tooltip("The response panel"), SerializeField]
	private GameObject _ResponsePanel;

	private Text buttonText;
	private bool toggleLog;
	private bool _IsPlayerListener;

	private void Start()
	{
		buttonText = gameObject.GetComponentInChildren<Text>();
	}

	private void Update()
	{
		// Current fix - If autoplay is active and log panel opens, current subtitle still hides at the end
		// and the continue button appears. This line should prevent that for now.
		if (toggleLog) _ContinueButton.gameObject.SetActive(false);
	}

	void OnConversationLine(Subtitle subtitle) 
	{
		// Check if the player is the listener on the current subtitle.
		_IsPlayerListener = subtitle.listenerInfo.IsPlayer;
	}


	public void toggle()
	{
		toggleLog = !toggleLog;

		if (toggleLog) // When the backlog is showing
		{
			if(_IsPlayerListener)
				_ResponsePanel.SetActive(false);
			// Stops the speed of the current portrait animation.
			_PortraitAnimator.speed = 0f;
			// Set the button's display text
			buttonText.text = "Exit";
			// Hide all other buttons
			_ContinueButton.gameObject.SetActive(false);
			_AutoButton.gameObject.SetActive(false);
			_SkipButton.gameObject.SetActive(false);
			// Turns off autoplay
			DialogueManager.displaySettings.subtitleSettings.continueButton =
				DisplaySettings.SubtitleSettings.ContinueButtonMode.Always;
			// Pauses the pauses the dialogue system
			_dsController.Pause();
			// Opens and displays the dialogue history
			_blExample.ShowBackLog();
			_blExample.OpenLogWindow();
		}
		else // When the backlog is hidden
		{
            if (_IsPlayerListener)
                _ResponsePanel.SetActive(true);
            // Returns the speed of the current portrait animation.
            _PortraitAnimator.speed = 1.0f;
			// Set the button's display text
			buttonText.text = "History";
			// Show all other buttons
			_ContinueButton.gameObject.SetActive(true);
			_AutoButton.gameObject.SetActive(true);
			_SkipButton.gameObject.SetActive(true);
			// Unpauses the dialogue system
			_dsController.Unpause();
			// Hides the backlog window
			_LogWindow.SetActive(false);
		}
	}
}
}