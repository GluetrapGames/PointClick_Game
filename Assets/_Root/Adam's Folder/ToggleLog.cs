using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleLog : MonoBehaviour
{
    public bool toggleLog;
    public Canvas logWindow;
    public DialogueSystemController dsController;
    public BackLogExample blExample;

    private Text buttonText;
    private void Start()
    {
        buttonText = gameObject.GetComponentInChildren<Text>();

    }

    // Update is called once per frame
    void Update()
    {
        if (toggleLog)
        {
            buttonText.text = "Exit";
            dsController.Pause();
            blExample.ShowBackLog();
            blExample.OpenLogWindow();
        }
        else 
        {
            buttonText.text = "History";
            dsController.Unpause();
            blExample.gameObject.SetActive(false);
        }
    }

    public void toggle() { toggleLog = !toggleLog; }
}


