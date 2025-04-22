using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GlueTrap
{
public class BedEndingTracker : MonoBehaviour
{
	[SerializeField]
	private string _NumberVariableName;
	[SerializeField]
	private int _NumberVariableValue;
	private bool _HasInteracted;

	private InteractDialgoue _InteractDialogue;

	private void Awake()
	{
		_InteractDialogue = GetComponent<InteractDialgoue>();
	}

	private void Start()
	{
		_NumberVariableValue =
			DialogueLua.GetVariable(_NumberVariableName).AsInt;
	}

	private void Update()
	{
		if (_NumberVariableValue >= 5)
			SceneManager.LoadScene("CourtScene 4");

		if (!_InteractDialogue.m_Interacting)
		{
			if (_HasInteracted) _HasInteracted = false;
			return;
		}

		if (!_HasInteracted)
		{
			_NumberVariableValue++;
			DialogueLua.SetVariable(_NumberVariableName, _NumberVariableValue);
			_HasInteracted = true;
		}
	}
}
}