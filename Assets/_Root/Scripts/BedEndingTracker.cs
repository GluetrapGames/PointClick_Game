using PixelCrushers.DialogueSystem;
using UnityEngine;

namespace GlueTrap
{
public class BedEndingTracker : MonoBehaviour
{
	[SerializeField]
	private string _BedInteractionVariableName;
	[SerializeField]
	private int _NumberOfBedInteractions;
	[SerializeField]
	private SceneTransition _SceneTransition;

	private bool _HasInteracted;
	private InteractDialgoue _InteractDialogue;


	private void Awake()
	{
		_InteractDialogue = GetComponent<InteractDialgoue>();
	}

	private void Start()
	{
		// Keep track of any already existed interactions.
		_NumberOfBedInteractions =
			DialogueLua.GetVariable(_BedInteractionVariableName).AsInt;
	}

	private void Update()
	{
		// If the required number of bed interaction is met and the isn't a
		// active conversation, load to the last Court scene.
		if (_NumberOfBedInteractions >= 5 &&
		    !DialogueManager.instance.isConversationActive)
			_SceneTransition.CallFromConversationEnd();

		// Only continue if we interacted with the object.
		if (!_InteractDialogue.m_Interacting)
		{
			if (_HasInteracted) _HasInteracted = false;
			return;
		}

		// Only run this code once.
		if (!_HasInteracted)
		{
			_NumberOfBedInteractions++;
			DialogueLua.SetVariable(_BedInteractionVariableName,
				_NumberOfBedInteractions);
			_HasInteracted = true;
		}
	}
}
}