using PixelCrushers.DialogueSystem;
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
	// Image child may be removed
    [Tooltip("The Image child"), SerializeField]
    private GameObject _ChildImage;    
	[Tooltip("The Pause button on the screen"), SerializeField]
    private GameObject _ScreenPauseButton;
    [Tooltip("The Log button on the screen"), SerializeField]
    private GameObject _ScreenLogButton;

    
	// Changes the dialogue panel log button image but this may be removed
	// in favoour of having the close button on the log panel itself.
	[SerializeField,Tooltip("The BACKLOG image ")]
	private Sprite _OpenImage;
    [SerializeField, Tooltip("The CLOSE image")]
    private Sprite _CloseImage;

	private bool toggleLog;
	private bool _IsPlayerListener;
	private Image _ButtonImage;

	private void Awake()
	{
		_ButtonImage = _ChildImage.GetComponent<Image>();
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
			if (_ScreenLogButton != null) { _ScreenLogButton.SetActive(false); }
			if (_ScreenPauseButton != null) { _ScreenPauseButton.SetActive(false); }

			// Only do this if a conversation is open.
			if (DialogueManager.IsConversationActive) 
			{ 
				// Hide the player response panel
				if (_IsPlayerListener) _ResponsePanel.SetActive(false);
				// Stops the speed of the current portrait animation.
				_PortraitAnimator.speed = 0f;
				// Hide all other buttons
				_ContinueButton.gameObject.SetActive(false);
				_AutoButton.gameObject.SetActive(false);
				_SkipButton.gameObject.SetActive(false);
				// Change the button's image
				_ButtonImage.sprite = _CloseImage;
				// Turns off autoplay
				DialogueManager.displaySettings.subtitleSettings.continueButton =
				DisplaySettings.SubtitleSettings.ContinueButtonMode.Always;
				// Pauses the dialogue system
				_dsController.Pause();
			}
			
			// Opens and displays the dialogue history
			_blExample.ShowBackLog();
			_blExample.OpenLogWindow();
		}
		else // When the backlog is hidden
		{
            if (_ScreenLogButton != null) { _ScreenLogButton.SetActive(true); }
            if (_ScreenPauseButton != null) { _ScreenPauseButton.SetActive(true); }

            // Only do this if a conversation is open.
            if (DialogueManager.IsConversationActive)
			{
				// Show the player response panel
				if (_IsPlayerListener) _ResponsePanel.SetActive(true);
				// Returns the speed of the current portrait animation.
				_PortraitAnimator.speed = 1.0f; ;
				// Show all other buttons
				_ContinueButton.gameObject.SetActive(true);
				_AutoButton.gameObject.SetActive(true);
				_SkipButton.gameObject.SetActive(true);
				// Change the button's image
				_ButtonImage.sprite = _OpenImage;
				// Unpauses the dialogue system
				_dsController.Unpause();
			}
			
			// Hides the backlog window
			_LogWindow.SetActive(false);
		}
	}
}
}