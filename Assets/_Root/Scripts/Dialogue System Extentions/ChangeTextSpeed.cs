using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeTextSpeed : MonoBehaviour
{
    public float typeSpeed;
    public GameObject subtitleText; // The subtitle text gameobject

    private UnityUITypewriterEffect typewriterEffect; // The custom typewriter script
    private int currentMultiplier = 1;
    private Text buttonText; // This buttons text child

    void Start()
    {
        typeSpeed = 50.0f; // Default type speed
        currentMultiplier = 1; // Default multiplier

        buttonText = gameObject.GetComponentInChildren<Text>(); // Get this gameobject's text child
        typewriterEffect = subtitleText.GetComponentInChildren<UnityUITypewriterEffect>(); // Get the typewriter component of the subtitle text.

        typeSpeed = typewriterEffect.GetSpeed();
        buttonText.text = "1x";

    }

    // Increases the speed of the typewriter effect
    public void IncreaseTextSpeed()
    {
        currentMultiplier++; // Increment multiplier first

        if (currentMultiplier > 3) // Reset to 1 after reaching 3x
        {
            currentMultiplier = 1;
        }

        buttonText.text = currentMultiplier + "x"; // Update button text
        typewriterEffect.charactersPerSecond = typeSpeed * currentMultiplier; // Apply new speed
    }

    // Default function sets variables back to default values.
    private void SetDefaultTextSpeed()
    {
        typeSpeed = typewriterEffect.GetSpeed();
        currentMultiplier = 1;
        buttonText.text = "1x";
    }
}