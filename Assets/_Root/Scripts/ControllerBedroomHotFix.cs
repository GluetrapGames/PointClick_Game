using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GlueTrap
{
public class ControllerBedroomHotFix : MonoBehaviour
{
	[SerializeField]
	private InteractDialgoue _BedInteractDialogue;

	// Update is called once per frame
	private void Update()
	{
		if (Gamepad.current == null) return;

		_BedInteractDialogue.enabled =
			DialogueLua.GetVariable("Money_Collected").AsBool;
	}
}
}