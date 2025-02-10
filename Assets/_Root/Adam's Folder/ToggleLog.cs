using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleLog : MonoBehaviour
{
    [Tooltip("The backlog window.")]
    public GameObject logWindow;
    [Tooltip("The dialogue system controller script.")]
    public DialogueSystemController dsController;
    [Tooltip("The back log example script.")]
    public BackLogExample blExample;
    [Tooltip("The UI continue button")]
    public Button continueButton;
    [Tooltip("The UI auto play button")]
    public Button autoButton;
    [Tooltip("The UI skip button")]
    public Button skipButton;

    private Text buttonText;
    private bool toggleLog;

    private void Start()
    {
        buttonText = gameObject.GetComponentInChildren<Text>();
    }

    private void Update()
    {
        // Current fix - If autoplay is active and log panel opens, current subtitle still hides at the end
        // and the continue button appears. This line should prevent that for now.
        if (toggleLog) continueButton.gameObject.SetActive(false);
    }


    public void toggle() 
    {
        toggleLog = !toggleLog;

        if (toggleLog) // When the backlog is showing
        {
            // Set the button's display text
            buttonText.text = "Exit";
            // Hide all other buttons
            continueButton.gameObject.SetActive(false); 
            autoButton.gameObject.SetActive(false);
            skipButton.gameObject.SetActive(false);
            // Turns off autoplay
            DialogueManager.displaySettings.subtitleSettings.continueButton = DisplaySettings.SubtitleSettings.ContinueButtonMode.Always;
            // Pauses the pauses the dialogue system
            dsController.Pause();
            // Opens and displays the dialogue history
            blExample.ShowBackLog();
            blExample.OpenLogWindow();
        }
        else // When the backlog is hidden
        {
            // Set the button's display text
            buttonText.text = "History";
            // Show all other buttons
            continueButton.gameObject.SetActive(true);
            autoButton.gameObject.SetActive(true);
            skipButton.gameObject.SetActive(true);
            // Unpauses the dialogue system
            dsController.Unpause();
            // Hides the backlog window
            logWindow.SetActive(false);
        }
    }
}


