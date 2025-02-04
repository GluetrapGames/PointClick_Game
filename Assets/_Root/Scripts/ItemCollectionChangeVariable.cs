using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;
using Language.Lua;
using PixelCrushers.DialogueSystem.Articy.Articy_4_0;
using Unity.VisualScripting;


public class ItemCollectionChangeVariable : MonoBehaviour
{
    [Tooltip("Apply the game object's pick up script.")]
    public pickUpScript pickUpScript;

    [Tooltip("Enter the name of the variable. (CASE SENSITIVE)")]
    public string variableName;

    [Tooltip("Is the variable of type bool?")]
    public bool isBool;
    public bool boolToggle;

    [Tooltip("Is the variable of type float?")]
    public bool isFloat;
    public float floatValue;

    [Tooltip("Is the variable of type int?")]
    public bool isInt;
    public int intValue;

    private bool isCollected;


    // Update is called once per frame
    void Update()
    {
        isCollected = pickUpScript.activateVariable;

        if (isCollected)
        { 
            if(isBool) DialogueLua.SetVariable(variableName, boolToggle);
            else if(isFloat) DialogueLua.SetVariable(variableName, floatValue);
            else if(isInt) DialogueLua.SetVariable(variableName, intValue);
            gameObject.SetActive(false);
        }
    }
}
