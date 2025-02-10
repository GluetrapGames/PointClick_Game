using PixelCrushers.DialogueSystem;
using UnityEngine;

public class ItemCollectionChangeVariable : MonoBehaviour
{
	[Tooltip("Apply the game object's pick up script.")]
	public PickUpScript pickUpScript;

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
	private void Update()
	{
		isCollected = pickUpScript.m_ActivateVariable;

		if (isCollected)
		{
			if (isBool) DialogueLua.SetVariable(variableName, boolToggle);
			else if (isFloat) DialogueLua.SetVariable(variableName, floatValue);
			else if (isInt) DialogueLua.SetVariable(variableName, intValue);
			gameObject.SetActive(false);
		}
	}
}