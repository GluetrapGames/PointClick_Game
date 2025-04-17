using EditorAttributes;
using UnityEngine;
using UnityEngine.VFX;

namespace GlueTrap
{
public class TestVFX : MonoBehaviour
{
	[SerializeField]
	private VisualEffect _vfx;

	public void PlayCustom(string eventName)
	{
		_vfx.SendEvent(eventName); // For custom events
	}

	[Button("Play Custom")]
	public void PlayEffect()
	{
		_vfx.Play(); // Triggers OnPlay block
	}
}
}