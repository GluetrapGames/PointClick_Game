using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class ToggleLog : MonoBehaviour
{
    [SerializeField,Tooltip("The portrait image's animator.")]
    private Animator _PortraitAnimator;
    [SerializeField,Tooltip("The backlog window.")]
    private GameObject _LogWindow;
    [SerializeField,Tooltip("The dialogue system controller script.")]
    private DialogueSystemController _dsController;
    [SerializeField,Tooltip("The back log example script.")]
    private BackLogExample _blExample;
    [SerializeField,Tooltip("The UI continue button")]
    private Button _ContinueButton;
    [SerializeField,Tooltip("The UI auto play button")]
    private Button _AutoButton;
    [SerializeField,Tooltip("The UI skip button")]
    private Button _SkipButton;
    [SerializeField, Tooltip("The response menu panel")]
    private GameObject _ResponseMenuPanel;

    private Text buttonText;
    private bool toggleLog;
    private bool isPlayerListening;

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
        isPlayerListening = subtitle.listenerInfo.isPlayer;
    }

    public void toggle() 
    {
        toggleLog = !toggleLog;

        if (toggleLog) // When the backlog is showing
        {
            // Hides the response panel if it is currently active.
            if(isPlayerListening)
                _ResponseMenuPanel.SetActive(false);
            // Stops the speed of the current portrait animation.
            _PortraitAnimator.speed = 0f;
            // Set the button's display text
            buttonText.text = "Exit";
            // Hide all other buttons
            _ContinueButton.gameObject.SetActive(false); 
            _AutoButton.gameObject.SetActive(false);
            _SkipButton.gameObject.SetActive(false);
            // Turns off autoplay
            DialogueManager.displaySettings.subtitleSettings.continueButton = DisplaySettings.SubtitleSettings.ContinueButtonMode.Always;
            // Pauses the pauses the dialogue system
            _dsController.Pause();
            // Opens and displays the dialogue history
            _blExample.ShowBackLog();
            _blExample.OpenLogWindow();
        }
        else // When the backlog is hidden
        {
            if (isPlayerListening)
                _ResponseMenuPanel.SetActive(true);
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


