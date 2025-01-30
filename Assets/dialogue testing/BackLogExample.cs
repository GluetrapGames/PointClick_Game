using System.Collections.Generic;  // Import the namespace for working with collections like List.
using UnityEngine;  // Import Unity's core functionality.
using PixelCrushers.DialogueSystem;  // Import the Pixel Crushers Dialogue System for handling dialogue.

/// <summary>
/// This script runs the back log. It records dialogue lines as they're played.
/// The ShowBackLog() method fills a back log window with the recorded lines.
/// </summary>
public class BackLogExample : MonoBehaviour
{
    // A reference to the container where log entries will be placed in the UI.
    public Transform logEntryContainer;

    // The template used to create a new log entry.
    public LogEntryTemplate logEntryTemplate;

    // Serialized field for the back log window to toggle visibility.
    [SerializeField]
    private GameObject backlogWindow;

    // A list to store dialogue subtitles (recorded lines).
    private List<Subtitle> log = new List<Subtitle>();

    // A list of instantiated log entry GameObjects to manage their lifecycle.
    private List<GameObject> instances = new List<GameObject>();

    // Initialization method. Set the log entry template as inactive at the start.
    private void Awake()
    {
        logEntryTemplate.gameObject.SetActive(false);
    }

    /// <summary>
    /// This method is called when a conversation line is received.
    /// It checks if the subtitle has any text and adds it to the log.
    /// </summary>
    public void OnConversationLine(Subtitle subtitle)
    {
        if (!string.IsNullOrEmpty(subtitle.formattedText.text))  // Only add subtitles with text.
        {
            log.Add(subtitle);  // Add the subtitle to the log.
        }
    }

    /// <summary>
    /// This method displays the back log window with the recorded dialogue lines.
    /// It creates a new UI entry for each line and fills it with the appropriate subtitle.
    /// </summary>
    public void ShowBackLog()
    {
        // Destroy any previously instantiated log entries.
        instances.ForEach(instance => Destroy(instance));
        instances.Clear();

        // Reverse the log so that the newest entries are shown first.
        List<Subtitle> reversedLog = new List<Subtitle>(log);
        reversedLog.Reverse();

        // Instantiate new log entries for each subtitle in the reversed log.
        foreach (Subtitle subtitle in reversedLog)
        {
            var instance = Instantiate(logEntryTemplate, logEntryContainer);  // Create a new log entry.
            instances.Add(instance.gameObject);  // Add the instance to the list of instances.
            instance.gameObject.SetActive(true);  // Activate the log entry in the scene.
            instance.Assign(subtitle);  // Assign the subtitle text to the log entry.
        }
    }

    /// <summary>
    /// This method opens the back log window when called.
    /// </summary>
    public void OpenLogWindow()
    {
        backlogWindow.SetActive(true);  // Enable the back log window.
    }
}