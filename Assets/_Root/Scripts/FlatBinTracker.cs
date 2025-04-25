using GlueTrap.Utilities;
using PixelCrushers.DialogueSystem;
using UnityEngine;

namespace GlueTrap
{
public class FlatBinTracker : MonoBehaviour
{
	private BreakableItem _BreakableItem;

	private void Awake()
	{
		_BreakableItem = GetComponent<BreakableItem>();
	}

	private void Update()
	{
		// Set the "Has_Hit_Bin" in the dialogue system to true.
		if (_BreakableItem.m_DamageState == ItemDamageStates.Broken)
			DialogueLua.SetVariable("Has_Hit_Bin", true);
	}
}
}